namespace BackendServer.Models
{
    internal abstract class Message
    {
        public enum MessageType
        {
            TextMessage,
            CommandMessage
        }

        public static MessageType GetMessageType(string rawString)
        {
            string[] messageParams = rawString.Split("\r\n");
            string doLine = messageParams[0];
            string[] doParams = doLine.Split(" ");
            string doArgument = doParams[1];
            if (doArgument.Equals("SEND"))
            {
                return MessageType.TextMessage;
            }
            return MessageType.CommandMessage;
        }

        struct MessageParams
        {
            public string Do;
            public string From;
            public string To;
            public string In;
            public DateTime At;
            public string With;
            public MessageParams(string rawString)
            {
                // TODO: Parse requestString here (akram kaif tmam?)
                Do = "TEST";
                From = String.Empty;
                To = String.Empty;
                In = String.Empty;
                At = DateTime.Now;
                With = String.Empty;
            }
        }

        protected string _do;
        protected string _from;
        protected string _to;
        protected string _in;
        protected DateTime _at;
        protected string _with;

        public Message(string rawString)
        {
            MessageParams messageParams = new MessageParams(rawString);

            _do = messageParams.Do;
            _from = messageParams.From;
            _to = messageParams.To;
            _in = messageParams.In;
            _at = messageParams.At;
            _with = messageParams.With;
        }
    }
}
