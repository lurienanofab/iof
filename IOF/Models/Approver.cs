namespace IOF.Models
{
    public class Approver : Client
    {
        public int ApproverID { get; set; }
        public bool IsPrimary { get; set; }
        public bool Active { get; set; }
    }
}
