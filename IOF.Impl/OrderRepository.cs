using IOF.Models;
using LNF.Ordering;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Data = LNF.Repository.Data;
using Ordering = LNF.Repository.Ordering;
using LNF.Models.Ordering;

namespace IOF.Impl
{
    public class OrderRepository : RepositoryBase, IOrderRepository
    {
        public IContext Context { get; }

        public OrderRepository(IContext context)
        {
            Context = context;
        }

        public Order Single(int poid)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);
            return CreateOrder(po);
        }

        public IEnumerable<Order> GetDrafts(int clientId)
        {
            var query = DA.Current.Query<Ordering.PurchaseOrder>().Where(x => x.Client.ClientID == clientId && x.Status.StatusID == Status.Draft);
            return CreateOrders(query);
        }

        public IEnumerable<Order> GetAwaitingApproval()
        {
            var query = DA.Current.Query<Ordering.PurchaseOrder>().Where(x => x.Status.StatusID == Status.AwaitingApproval);
            return CreateOrders(query);
        }

        public OrderSummary GetOrderSummary(int clientId)
        {
            var query = DA.Current.Query<Ordering.PurchaseOrder>().Where(x => x.Client.ClientID == clientId);

            var result = new OrderSummary()
            {
                Draft = query.Count(x => x.Status.StatusID == (int)OrderStatus.Draft),
                AwaitingApproval = query.Count(x => x.Status.StatusID == (int)OrderStatus.AwaitingApproval),
                Approved = query.Count(x => x.Status.StatusID == (int)OrderStatus.Approved && (x.RealPO == null || x.RealPO == string.Empty)),
                Ordered = query.Count(x => x.Status.StatusID == (int)OrderStatus.Ordered && !(x.RealPO == null || x.RealPO == string.Empty))
            };

            return result;
        }

        public void RequestApproval(int poid)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);
            po.Status = Ordering.Status.AwaitingApproval;
            TrackingUtility.Track(Ordering.TrackingCheckpoints.SentForApproval, po, Context.CurrentUser.ClientID);
        }

        public void Approve(int poid, int approverId)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);

            po.Status = Ordering.Status.Approved;
            po.RealApproverID = approverId;
            po.ApprovalDate = DateTime.Now;

            TrackingUtility.Track(Ordering.TrackingCheckpoints.Approved, po, approverId);
        }

        public void Reject(int poid, int approverId)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);
            po.Status = Ordering.Status.Draft;
            TrackingUtility.Track(Ordering.TrackingCheckpoints.Rejected, po, approverId);
        }

        public Order Copy(int poid, int? accountId = null)
        {
            //INSERT INTO dbo.PurchaseOrder (ClientID, AccountID, VendorID, CreatedDate, NeededDate, ApproverID, Oversized, ShippingMethodID, Notes, Attention, StatusID)
            //SELECT ClientID, AccountID, VendorID, GETDATE(), DATEADD(DAY, 7, GETDATE()), ApproverID, Oversized, ShippingMethodID, Notes, Attention, 1

            // get the po to be copied
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);

            // po may be for a different user than current, this will get the correct vendor and approver - making copies if necessary
            var vendor = GetVendorForCopy(po);
            var approver = GetApproverForCopy(po);

            var copy = new Ordering.PurchaseOrder()
            {
                Client = Require<Data.Client>(x => x.ClientID, Context.CurrentUser.ClientID),
                AccountID = accountId ?? po.AccountID,
                Vendor = vendor,
                CreatedDate = DateTime.Now,
                NeededDate = DateTime.Now.AddDays(7),
                Approver = approver,
                Oversized = po.Oversized,
                ShippingMethod = po.ShippingMethod,
                Notes = po.Notes,
                Attention = po.Attention,
                Status = Ordering.Status.Draft
            };

            DA.Current.Insert(copy);

            // po may be for a different user than current, this will get the correct details - making item copies if necessary
            var details = GetDetailsForCopy(po, copy, vendor);

            DA.Current.Insert(details);

            copy.Details = details;

            TrackingUtility.Track(Ordering.TrackingCheckpoints.DraftCreated, copy, Context.CurrentUser.ClientID);

            return CreateOrder(copy);
        }

        public void ManuallyProcess(int poid)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);
            po.Status = Ordering.Status.ProcessedManually;
            TrackingUtility.Track(Ordering.TrackingCheckpoints.ManuallyProcessed, po, Context.CurrentUser.ClientID);
        }

        public bool IsInventoryControlled(int poid)
        {
            var result = DA.Current.Query<Ordering.PurchaseOrderDetail>().Any(x => x.PurchaseOrder.POID == poid && x.Item.InventoryItemID.HasValue);
            return result;
        }

        public bool IsClaimed(int poid, out int purchaserId, out string purchaserName, out string reqNum, out string realPO, out string purchNotes)
        {
            var po = DA.Current.Single<Ordering.PurchaseOrder>(poid);

            var result = false;

            purchaserId = 0;
            purchaserName = string.Empty;
            reqNum = string.Empty;
            realPO = string.Empty;
            purchNotes = string.Empty;

            if (po == null) return false;

            if (po.PurchaserID.HasValue)
            {
                purchaserId = po.PurchaserID.Value;
                purchaserName = po.GetPurchaser().DisplayName;
                realPO = po.RealPO;
                result = true;
            }

            reqNum = po.ReqNum;
            purchNotes = po.PurchaserNotes;

            return result;
        }

        public void Claim(int poid, int clientId)
        {
            var po = DA.Current.Single<Ordering.PurchaseOrder>(poid);
            var ps = DA.Current.Query<Ordering.PurchaserSearch>().FirstOrDefault(x => x.POID == poid);
            var purchaser = DA.Current.Single<Data.Client>(clientId);

            if (po != null && ps != null && purchaser != null)
            {
                po.PurchaserID = purchaser.ClientID;

                // the view must also be updated or PurchaserSearch results won't change
                ps.PurchaserID = purchaser.ClientID;

                TrackingUtility.Track(Ordering.TrackingCheckpoints.Claimed, po, clientId);
            }
        }

        public void SaveRealPO(int poid, string reqNum, string realPO, string purchNotes)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);

            string currentRealPO;
            bool currentIsOrdered;

            currentRealPO = po.RealPO;
            currentIsOrdered = po.Status == Ordering.Status.Ordered;

            Ordering.TrackingCheckpoints checkpoint = 0;
            bool track = false;

            if (currentIsOrdered)
            {
                checkpoint = Ordering.TrackingCheckpoints.Modified;
                track = currentIsOrdered && currentRealPO != realPO;
            }
            else
            {
                if (!string.IsNullOrEmpty(realPO))
                {
                    po.Status = Ordering.Status.Ordered;
                    checkpoint = Ordering.TrackingCheckpoints.Ordered;
                    track = true;
                }
            }

            string realPoValue = null;

            if (string.IsNullOrEmpty(realPO))
            {
                if (currentIsOrdered)
                {
                    // for already ordered PO do not allow saving an empty RealPO value
                    realPoValue = currentRealPO;
                }
            }
            else
            {
                realPoValue = realPO;
            }

            po.ReqNum = reqNum;
            po.RealPO = realPoValue;
            po.PurchaserNotes = purchNotes;

            if (track)
            {
                // only track if order status set (TrackingCheckpoints.Ordered) or the PO has
                // already been ordered and the RealPO is changing (TrackingCheckpoints.Modified)
                TrackingUtility.Track(checkpoint, po, Context.CurrentUser.ClientID, new { po.ReqNum, po.RealPO, po.PurchaserNotes });
            }
        }

        public void Cancel(int poid)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);
            po.Status = Ordering.Status.Cancelled;
            TrackingUtility.Track(Ordering.TrackingCheckpoints.Cancelled, po, Context.CurrentUser.ClientID);
        }

        public Order Add(int clientId, int vendorId, int? accountId, int approverId, DateTime neededDate, bool oversized, int shippingMethodId, string notes, bool attention)
        {
            /*
            INSERT INTO dbo.PurchaseOrder (ClientID, AccountID, VendorID, CreatedDate, NeededDate, ApproverID, Oversized, ShippingMethodID, Notes, Attention, StatusID)
            VALUES (@ClientID, @AccountID, @VendorID, @CreatedDate, @NeededDate, @ApproverID, @Oversized, @ShippingMethodID, @Notes, @Attention, 1)

            1 = Status.Draft
            */

            var po = new Ordering.PurchaseOrder()
            {
                Client = Require<Data.Client>(x => x.ClientID, clientId),
                AccountID = accountId,
                Vendor = Require<Ordering.Vendor>(x => x.VendorID, vendorId),
                CreatedDate = DateTime.Now,
                NeededDate = neededDate,
                Approver = Require<Data.Client>(x => x.ClientID, approverId),
                Oversized = oversized,
                ShippingMethod = Require<Ordering.ShippingMethod>(x => x.ShippingMethodID, shippingMethodId),
                Notes = notes,
                Attention = attention,
                Status = Ordering.Status.Draft,
                Details = new List<Ordering.PurchaseOrderDetail>()
            };

            DA.Current.Insert(po);

            TrackingUtility.Track(Ordering.TrackingCheckpoints.DraftCreated, po, clientId, new { VendorID = vendorId, AccountID = accountId, ApproverID = approverId });

            return CreateOrder(po);
        }

        public void Update(int poid, int? accountId, int approverId, DateTime neededDate, bool oversized, int shippingMethodId, string notes, bool attention)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);

            if (po.Approver.ClientID != approverId)
                po.Approver = Require<Data.Client>(x => x.ClientID, approverId);

            if (po.ShippingMethod.ShippingMethodID != shippingMethodId)
                po.ShippingMethod = Require<Ordering.ShippingMethod>(x => x.ShippingMethodID, shippingMethodId);

            po.AccountID = accountId;
            po.NeededDate = Convert.ToDateTime(neededDate);
            po.Oversized = oversized;
            po.Notes = notes;
            po.Attention = attention;
        }

        public bool DeleteDraft(int poid)
        {
            var po = Require<Ordering.PurchaseOrder>(x => x.POID, poid);

            if (po.Status == Ordering.Status.Draft)
            {
                TrackingUtility.Track(Ordering.TrackingCheckpoints.Deleted, po, Context.CurrentUser.ClientID);
                DA.Current.Delete(po.Details);
                DA.Current.Delete(po);
                return true;
            }

            return false;
        }

        public IEnumerable<ShippingMethod> GetAllShippingMethods()
        {
            return DA.Current.Query<Ordering.ShippingMethod>().Select(x => new ShippingMethod()
            {
                ShippingMethodID = x.ShippingMethodID,
                ShippingMethodName = x.ShippingMethodName
            }).ToList();
        }

        private static Order CreateOrder(Ordering.PurchaseOrder po)
        {
            if (po == null) return null;

            return new Order()
            {
                POID = po.POID,
                ClientID = po.Client.ClientID,
                DisplayName = po.Client.DisplayName,
                VendorID = po.Vendor.VendorID,
                VendorName = po.Vendor.VendorName,
                AccountID = po.AccountID,
                ApproverID = po.Approver.ClientID,
                ApproverName = po.Approver.DisplayName,
                CreatedDate = po.CreatedDate,
                NeededDate = po.NeededDate,
                Oversized = po.Oversized,
                ShippingMethodID = po.ShippingMethod.ShippingMethodID,
                ShippingMethodName = po.ShippingMethod.ShippingMethodName,
                Notes = po.Notes,
                StatusID = po.Status.StatusID,
                StatusName = po.Status.StatusName,
                CompletedDate = po.CompletedDate,
                RealApproverID = po.RealApproverID,
                ApprovalDate = po.ApprovalDate,
                Attention = po.Attention,
                PurchaserID = po.PurchaserID,
                RealPO = po.RealPO,
                ReqNum = po.ReqNum,
                PurchaserNotes = po.PurchaserNotes,
                TotalPrice = po.Details.Sum(x => (double?)(x.Quantity * x.UnitPrice)).GetValueOrDefault(0)
            };
        }
        [Obsolete("Dont use the view because it doesn't update when entities are modified/deleted.")]
        private static Order CreateOrder(Ordering.PurchaseOrderInfo po)
        {
            if (po == null) return null;

            return new Order()
            {
                POID = po.POID,
                ClientID = po.ClientID,
                DisplayName = po.DisplayName,
                VendorID = po.VendorID,
                VendorName = po.VendorName,
                AccountID = po.AccountID,
                ApproverID = po.ApproverID,
                ApproverName = po.ApproverName,
                CreatedDate = po.CreatedDate,
                NeededDate = po.NeededDate,
                Oversized = po.Oversized,
                ShippingMethodID = po.ShippingMethodID,
                ShippingMethodName = po.ShippingMethodName,
                Notes = po.Notes,
                StatusID = po.StatusID,
                StatusName = po.StatusName,
                CompletedDate = po.CompletedDate,
                RealApproverID = po.RealApproverID,
                ApprovalDate = po.ApprovalDate,
                Attention = po.Attention,
                PurchaserID = po.PurchaserID,
                RealPO = po.RealPO,
                ReqNum = po.ReqNum,
                PurchaserNotes = po.PurchaserNotes,
                TotalPrice = po.TotalPrice
            };
        }

        [Obsolete("Dont use the view because it doesn't update when entities are modified/deleted.")]
        private static IEnumerable<Order> CreateOrders(IQueryable<Ordering.PurchaseOrderInfo> query)
        {
            var result = query.Select(x => new Order()
            {
                POID = x.POID,
                ClientID = x.ClientID,
                DisplayName = x.DisplayName,
                VendorID = x.VendorID,
                VendorName = x.VendorName,
                AccountID = x.AccountID,
                ApproverID = x.ApproverID,
                ApproverName = x.ApproverName,
                ApprovalDate = x.ApprovalDate,
                Attention = x.Attention,
                CompletedDate = x.CompletedDate,
                CreatedDate = x.CreatedDate,
                NeededDate = x.NeededDate,
                Notes = x.Notes,
                Oversized = x.Oversized,
                PurchaserID = x.PurchaserID,
                PurchaserNotes = x.PurchaserNotes,
                RealApproverID = x.RealApproverID,
                RealPO = x.RealPO,
                ReqNum = x.ReqNum,
                ShippingMethodID = x.ShippingMethodID,
                ShippingMethodName = x.ShippingMethodName,
                StatusID = x.StatusID,
                StatusName = x.StatusName,
                TotalPrice = x.TotalPrice
            }).ToList();

            return result;
        }

        private static IEnumerable<Order> CreateOrders(IQueryable<Ordering.PurchaseOrder> query)
        {
            var result = query.Select(x => new Order()
            {
                POID = x.POID,
                ClientID = x.Client.ClientID,
                DisplayName = $"{x.Client.LName}, {x.Client.FName}",
                VendorID = x.Vendor.VendorID,
                VendorName = x.Vendor.VendorName,
                AccountID = x.AccountID,
                ApproverID = x.Approver.ClientID,
                ApproverName = $"{x.Approver.LName}, {x.Approver.FName}",
                ApprovalDate = x.ApprovalDate,
                Attention = x.Attention,
                CompletedDate = x.CompletedDate,
                CreatedDate = x.CreatedDate,
                NeededDate = x.NeededDate,
                Notes = x.Notes,
                Oversized = x.Oversized,
                PurchaserID = x.PurchaserID,
                PurchaserNotes = x.PurchaserNotes,
                RealApproverID = x.RealApproverID,
                RealPO = x.RealPO,
                ReqNum = x.ReqNum,
                ShippingMethodID = x.ShippingMethod.ShippingMethodID,
                ShippingMethodName = x.ShippingMethod.ShippingMethodName,
                StatusID = x.Status.StatusID,
                StatusName = x.Status.StatusName,
                TotalPrice = x.Details.Sum(i => (double?)(i.Quantity * i.UnitPrice)).GetValueOrDefault(0)
            }).ToList();

            return result;
        }

        private Ordering.Vendor GetVendorForCopy(Ordering.PurchaseOrder po)
        {
            var currentClientId = Context.CurrentUser.ClientID;

            Ordering.Vendor vendor;

            if (po.Client.ClientID == currentClientId || po.Vendor.ClientID == 0)
            {
                // current user is copying own order, or store manager order
                vendor = po.Vendor;
            }
            else
            {
                // current user is copying another user's order

                // check for a vendor for the current user that has the same name as the po vendor
                vendor = DA.Current.Query<Ordering.Vendor>().Where(x => x.ClientID == currentClientId).ToList().FirstOrDefault(x =>
                    Ordering.PurchaseOrderItem.CleanString(x.VendorName) == Ordering.PurchaseOrderItem.CleanString(po.Vendor.VendorName));

                if (vendor == null)
                {
                    // make a copy
                    vendor = new Ordering.Vendor()
                    {
                        Active = true,
                        Address1 = po.Vendor.Address1,
                        Address2 = po.Vendor.Address2,
                        Address3 = po.Vendor.Address3,
                        ClientID = currentClientId,
                        Contact = po.Vendor.Contact,
                        Email = po.Vendor.Email,
                        Fax = po.Vendor.Fax,
                        Items = new List<Ordering.PurchaseOrderItem>(),
                        Phone = po.Vendor.Phone,
                        URL = po.Vendor.URL,
                        VendorName = po.Vendor.VendorName
                    };

                    DA.Current.Insert(vendor);
                }
                else
                {
                    vendor.Active = true; //just in case
                }
            }

            return vendor;
        }

        private IList<Ordering.PurchaseOrderDetail> GetDetailsForCopy(Ordering.PurchaseOrder po, Ordering.PurchaseOrder copy, Ordering.Vendor vendor)
        {
            var currentClientId = Context.CurrentUser.ClientID;

            IList<Ordering.PurchaseOrderDetail> details;

            if (po.Client.ClientID == currentClientId || po.Vendor.ClientID == 0)
            {
                // current user is copying own order, or store manager vendor
                details = po.Details.Select(x => new Ordering.PurchaseOrderDetail()
                {
                    Category = x.Category,
                    IsInventoryControlled = x.IsInventoryControlled,
                    Item = x.Item,
                    PurchaseOrder = copy,
                    Quantity = x.Quantity,
                    ToInventoryDate = null,
                    Unit = x.Unit,
                    UnitPrice = x.UnitPrice
                }).ToList();
            }
            else
            {
                // current user is copying another user's order

                details = new List<Ordering.PurchaseOrderDetail>();

                // check for items for the current user that have the same description and partnum as the po detail items
                foreach (var d in po.Details)
                {
                    var i = vendor.Items.FirstOrDefault(x =>
                        Ordering.PurchaseOrderItem.CleanString(x.Description) == Ordering.PurchaseOrderItem.CleanString(d.Item.Description)
                        && Ordering.PurchaseOrderItem.CleanString(x.PartNum) == Ordering.PurchaseOrderItem.CleanString(d.Item.PartNum));

                    if (i == null)
                    {
                        // make a copy of the item
                        i = new Ordering.PurchaseOrderItem()
                        {
                            Active = true,
                            Description = d.Item.Description,
                            Details = new List<Ordering.PurchaseOrderDetail>(),
                            InventoryItemID = d.Item.InventoryItemID,
                            PartNum = d.Item.PartNum,
                            UnitPrice = d.Item.UnitPrice,
                            Vendor = vendor
                        };

                        DA.Current.Insert(i);

                        vendor.Items.Add(i);
                    }
                    else
                    {
                        i.Active = true; //just in case
                    }

                    var detail = new Ordering.PurchaseOrderDetail()
                    {
                        Category = d.Category,
                        IsInventoryControlled = d.IsInventoryControlled,
                        Item = i,
                        PurchaseOrder = copy,
                        Quantity = d.Quantity,
                        ToInventoryDate = null,
                        Unit = d.Unit,
                        UnitPrice = d.UnitPrice
                    };

                    details.Add(detail);
                }
            }

            return details;
        }

        private Data.Client GetApproverForCopy(Ordering.PurchaseOrder po)
        {
            var currentClientId = Context.CurrentUser.ClientID;

            Data.Client approver;

            if (po.Client.ClientID != currentClientId)
            {
                // check if the current user has the same approver
                var app = DA.Current.Query<Ordering.Approver>().FirstOrDefault(x => x.ApproverID == po.Approver.ClientID && x.ClientID == currentClientId);

                if (app == null)
                {
                    // use the current user's primary approver
                    var defapp = DA.Current.Query<Ordering.Approver>().FirstOrDefault(x => x.Active && x.ClientID == currentClientId && x.IsPrimary);

                    if (defapp == null)
                    {
                        // fall-back: copy the po approver
                        app = new Ordering.Approver()
                        {
                            Active = true,
                            ApproverID = po.Approver.ClientID,
                            ClientID = currentClientId,
                            IsPrimary = true //because the current user has no active primary at the moment
                        };

                        DA.Current.Insert(app);

                        approver = po.Approver;
                    }
                    else
                    {
                        approver = Require<Data.Client>(x => x.ClientID, defapp.ApproverID);
                    }
                }
                else
                {
                    app.Active = true; //just in case
                    approver = Require<Data.Client>(x => x.ClientID, app.ApproverID);
                }
            }
            else
            {
                approver = po.Approver;
            }

            return approver;
        }
    }
}
