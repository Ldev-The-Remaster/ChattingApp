using Backend.Interfaces;
using Backend.ServerModules;

namespace Backend.Models.Messages
{
    public class AlertMessage : IEncodable
    {
        #region Fields

        private string _hash = "AlertMessageHash";
        private string _content;
        private long _timestamp;

        #endregion

        #region Properties

        public string Hash
        {
            get { return _hash; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public long TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
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
            return LSMPBehavior.EncodeAlertMessageToString(this);
        }

        #endregion
    }
}
