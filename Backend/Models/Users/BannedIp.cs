using Backend.Database;

namespace Backend.Models.Users
{
    public class BannedIp
    {
        #region Properties

        public int BannedIpId { get; set; }
        public string IpAddress { get; set; }
        public string BanReason { get; set; }

        #endregion

        #region Constructors

        private BannedIp()
        {
            // This is needed by EntityFramework
            IpAddress = String.Empty;
            BanReason = String.Empty;
        }

        public BannedIp(string ip, string banReason)
        {
            IpAddress = ip;
            BanReason = banReason;
        }

        #endregion

        #region Persistence

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
            if (bannedIpsFromDb.Count == 0)
            {
                return null;
            }
            return bannedIpsFromDb.First();
        }

        public static List<BannedIp> GetAllBannedIps()
        {
            return context.BannedIps.ToList();
        }

        public static bool AlreadyExists(string ip)
        {
            List<BannedIp> ipsFromDb = context.BannedIps.Where(m => m.IpAddress == ip).ToList();
            return ipsFromDb.Count > 0;
        }

        public static string BannedIpListToString(List<BannedIp> bannedIps)
        {
            string arrString = "/*$*/";

            foreach (BannedIp bannedIp in bannedIps)
            {
                arrString += "\r\n";
                arrString += bannedIp.IpAddress;
                arrString += "\r\n";
                arrString += bannedIp.BanReason;
                arrString += "\r\n" + "/*$*/";
            }
            return arrString;
        }

        #endregion
    }
}