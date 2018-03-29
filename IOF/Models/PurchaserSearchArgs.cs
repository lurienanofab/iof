using System;

namespace IOF.Models
{
    public class PurchaserSearchArgs : SearchArgs
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DateRangePreset { get; set; }
        public PurchaserClaimStatus ClaimStatus { get; set; }
        public int PurchaserClientID { get; set; }
        public int CreatorClientID { get; set; }
        public int POID { get; set; }
        public string RealPO { get; set; }
        public PurchaserOrderStatus OrderStatus { get; set; }
    }

    public enum PurchaserClaimStatus
    {
        All = 0,
        Unclaimed = 1,
        Claimed = 2,
        ClaimedBy = 3
    }

    public enum PurchaserOrderStatus
    {
        NotSpecified = 0,
        Unordered = 1,
        Ordered = 2,
    }
}
