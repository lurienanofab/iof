namespace IOF.Models
{
    public class Purchaser : Client
    {
        public int PurchaserID { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
