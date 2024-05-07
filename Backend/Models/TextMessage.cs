using Backend.Utils;

namespace BackendServer.Models
{
    public class TextMessage : Message
    {
        public int TextMessageId { get; set; }

        private string _sender;
        public string Sender
        {
            get { return _sender; }
        }

        private string _channel;
        public string Channel
        {
            get { return _channel; }
        }

        private DateTime _timestamp;
        public DateTime TimeStamp
        {
            get { return _timestamp; }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
        }

        private TextMessage ()
        {
            _sender = String.Empty;
            _channel = String.Empty;
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

        public static TextMessage[] GetMessageHistory(string channel, int from, int to)
        {
            // TODO: Implement fetching message history from db
            TextMessage test = new TextMessage("DO SEND\r\nWITH\r\nTEST");
            return [test];
        }
    }
}
