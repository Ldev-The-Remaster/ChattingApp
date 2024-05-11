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
            string doArgument = rawString.Substring(3, 4); // "DO SEND"
            if (doArgument.Equals("SEND"))
            {
                return MessageType.TextMessage;
            }
            return MessageType.CommandMessage;
        }

        struct MessageParams
        {
            public string Do = "";
            public string From = "";
            public string To = "";
            public string In = "";
            public DateTime At = DateTime.Now;
            public string With = "";

            public MessageParams(string rawString)
            {   
                string[] commandPairs = rawString.Split("\r\n");
                foreach(string line in commandPairs) 
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
                            long unixTimestamp = long.Parse(commandPair[1]);
                            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
                            DateTime dateTime = dateTimeOffset.UtcDateTime;
                            At = dateTime;
                            break;
                        case "WITH":
                            With = rawString.Split("WITH\r\n",2)[1];
                            break;
                    }
                }
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
