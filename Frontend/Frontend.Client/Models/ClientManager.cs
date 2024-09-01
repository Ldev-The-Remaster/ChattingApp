using LSMP;

namespace Frontend.Client.Models
{
    public static class ClientManager
    {
        public static Dictionary<string, string> dmChannels = new Dictionary<string, string>();
        private static readonly List<string> _users = new List<string>();
        public static event Action? OnUserListUpdate;
        public static bool IsAdmin { get; set; } = false;
        public static string CurrentUser { get; set; } = string.Empty;
    }
}
