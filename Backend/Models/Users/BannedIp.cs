using Backend.Database;

namespace Backend.Models.Users
{
    public class BannedIp
    {
        public int BannedIpId { get; set; }
        public string IpAddress { get; set; }

        private BannedIp()
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

        public bool AlreadyExists()
        {
            List<BannedIp> ipsFromDb = context.BannedIps.Where(m => m.IpAddress == IpAddress).ToList();
            return ipsFromDb.Count > 0;
        }
    }
}
