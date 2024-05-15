namespace Backend.Models.Messages
{
    struct MessageParams
    {
        public string Do = "";
        public string From = "";
        public string To = "";
        public string In = "";
        public long At = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        public string With = "";
        public MessageParams(string rawString)
        {
            string[] commandPairs = rawString.Split("\r\n");
            foreach (string line in commandPairs)
            {
                string[] commandPair = line.Split(' ');
                switch (commandPair[0])
                {
                    case "DO":
                        Do = commandPair[1];
                        break;
                    case "FROM":
                        From = commandPair[1];
                        break;
                    case "TO":
                        To = commandPair[1];
                        break;
                    case "IN":
                        In = commandPair[1];
                        break;
                    case "AT":
                        At = long.Parse(commandPair[1]);
                        break;
                    case "WITH":
                        With = rawString.Split("WITH\r\n", 2)[1];
                        break;
                }
            }
        }
    }

    public abstract class Message
    {
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

        protected Message()
        {
            _do = string.Empty;
            _from = string.Empty;
            _to = string.Empty;
            _in = string.Empty;
            _at = 0;
            _with = string.Empty;
        }

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

        public static MessageType GetMessageType(string rawString)
        {
            string doArgument = rawString.Substring(3, 4); // "DO SEND"
            if (doArgument.Equals("SEND"))
            {
                return MessageType.TextMessage;
            }
            return MessageType.CommandMessage;
        }
    }
}
