namespace IOF.Models
{
    public class Account
    {
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public bool Active { get; set; }
        public string AccountDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(ShortCode))
                    return AccountName;
                else
                    return $"[{ShortCode}] {AccountName}";
            }
        }
    }
}
