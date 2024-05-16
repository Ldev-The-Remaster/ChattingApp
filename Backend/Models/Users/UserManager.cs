using System.Security.Cryptography;
using WebSocketSharp;

namespace Backend.Models.Users
{
    public static class UserManager
    {
        public static User[] UsersList { get; set; } = [];

        // Infractions
        public static void Mute(User user) { }
        public static void Unmute(User user) { }
        public static void Kick(User user) { }
        public static void Ban(User user) { }
        public static void Unban(User user) { }
        public static void BanIp(string ip) { }
        public static void UnbanIp(string ip) { }

        // Connection
        public static User Connect(WebSocket socket, string ip) { return new User(socket, ip); }
        public static void Authenticate(WebSocket socket, string username) { }
        public static void Disconnect(User user) { }

        private static User? GetUserBySocket(WebSocket socket) { return null; }
    }
}
