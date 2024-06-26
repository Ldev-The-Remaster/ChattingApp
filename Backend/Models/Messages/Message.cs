﻿using Backend.ServerModules;
using WebSocketSharp;

namespace Backend.Models.Messages
{
    struct MessageParams
    {
        public string Do = "";
        public string From = "";
        public string To = "";
        public string In = "";
        public long At = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string With = "";

        public MessageParams(string rawString)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine($"Error Parsing: {e.Message}");
            }
        }
    }

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
            MessageParams messageParams = new MessageParams(rawString);

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
