using LSMP;
using static Frontend.Client.Pages.Settings;

namespace Frontend.Client.Models
{
    public static class ClientManager
    {
        public static Dictionary<string, string> dmChannels = new Dictionary<string, string>();
        private static readonly List<string> _users = new List<string>();
        public static event Action? OnUserListUpdate;
        public static bool IsAdmin { get; set; } = false;
        public static string CurrentUser { get; set; } = string.Empty;

        public static List<UserBan> userBans = new List<UserBan>();
        public static List<IpBan> ipBans = new List<IpBan>();

        public static event Action? OnBanListUpdate;

        public static void UpdateBanList()
        {
            OnBanListUpdate?.Invoke();
        }

        public static void AddUserBan(string username, string reason)
        {
            userBans.Add(new UserBan { Username = username, Reason = reason });
        }

        public static void AddIpBan(string ipAddress, string reason)
        {
            ipBans.Add(new IpBan { IpAddress = ipAddress, Reason = reason });
        }

    }
}
