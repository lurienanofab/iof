using System;

namespace IOF.Models
{
    public class Detail
    {
        public int PODID { get; set; }
        public int POID { get; set; }
        public int ItemID { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNumber { get; set; }
        public bool CategoryActive { get; set; }
        public int ParentID { get; set; }
        public string ParentCategoryName { get; set; }
        public string ParentCategoryNumber { get; set; }
        public bool ParentCategoryActive { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string Unit { get; set; }
        public DateTime? ToInventoryDate { get; set; }
        public bool? IsInventoryControlled { get; set; }
        public double ExtPrice { get { return Quantity * UnitPrice; } }
        public DateTime CreatedDate { get; set; }
        public int? InventoryItemID { get; set; }
        public bool IsCategoryActive() => ParentCategoryActive && CategoryActive;
    }
}
