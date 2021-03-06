﻿using IOF.Models;
using LNF;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Ordering = LNF.Impl.Repository.Ordering;

namespace IOF.Impl
{
    public class DetailRepository : RepositoryBase, IDetailRepository
    {
        public IContext Context { get; }
        public IItemRepository ItemRepository { get; }

        private IEnumerable<Ordering.PurchaseOrderCategory> _parents;

        public DetailRepository(IProvider provider, IContext context, IItemRepository itemRepo) : base(provider)
        {
            Context = context;
            ItemRepository = itemRepo;
        }

        public Detail Single(int podid)
        {
            var pod = Require<Ordering.PurchaseOrderDetail>(x => x.PODID, podid);
            return CreateDetail(pod);
        }

        public IEnumerable<Detail> GetOrderDetails(int poid)
        {
            var query = DataSession.Query<Ordering.PurchaseOrderDetail>().Where(x => x.PurchaseOrder.POID == poid);
            return CreateDetails(query);
        }

        public IEnumerable<Detail> GetItemDetails(int itemId)
        {
            var query = DataSession.Query<Ordering.PurchaseOrderDetail>().Where(x => x.Item.ItemID == itemId);
            return CreateDetails(query);
        }

        public Detail Add(int poid, int itemId, int catId, double qty, string unit, double unitPrice)
        {
            var item = Require<Ordering.PurchaseOrderItem>(x => x.ItemID, itemId);

            var pod = new Ordering.PurchaseOrderDetail()
            {
                PurchaseOrder = Require<Ordering.PurchaseOrder>(x => x.POID, poid),
                Item = item,
                Category = Require<Ordering.PurchaseOrderCategory>(x => x.CatID, catId),
                IsInventoryControlled = item.InventoryItemID.HasValue,
                Quantity = qty,
                Unit = unit,
                UnitPrice = unitPrice
            };

            DataSession.Insert(pod);

            return CreateDetail(pod);
        }

        public void Update(int podid, int catId, double qty, string unit, double unitPrice)
        {
            var pod = Require<Ordering.PurchaseOrderDetail>(x => x.PODID, podid);

            if (pod.Category.CatID != catId)
                pod.Category = Require<Ordering.PurchaseOrderCategory>(x => x.CatID, catId);

            pod.Quantity = qty;
            pod.Unit = unit;
            pod.UnitPrice = unitPrice;
        }

        public IEnumerable<string> PurchaserUpdate(int podid, string partNum, double quantity, double unitPrice, bool updateInventory)
        {
            var detail = Require<Ordering.PurchaseOrderDetail>(x => x.PODID, podid);
            var item = detail.Item;

            var changes = new List<string>();

            bool track = false;

            if (item.PartNum != partNum)
            {
                changes.Add($"PartNum: changed from {item.PartNum} to {partNum}");
                item.PartNum = partNum;
            }

            if (detail.Quantity != quantity)
            {
                changes.Add($"Quantity: changed from {detail.Quantity} to {quantity}");
                detail.Quantity = quantity;
                track = true;
            }

            if (detail.UnitPrice != unitPrice)
            {
                changes.Add($"PO Item Unit Price: changed from {detail.UnitPrice:C} to {unitPrice:C}");
                detail.UnitPrice = unitPrice;
                track = true;
            }

            if (updateInventory)
            {
                if (item.UnitPrice != unitPrice)
                {
                    changes.Add($"Inventory Unit Price: changed from {item.UnitPrice:C} to {unitPrice:C} (price will be used on future orders)");
                    item.UnitPrice = unitPrice;
                    track = true;
                }
            }

            // track the iof modification
            if (track)
                LNF.Ordering.Tracking.Track(LNF.Ordering.TrackingCheckpoints.Modified, detail.PurchaseOrder.POID, Context.CurrentUser.ClientID);

            return changes;
        }

        public void Delete(int podid)
        {
            var pod = Require<Ordering.PurchaseOrderDetail>(x => x.PODID, podid);
            DataSession.Delete(pod);
        }

        public Category GetCategory(Detail detail)
        {
            var category = Require<Ordering.PurchaseOrderCategory>(x => x.CatID, detail.CategoryID);
            return CreateCategory(category);
        }

        public IEnumerable<Category> GetParentCategories()
        {
            var query = SelectParentCategories().Where(x => x.Active);
            return CreateCategories(query);
        }

        public IEnumerable<Category> GetChildCategories(int parentId)
        {
            var query = DataSession.Query<Ordering.PurchaseOrderCategory>().Where(x => x.Active && x.ParentID == parentId);
            return CreateCategories(query);
        }

        public int AddCategory(int parentId, string categoryName, string categoryNumber)
        {
            var existing = DataSession.Query<Ordering.PurchaseOrderCategory>().FirstOrDefault(x => x.ParentID == parentId && x.CatNo == categoryNumber);

            if (existing != null)
            {
                if (existing.Active)
                {
                    throw new Exception($"A category already exists with category number: {categoryNumber}");
                }
                else
                {
                    existing.Active = true;
                    return existing.CatID;
                }
            }

            var cat = new Ordering.PurchaseOrderCategory()
            {
                ParentID = parentId,
                CatName = categoryName,
                CatNo = categoryNumber,
                Active = true
            };

            DataSession.Insert(cat);

            return cat.CatID;
        }

