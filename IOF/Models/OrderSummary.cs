namespace IOF.Models
{
    public class OrderSummary
    {
        public int Draft { get; set; }
        public int AwaitingApproval { get; set; }
        public int Approved { get; set; }
        public int Ordered { get; set; }
    }
}
