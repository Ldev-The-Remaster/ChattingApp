using WebSocketSharp;

namespace Backend.Models.Users
{
    public class User(WebSocket socket, string ip)
    {
        private WebSocket _socket = socket;
        public WebSocket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        private string _ip = ip;
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
    }
}
