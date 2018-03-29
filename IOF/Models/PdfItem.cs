using System;

namespace IOF.Models
{
    public class PdfItem
    {
        public int PODID { get; set; }
        public int CategoryID { get; set; }
        public int ParentID { get; set; }
        public string CategoryNumber { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public double ExtPrice { get { return Quantity * UnitPrice; } }
        public DateTime CreatedDate { get; set; }
        public bool IsNotes { get; set; }
    }
}
