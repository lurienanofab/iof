namespace IOF.Models
{
    public class Status
    {
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        public static int Draft
        {
            get { return 1; }
        }

        public static int AwaitingApproval
        {
            get { return 2; }
        }

        public static int Approved
        {
            get { return 3; }
        }

        public static int Ordered
        {
            get { return 4; }
        }

        public static int Completed
        {
            get { return 5; }
        }

        public static int Cancelled
        {
            get { return 6; }
        }

        public static int ProcessedManually
        {
            get { return 7; }
        }
    }
}
