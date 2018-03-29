using IOF.Models;
using LNF.Repository;
using System.Collections.Generic;
using System.Linq;
using Inventory = LNF.Repository.Inventory;
using Ordering = LNF.Repository.Ordering;

namespace IOF.Impl
{
    public class ItemRepository : RepositoryBase, IItemRepository
    {
        public Item Single(int itemId)
        {
            var item = Require<Ordering.PurchaseOrderItem>(x => x.ItemID, itemId);
            return CreateItem(item);
        }

        public IEnumerable<Item> GetOrderItems(int poid, bool? active = true)
        {
            var query = DA.Current.Query<Ordering.PurchaseOrderDetail>().Where(x => x.PurchaseOrder.POID == poid && x.Item.Active == active.GetValueOrDefault(x.Item.Active));
            return CreateItems(query);
        }

        public IEnumerable<Item> GetVendorItems(int vendorId, bool? active = true)
        {
            var query = DA.Current.Query<Ordering.PurchaseOrderItem>().Where(x => x.Vendor.VendorID == vendorId && x.Active == active.GetValueOrDefault(x.Active));
            return CreateItems(query);
        }

        public IEnumerable<Item> GetClientItems(int clientId, bool? active = true)
        {
            var query = DA.Current.Query<Ordering.PurchaseOrderItem>().Where(x => x.Vendor.ClientID == clientId && x.Active == active.GetValueOrDefault(x.Active));
            return CreateItems(query);
        }

        public IEnumerable<Item> Copy(int toVendorId, int fromVendorId)
        {
            var toVendor = Require<Ordering.Vendor>(x => x.VendorID, toVendorId);
            var fromVendor = Require<Ordering.Vendor>(x => x.VendorID, fromVendorId);

            var items = fromVendor.Items.Select(x => new Ordering.PurchaseOrderItem()
            {
                Active = true,
                Description = x.Description,
                InventoryItemID = x.InventoryItemID,
                PartNum = x.PartNum,
                UnitPrice = x.UnitPrice,
                Vendor = toVendor
            }).ToList();

            DA.Current.Insert(items);

            // return the new items
            return CreateItems(items.AsQueryable());
        }

        public InventoryItem GetInventoryItem(Item item)
        {
            if (item.InventoryItemID.HasValue)
            {
                var inventoryItem = Require<Inventory.Item>(x => x.ItemID, item.InventoryItemID.Value);
                return CreateInventoryItem(inventoryItem);
            }

            return null;
        }

        public IEnumerable<InventoryItem> GetInventoryItems()
        {
            var query = DA.Current.Query<Inventory.Item>().Where(x => x.Active);
            return CreateInventoryItems(query);
        }

        public Item Add(string partNum, string description, double unitPrice, int vendorId, int? inventoryItemId)
        {
            var item = new Ordering.PurchaseOrderItem()
            {
                Active = true,
                Description = description,
                InventoryItemID = inventoryItemId,
                PartNum = partNum,
                UnitPrice = unitPrice,
                Vendor = DA.Current.Single<Ordering.Vendor>(vendorId)
            };

            DA.Current.Insert(item);

            return CreateItem(item);
        }

        public void Update(int itemId, string partNum, string description, double unitPrice, int? inventoryItemId)
        {
            var item = Require<Ordering.PurchaseOrderItem>(x => x.ItemID, itemId);
            item.PartNum = partNum;
            item.Description = description;
            item.UnitPrice = unitPrice;
            item.InventoryItemID = inventoryItemId;
        }

        public void Delete(int itemId)
        {
            var item = Require<Ordering.PurchaseOrderItem>(x => x.ItemID, itemId);
            item.Active = false;
        }

        public void Restore(int itemId)
        {
            var item = Require<Ordering.PurchaseOrderItem>(x => x.ItemID, itemId);
            item.Active = true;
        }

        private Item CreateItem(Ordering.PurchaseOrderItem item)
        {
            return new Item()
            {
                ItemID = item.ItemID,
                VendorID = item.Vendor.VendorID,
                VendorName = item.Vendor.VendorName,
                PartNum = item.PartNum,
                Description = item.Description,
                UnitPrice = item.UnitPrice,
                InventoryItemID = item.InventoryItemID,
                Active = item.Active
            };
        }

        private IEnumerable<Item> CreateItems(IQueryable<Ordering.PurchaseOrderItem> query)
        {
            return query.Select(x => new Item()
            {
                ItemID = x.ItemID,
                VendorID = x.Vendor.VendorID,
                VendorName = x.Vendor.VendorName,
                PartNum = x.PartNum,
                Description = x.Description,
                UnitPrice = x.UnitPrice,
                InventoryItemID = x.InventoryItemID,
                Active = x.Active
            }).ToList();
        }

        private IEnumerable<Item> CreateItems(IQueryable<Ordering.PurchaseOrderDetail> query)
        {
            return query.Select(x => new Item()
            {
                ItemID = x.Item.ItemID,
                VendorID = x.Item.Vendor.VendorID,
                VendorName = x.Item.Vendor.VendorName,
                PartNum = x.Item.PartNum,
                Description = x.Item.Description,
                UnitPrice = x.Item.UnitPrice,
                InventoryItemID = x.Item.InventoryItemID,
                Active = x.Item.Active
            }).ToList();
        }

        private InventoryItem CreateInventoryItem(Inventory.Item inventoryItem)
        {
            return new InventoryItem()
            {
                InventoryItemID = inventoryItem.ItemID,
                Description = inventoryItem.Description,
                Active = inventoryItem.Active
            };
        }

        private IEnumerable<InventoryItem> CreateInventoryItems(IQueryable<Inventory.Item> query)
        {
            return query.Select(x => new InventoryItem()
            {
                InventoryItemID = x.ItemID,
                Description = x.Description,
                Active = x.Active
            }).ToList();
        }
    }
}
