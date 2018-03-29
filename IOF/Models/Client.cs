namespace IOF.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DisplayName => $"{LName}, {FName}";
    }
}
