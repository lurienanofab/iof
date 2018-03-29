using System;

namespace IOF.Models
{
    public class StoreManagerReportItem
    {
        public int ItemID { get; set; }
        public string Description { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public DateTime? LastOrdered { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public int? StoreItemID { get; set; }
        public string StoreDescription { get; set; }
        public int? StorePackageQuantity { get; set; }
        public double? StorePackagePrice { get; set; }
        public double? StoreUnitPrice { get; set; }
        public DateTime? LastPurchased { get; set; }
    }
}
