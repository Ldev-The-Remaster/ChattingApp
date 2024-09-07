using Backend.Database;
using LSMP.Utils;
using LSMP;
using System.ComponentModel.DataAnnotations.Schema;
using WebSocketSharp;

namespace Backend.Models.Users
{
    public class User : IEncodable
    {
        #region Fields

        public int UserId { get; set; }

        private WebSocket _socket;
        private string _ip;
        private string _username = "";
        private bool _isRegistered = false;
        private bool _isMuted = false;
        private bool _isBanned = false;
        private string _muteReason = "";
        private string _banReason = "";

        #endregion

        #region Properties

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

        #endregion

        #region Constructors

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

        #endregion

        #region Methods

        public string EncodeToString()
        {
            return Username;
        }
        #endregion

        #region Persistence

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

        public static List<User> GetBannedUsersFromDB()
        {
            List<User> usersFromDB = context.Users.Where(m => m.IsBanned).ToList();
            return usersFromDB;
        }

        public static string UserListToString(List<User> users)
        {
            string arrString = "/*$*/";

            foreach (User user in users)
            {
                arrString += user.Username;
                arrString += "\r\n";
                arrString += user.BanReason;
                arrString += "/*$*/";
            }
            return arrString;
        }

        #endregion
    }
}
