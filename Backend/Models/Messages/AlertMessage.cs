using LSMP;

namespace Backend.Models.Messages
{
    public class AlertMessage : IEncodable, IMessage
    {
        #region Fields

        private string _content;
        private long _timestamp;

        #endregion

        #region Properties

        public string Sender { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public long TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        public string Hash
        {
            get { return "AlertMessageHash"; }
        }
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        #endregion

        #region Constructors

        public AlertMessage(string content) 
        {
            _content = content;
            _timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        #endregion

        #region Methods

        public string EncodeToString()
        {
            return Messaging.EncodeMessageToString(this);
        }

        #endregion
    }
}
