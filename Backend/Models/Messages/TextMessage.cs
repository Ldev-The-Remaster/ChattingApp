using Backend.Database;
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

        #endregion

        #region Constructors

        private TextMessage()
        {
            // This is needed by EntityFramework
            _sender = string.Empty;
            _channel = string.Empty;
            _timestamp = 0;
            _content = string.Empty;
        }

        public TextMessage(WebSocket? socket, string rawString) : base(socket, rawString)
        {
            _sender = string.Empty;
            if (socket != null)
            {
                _sender = UserManager.GetUsernameBySocket(socket);
            }

            _channel = "general-chat";
            if (_in != string.Empty)
            {
                _channel = _in;
            }

            _timestamp = _at;
            _content = _with;
        }

        #endregion

        #region Methods

        public string EncodeToString()
        {
            string msgString = "DO SEND\r\n";

            if (_sender != String.Empty)
            {
                msgString += $"FROM {_sender}\r\n"; 
            }

            if (_channel != String.Empty)
            {
                msgString += $"IN {_channel}\r\n";
            }

            if (_timestamp != 0)
            {
                msgString += $"AT {_timestamp}\r\n";
            }

            if (_content != String.Empty)
            {
                msgString += $"WITH\r\n{_content}";
            }

            return msgString;
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
