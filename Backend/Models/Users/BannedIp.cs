using Backend.Database;

namespace Backend.Models.Users
{
    public class BannedIp
    {
        public int BannedIpId { get; set; }
        public string IpAddress { get; set; }

        public BannedIp()
        {
            IpAddress = String.Empty;
        }
        public BannedIp(string ip)
        {
            IpAddress = ip;
        }

        //Persistence
        public static BannedIpContext context = new BannedIpContext();

        public void SaveToDb()
        {
            context.Add(this);
            context.SaveChanges();
        }
    }
}
