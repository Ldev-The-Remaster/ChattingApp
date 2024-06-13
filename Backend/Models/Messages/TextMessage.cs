using Backend.Database;
using Backend.Models.Users;
using Backend.ServerModules;
using WebSocketSharp;

namespace Backend.Models.Messages
{
    public class TextMessage : Message, IEncodable
    {
        public int TextMessageId { get; set; }

        private string _sender;
        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        private string _channel;
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        private long _timestamp;
        public long TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

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

        // Persistence
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
    }
}
