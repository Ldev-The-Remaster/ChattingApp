using Backend.Database;
using Backend.ServerModules;
using Backend.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using WebSocketSharp;

namespace Backend.Models.Users
{
    public class User : IEncodable
    {
        public int UserId { get; set; }

        private WebSocket _socket;
        [NotMapped]
        public WebSocket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        private string _ip;
        [NotMapped]
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        private string _username = "";
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private bool _isRegistered = false;
        [NotMapped]
        public bool IsRegistered
        {
            get { return _isRegistered; }
            set { _isRegistered = value; }
        }

        private bool _isMuted = false;
        public bool IsMuted
        {
            get { return _isMuted; }
            set { _isMuted = value; }
        }

        private bool _isBanned = false;
        public bool IsBanned
        {
            get { return _isBanned; }
            set { _isBanned = value; }
        }

        private string _muteReason = "";
        public string MuteReason
        {
            get { return _muteReason; }
            set { _muteReason = value; }
        }

        private string _banReason = "";
        public string BanReason
        {
            get { return _banReason; }
            set { _banReason = value; }
        }

        private User()
        {
            // This is needed by EntityFramework
            _socket = new WebSocket("ws://localhost");
            _ip = String.Empty;
        }

        public User(WebSocket socket, string ip)
        {
            _socket = socket;
            _ip = ip;
        }

        public string EncodeToString()
        {
            return Username;
        }

        // Persistence
        private static UserContext context = new UserContext();

        public void SaveToDb()
        {
            context.Add(this);
            context.SaveChanges();
        }

        public static User? GetUserFromDB(string username)
        {
            List<User> usersFromDB = context.Users.Where(m => m.Username.ToLower() == username.ToLower()).ToList();
            if (usersFromDB.Count == 0) return null;
            return usersFromDB.First();
        }

        public void UpdateToDB()
        {
            User? userFromDB = GetUserFromDB(Username);
            if (userFromDB == null) {
                CLogger.Error("User not found.");
                return;
            }
            userFromDB = this;
            context.SaveChanges();
        }
    }
}
