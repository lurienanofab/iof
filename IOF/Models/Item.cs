namespace IOF.Models
{
    public class Item
    {
        public int ItemID { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
        public double UnitPrice { get; set; }
        public int? InventoryItemID { get; set; }
        public bool Active { get; set; }
    }
}
