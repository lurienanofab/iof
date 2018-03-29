using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOF.Models
{
    public class OrderSearchItem
    {
        public int ClientID { get; set; }
        public int POID { get; set; }
        public string DisplayName { get; set; }
        public string ApproverDisplayName { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ShortCode { get; set; }
        public int CategoryID { get; set; }
        public string CategoryNumber { get; set; }
        public string CategoryName { get; set; }
        public double TotalPrice { get; set; }
        public string StatusName { get; set; }
    }
}
