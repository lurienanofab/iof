using System;

namespace IOF.Models
{
    public class OrderSearchArgs : SearchArgs
    {
        public int ClientID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string Keywords { get; set; }
        public string PartNumber { get; set; }
        public int POID { get; set; }
        public string ShortCode { get; set; }
        public int OtherClientID { get; set; }
        public bool IncludeSelf { get; set; }
        public OrderDisplayOption DisplayOption { get; set; }
    }

    public enum OrderDisplayOption
    {
        Detail = 0,
        Summary = 1
    }
}