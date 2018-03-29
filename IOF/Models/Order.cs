using System;

namespace IOF.Models
{
    public class Order
    {
        public int POID { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public int? AccountID { get; set; }
        public int ApproverID { get; set; }
        public string ApproverName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime NeededDate { get; set; }
        public bool Oversized { get; set; }
        public int ShippingMethodID { get; set; }
        public string Notes { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? RealApproverID { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public bool? Attention { get; set; }
        public int? PurchaserID { get; set; }
        public string RealPO { get; set; }
        public string ReqNum { get; set; }
        public string PurchaserNotes { get; set; }
        public double TotalPrice { get; set; }
        public string ShippingMethodName { get; set; }

        public bool IsApproved()
        {
            return ApprovalDate.HasValue;
        }
    }
}
