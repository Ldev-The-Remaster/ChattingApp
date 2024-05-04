namespace BackendServer.Models
{
    internal class TextMessage : Message
    {
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

        public TextMessage(string rawString) : base(rawString)
        {
            _sender = _from;
            _channel = _in;
            _timestamp = _at;
            _content = _with;
        }
    }
}
