using Backend.Database;

namespace Backend.Models
{
    public class TextMessage : Message
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
            _sender = String.Empty;
            _channel = String.Empty;
            _timestamp = 0;
            _content = String.Empty;
        }

        public TextMessage(string rawString) : base(rawString)
        {
            _sender = _from;
            _channel = _in;
            _timestamp = _at;
            _content = _with;
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
                .Where(m => m.TextMessageId < from && m.TextMessageId >= to)
                .ToList();

            return messageHistory;
        }
    }
}
