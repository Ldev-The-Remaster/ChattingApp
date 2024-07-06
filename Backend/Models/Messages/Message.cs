using Backend.ServerModules;
using WebSocketSharp;

using LSMP;
namespace Backend.Models.Messages
{
    public abstract class Message
    {
        #region Fields

        public enum MessageType
        {
            TextMessage,
            CommandMessage
        }

        protected string _do;
        protected string _from;
        protected string _to;
        protected string _in;
        protected long _at;
        protected string _with;
        protected WebSocket? _socket;

        #endregion

        #region Constructors

        protected Message()
        {
            // This is needed by EntityFramework
            _do = string.Empty;
            _from = string.Empty;
            _to = string.Empty;
            _in = string.Empty;
            _at = 0;
            _with = string.Empty;
            _socket = null;
        }

        public Message(WebSocket? socket, string rawString)
        {
            MessageParser messageParams = new MessageParser(rawString);

            _do = messageParams.Do;
            _from = messageParams.From;
            _to = messageParams.To;
            _in = messageParams.In;
            _at = messageParams.At;
            _with = messageParams.With;
            _socket = socket;
        }

        #endregion

        #region Methods

        public static MessageType GetMessageType(string rawString)
        {
            string doArgument = rawString.Substring(3, 4); // "DO SEND"
            if (doArgument.Equals("SEND"))
            {
                return MessageType.TextMessage;
            }
            return MessageType.CommandMessage;
        }

        #endregion
    }
}
