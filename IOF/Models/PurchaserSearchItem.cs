using System;

namespace IOF.Models
{
    public class PurchaserSearchItem
    {
        public int POID { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public int? PurchaserID { get; set; }
        public string PurchaserName { get; set; }
        public double TotalPrice { get; set; }
        public string RealPO { get; set; }
    }
}
