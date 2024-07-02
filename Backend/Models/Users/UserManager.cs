using LSMP.Utils;
using WebSocketSharp;

namespace Backend.Models.Users
{
    public static class UserManager
    {
        #region Fields

        public static List<User> UsersList { get; set; } = new List<User>();

        #endregion

        #region Infraction Methods

        public static void Mute(User user, string reason = "")
        {
            user.IsMuted = true;
            user.MuteReason = reason;
            user.UpdateToDB();
        }

        public static void Unmute(User user)
        {
            user.IsMuted = false;
            user.MuteReason = "";
            user.UpdateToDB();
        }
       
        public static void Kick(User user) 
        {
            Disconnect(user);
        }

        public static void Ban(User user, string reason = "")
        {
            Disconnect(user);
            user.IsBanned = true;
            user.BanReason = reason;
            user.UpdateToDB();
        }
        public static void Unban(User user) 
        {
            user.IsBanned = false;
            user.BanReason = "";
            user.UpdateToDB();
        }

        public static void BanIp(string ip, string reason = "")
        { 
            BannedIp ipToBan = new BannedIp(ip,reason);
            ipToBan.SaveToDb();
            UsersList.Where(user => user.Ip == ip).ToList().ForEach(user => Disconnect(user));
        }

        public static void UnbanIp(string ip)
        {
            BannedIp? bannedIp = BannedIp.GetBannedIpFromDb(ip);
            if (bannedIp == null)
            {
                CLogger.Error("IP not found.");
                return;
            }
            bannedIp.RemoveFromDb();
        }

        #endregion

        #region User Management

        public static User Connect(WebSocket socket, string ip)
        {
            User newUser = new User(socket, ip);
            UsersList.Add(newUser);
            return newUser;
        }

        public static bool InitializeUser(WebSocket socket, string username)
        {
            User? user = GetUserBySocket(socket);
            if (user == null)
            {
                CLogger.Error("Authentication failed: User client not found");
                return false;
            }

            bool isUsernameConnected = GetUserByUsername(username) != null;
            if (isUsernameConnected)
            {
                return false;
            }

            User? userInDb = User.GetUserFromDB(username);
            if (userInDb == null)
            {
                user.Username = username;
                user.SaveToDb();
            }
            else
            {
                user.Username = userInDb.Username;
                user.IsMuted = userInDb.IsMuted;
                user.IsBanned = userInDb.IsBanned;
            }


            user.IsRegistered = true;
            CLogger.Event($"User authenticated with username: {username}");
            return true;
        }

        public static bool IsUserAdmin(User user) 
        {
            return user.Ip == "127.0.0.1";
        }


        public static void Disconnect(User user) 
        {
            user.Socket.Close();
            UsersList.Remove(user);
        }

        #endregion

        #region Getters

        public static User? GetUserByUsername(string username) 
        {
            return UsersList.Find(user => user.Username.ToLower() == username.ToLower());
        }

        public static User? GetUserBySocket(WebSocket socket)
        {
            return UsersList.Find(user => user.Socket == socket);
        }

        public static string GetUsernameBySocket(WebSocket socket)
        {
            return GetUserBySocket(socket)?.Username ?? String.Empty;
        }

        #endregion
    }
}
