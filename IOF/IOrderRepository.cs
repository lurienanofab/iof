using IOF.Models;
using System;
using System.Collections.Generic;

namespace IOF
{
    public interface IOrderRepository
    {
        Order Single(int poid);
        IEnumerable<Order> GetDrafts(int clientId);
        IEnumerable<Order> GetAwaitingApproval();
        OrderSummary GetOrderSummary(int clientId);
        Order Copy(int original, int? accountId = null);
        void RequestApproval(int poid);
        void Approve(int poid, int approverId);
        void Reject(int poid, int approverId);
        void ManuallyProcess(int poid);
        bool IsInventoryControlled(int poid);
        bool IsClaimed(int poid, out int purchaserId, out string purchaserName, out string reqNum, out string realPO, out string purchNotes);        
        void Claim(int poid, int clientId);
        void SaveRealPO(int poid, string reqNum, string realPO, string purchNotes);
        void Cancel(int poid);
        Order Add(int clientId, int vendorId, int? accountId, int approverId, DateTime neededDate, bool oversized, int shippingMethodId, string notes, bool attention);
        void Update(int poid, int? accountId, int approverId, DateTime neededDate, bool oversized, int shippingMethodId, string notes, bool attention);
        bool DeleteDraft(int poid);
        IEnumerable<ShippingMethod> GetAllShippingMethods();
    }
}
