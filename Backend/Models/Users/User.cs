using Backend.Database;
using Backend.ServerModules;
using Backend.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using WebSocketSharp;

namespace Backend.Models.Users
{
    public class User : IEncodable
    {
        // Fields
        public int UserId { get; set; }

        private WebSocket _socket;
        private string _ip;
        private string _username = "";
        private bool _isRegistered = false;
        private bool _isMuted = false;
        private bool _isBanned = false;
        private string _muteReason = "";
        private string _banReason = "";

        //Properties
        [NotMapped]
        public WebSocket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        [NotMapped]
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        [NotMapped]
        public bool IsRegistered
        {
            get { return _isRegistered; }
            set { _isRegistered = value; }
        }

        public bool IsMuted
        {
            get { return _isMuted; }
            set { _isMuted = value; }
        }

        public bool IsBanned
        {
            get { return _isBanned; }
            set { _isBanned = value; }
        }

        public string MuteReason
        {
            get { return _muteReason; }
            set { _muteReason = value; }
        }

        public string BanReason
        {
            get { return _banReason; }
            set { _banReason = value; }
        }

        // Constructors
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

        // Methods
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

        public static User? GetUserFromDB(string username)
        {
            List<User> usersFromDB = context.Users.Where(m => m.Username.ToLower() == username.ToLower()).ToList();
            if (usersFromDB.Count == 0) return null;
            return usersFromDB.First();
        }
    }
}
