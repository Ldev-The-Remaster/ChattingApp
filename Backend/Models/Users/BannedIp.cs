namespace Backend.Models.Users
{
    public class BannedIp
    {
        public int IpId { get; set; }
        public string IpAddress { get; set; }

        public BannedIp()
        {
            IpAddress = String.Empty;
        }
        public BannedIp(string ip)
        {
            IpAddress = ip;
        }
    }
}