        public void ModifyCategory(int categoryId, string categoryName, string categoryNumber)
        {
            var cat = Require<Ordering.PurchaseOrderCategory>(x => x.CatID, categoryId);

            var existing = DataSession.Query<Ordering.PurchaseOrderCategory>().FirstOrDefault(x => x.ParentID == cat.ParentID && x.CatNo == categoryNumber && x.CatID != categoryId);

            if (existing != null)
            {
                if (existing.Active)
                {
                    throw new Exception($"A category already exists with category number: {categoryNumber}");
                }
                else
                {
                    existing.Active = true;
                    return;
                }
            }

            cat.CatName = categoryName;
            cat.CatNo = categoryNumber;
        }

        public void DeleteCategory(int categoryId)
        {
            var cat = Require<Ordering.PurchaseOrderCategory>(x => x.CatID, categoryId);
            cat.Active = false;
        }

        public IEnumerable<Detail> GetInvalidCategoryItems(int poid)
        {
            var result = new List<Detail>();

            var parents = SelectParentCategories();

            var details = DataSession.Query<Ordering.PurchaseOrderDetail>().Where(x => x.PurchaseOrder.POID == poid);

            foreach (var d in details)
            {
                if (!d.Category.Active)
                    result.Add(CreateDetail(d));
                else
                {
                    if (!d.Category.IsParent())
                    {
                        var p = parents.FirstOrDefault(x => x.CatID == d.Category.ParentID);
                        if (p == null) throw new Exception($"No parent category found for CatID = {d.Category.CatID}");
                        if (!p.Active)
                            result.Add(CreateDetail(d));
                    }
                }
            }

            return result;
        }

        private IEnumerable<Ordering.PurchaseOrderCategory> SelectParentCategories()
        {
            if (_parents == null)
                _parents = DataSession.Query<Ordering.PurchaseOrderCategory>().Where(x => x.ParentID == 0).ToList();
            return _parents;
        }

        private Ordering.PurchaseOrderCategory FindParentCategory(Ordering.PurchaseOrderDetail pod)
        {
            Ordering.PurchaseOrderCategory p;

            if (pod.Category.IsParent())
                p = pod.Category;
            else
                p = SelectParentCategories().First(x => x.CatID == pod.Category.ParentID);

            return p;
        }

        private Detail CreateDetail(Ordering.PurchaseOrderDetail pod)
        {
            return new Detail()
            {
                PODID = pod.PODID,
                POID = pod.PurchaseOrder.POID,
                ItemID = pod.Item.ItemID,
                PartNum = pod.Item.PartNum,
                Description = pod.Item.Description,
                CategoryID = pod.Category.CatID,
                CategoryName = pod.Category.CatName,
                CategoryNumber = pod.Category.CatNo,
                CategoryActive = pod.Category.Active,
                ParentID = pod.Category.ParentID,
                ParentCategoryName = FindParentCategory(pod).CatName,
                ParentCategoryNumber = FindParentCategory(pod).CatNo,
                ParentCategoryActive = FindParentCategory(pod).Active,
                Quantity = pod.Quantity,
                UnitPrice = pod.UnitPrice,
                Unit = pod.Unit,
                ToInventoryDate = pod.ToInventoryDate,
                IsInventoryControlled = pod.IsInventoryControlled,
                CreatedDate = pod.PurchaseOrder.CreatedDate,
                InventoryItemID = pod.Item.InventoryItemID
            };
        }

        private IEnumerable<Detail> CreateDetails(IQueryable<Ordering.PurchaseOrderDetail> query)
        {
            return query.Select(x => new Detail()
            {
                PODID = x.PODID,
                POID = x.PurchaseOrder.POID,
                ItemID = x.Item.ItemID,
                PartNum = x.Item.PartNum,
                Description = x.Item.Description,
                CategoryID = x.Category.CatID,
                CategoryName = x.Category.CatName,
                CategoryNumber = x.Category.CatNo,
                CategoryActive = x.Category.Active,
                ParentID = x.Category.ParentID,
                ParentCategoryName = FindParentCategory(x).CatName,
                ParentCategoryNumber = FindParentCategory(x).CatNo,
                ParentCategoryActive = FindParentCategory(x).Active,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                Unit = x.Unit,
                ToInventoryDate = x.ToInventoryDate,
                IsInventoryControlled = x.IsInventoryControlled,
                CreatedDate = x.PurchaseOrder.CreatedDate,
                InventoryItemID = x.Item.InventoryItemID
            }).ToList();
        }

        private Category CreateCategory(Ordering.PurchaseOrderCategory category)
        {
            return new Category()
            {
                CategoryID = category.CatID,
                ParentID = category.ParentID,
                CategoryName = category.CatName,
                CategoryNumber = category.CatNo,
                Active = category.Active
            };
        }

        private IEnumerable<Category> CreateCategories(IEnumerable<Ordering.PurchaseOrderCategory> query)
        {
            return query.Select(x => new Category()
            {
                CategoryID = x.CatID,
                ParentID = x.ParentID,
                CategoryName = x.CatName,
                CategoryNumber = x.CatNo,
                Active = x.Active
            }).ToList();
        }
    }
}
