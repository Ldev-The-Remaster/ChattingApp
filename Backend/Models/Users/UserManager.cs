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
                Console.WriteLine($"Attempt to ban IP {ip} failed: IP already banned");
                return;
            }
            ipToBan.SaveToDb();
            Console.WriteLine($"IP {ip} was banned successfully");
        }

        public static void UnbanIp(string ip) { }

        // Connection
        public static User Connect(WebSocket socket, string ip)
        {
            User newUser = new User(socket, ip);
            newUser.SaveToDb();
            UsersList.Add(newUser);
            return newUser;
        }

        public static void Authenticate(WebSocket socket, string username) { }
        public static void Disconnect(User user) { }
        private static User? GetUserBySocket(WebSocket socket) { return null; }
    }
}
