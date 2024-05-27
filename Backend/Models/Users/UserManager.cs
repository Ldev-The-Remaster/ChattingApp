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
                Console.WriteLine("Invalid authentication attempt");
                return false;
            }

            if (user.IsRegistered)
            {
                Console.WriteLine($"Double registration prevented from: {user.Username}");
                return false;
            }

            bool isUserConnected = GetUserByUsername(username) != null;
            if (isUserConnected)
            {
                return false;
            }

            user.Username = username;
            user.IsRegistered = true;
            user.SaveToDb();

            Console.WriteLine($"User authenticated with username: {username}");
            return true;
        }

        public static void Disconnect(User user) { }

        public static User? GetUserBySocket(WebSocket socket)
        {
            return UsersList.Find(user => user.Socket == socket);
        }
    }
}
