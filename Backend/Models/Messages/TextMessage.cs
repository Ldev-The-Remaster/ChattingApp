using Backend.Database;
using Backend.Interfaces;
using Backend.Models.Users;
using Backend.ServerModules;
using WebSocketSharp;

namespace Backend.Models.Messages
{
    public class TextMessage : Message, IEncodable
    {
        #region Fields

        public int TextMessageId { get; set; }

        private string _sender;
        private string _channel;
        private long _timestamp;
        private string _hash;
        private string _content;

        #endregion

        #region Properties

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public long TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        public string Hash
        {
            get { return _hash; }
            set { _hash = value; }
        }

        #endregion

        #region Constructors

        private TextMessage()
        {
            // This is needed by EntityFramework
            _sender = string.Empty;
            _channel = string.Empty;
            _timestamp = 0;
            _hash = string.Empty;
            _content = string.Empty;
        }

        public TextMessage(WebSocket socket, string rawString) : base(socket, rawString)
        {
            _sender = UserManager.GetUsernameBySocket(socket);

            _channel = "general-chat";
            if (_in != string.Empty)
            {
                _channel = _in;
            }

            _timestamp = _at;
            (_hash, _content) = LSMPBehavior.GetHashAndMessage(_with);
        }

        #endregion

        #region Methods

        public string EncodeToString()
        {
            return LSMPBehavior.EncodeTextMessageToString(this);
        }

        #endregion

        #region Persistence

        private static TextMessageContext context = new TextMessageContext();

        public void SaveToDb()
        {
            context.Add(this);
            context.SaveChanges();
        }

        public static List<TextMessage> GetMessageHistory(string channel, int from, int to)
        {
            List<TextMessage> messageHistory = context.TextMessages
                .Where(m => m.TextMessageId >= from && m.TextMessageId < to && m.Channel == channel)
                .ToList();

            return messageHistory;
        }

        #endregion
    }
}
