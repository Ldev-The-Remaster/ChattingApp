using Backend.Utils;
using WebSocketSharp;

namespace Backend.Models.Users
{
    public static class UserManager
    {
        public static List<User> UsersList { get; set; } = new List<User>();

        // Infractions
        public static void Mute(User user) { }
        public static void Unmute(User user) { }
        public static void Kick(User user) { }
        public static void Ban(User user) { }
        public static void Unban(User user) { }

        public static void BanIp(string ip)
        { 
            BannedIp ipToBan = new BannedIp(ip);
            if (ipToBan.AlreadyExists())
            {
                CLogger.Error($"Attempt to ban IP {ip} failed: IP already banned");
                return;
            }

            ipToBan.SaveToDb();
            CLogger.Event($"IP {ip} was banned successfully");
        }

        public static void UnbanIp(string ip) { }

        // Connection
        public static User Connect(WebSocket socket, string ip)
        {
            User newUser = new User(socket, ip);
            UsersList.Add(newUser);
            return newUser;
        }

        public static bool IsUserAdmin(User user) 
        {
            return user.Ip == "127.0.0.1";
        }

        public static User? GetUserByUsername(string username) 
        {
            return UsersList.Find(user => user.Username == username);
        }

        public static bool Authenticate(WebSocket socket, string username)
        {
            User? user = GetUserBySocket(socket);
            if (user == null)
            {
                CLogger.Error("Authentication failed: User client not found");
                return false;
            }

            if (user.IsRegistered)
            {
                CLogger.Warn($"Double registration prevented from: {user.Username}");
                return false;
            }

            bool isUsernameConnected = GetUserByUsername(username) != null;
            if (isUsernameConnected)
            {
                return false;
            }

            user.Username = username;
            user.IsRegistered = true;
            user.SaveToDb();

            CLogger.Event($"User authenticated with username: {username}");
            return true;
        }

        public static void Disconnect(User user) { }

        public static User? GetUserBySocket(WebSocket socket)
        {
            return UsersList.Find(user => user.Socket == socket);
        }

        public static string GetUsernameBySocket(WebSocket socket)
        {
            return GetUserBySocket(socket)?.Username ?? String.Empty;
        }
    }
}
