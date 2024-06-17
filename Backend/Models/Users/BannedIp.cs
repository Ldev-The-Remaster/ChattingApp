using Backend.Database;

namespace Backend.Models.Users
{
    public class BannedIp
    {
        public int BannedIpId { get; set; }
        public string IpAddress { get; set; }
        public string BanReason { get; set; }

        private BannedIp()
        {
            IpAddress = String.Empty;
            BanReason = String.Empty;
        }
        public BannedIp(string ip, string banReason)
        {
            IpAddress = ip;
            BanReason = banReason;
        }

        //Persistence
        public static BannedIpContext context = new BannedIpContext();

        public void SaveToDb()
        {
            context.Add(this);
            context.SaveChanges();
        }

        public void RemoveFromDb()
        {
            context.Remove(this);
            context.SaveChanges();
        }

        public static BannedIp? GetBannedIpFromDb(string bannedIp)
        {
            List<BannedIp> bannedIpsFromDb = context.BannedIps.Where(m => m.IpAddress == bannedIp).ToList();
            if (bannedIpsFromDb.Count == 0) return null;
            return bannedIpsFromDb.First();
        }

        public static bool AlreadyExists(string ip)
        {
            List<BannedIp> ipsFromDb = context.BannedIps.Where(m => m.IpAddress == ip).ToList();
            return ipsFromDb.Count > 0;
        }
    }
}
