using LSMP;

namespace Frontend.Client.Models
{
    public static class ClientManager
    {
        private static readonly List<string> _users = new List<string>();
        public static event Action? OnUserListUpdate;
        public static bool IsAdmin { get; set; } = false;
        public static List<string> CurrentUsersList
            { get { return _users; } }
        public static string CurrentUser { get; set; } = string.Empty;

        public static void UpdateUserList(string rawUserList)
        {
            var usersList = Messaging.DecodeUserList(rawUserList);

            _users.Clear();
            _users.AddRange(usersList);

            OnUserListUpdate?.Invoke();
        }
    }
}
