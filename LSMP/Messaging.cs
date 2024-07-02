
using LSMP.Utils;

namespace LSMP
{
    public static class Messaging
    {
        private static readonly string NEW_LINE = "\r\n";

        public static string AcceptMessage()
        {
            return "DO ACCEPT";
        }

        public static string RefuseMessage(string reason = "")
        {
            string message = $"DO REFUSE";
            message += NEW_LINE;
            message += "WITH";
            message += NEW_LINE;
            message += reason;

            return message;
        }

        public static string UserListMessage<T>(List<T> array) where T : IEncodable
        {
            string msg = $"DO INTRODUCE";
            msg += NEW_LINE;
            msg += "WITH";
            msg += NEW_LINE;
            msg += EncodeArrayToString(array);

            return msg;
        }

        public static bool IsAuthRequest(string message)
        {
            return (message.Substring(3, 4).ToLower() == "auth");
        }

        public static string EncodeArrayToString<T>(List<T> array) where T : IEncodable
        {
            string arrString = "/*$*/";

            foreach (T item in array)
            {
                if (item == null)
                {
                    continue;
                }
                arrString += NEW_LINE;
                arrString += item.EncodeToString();
                arrString += NEW_LINE + "/*$*/";
            }

            return arrString;
        }

        public static string EncodeMessageToString(IMessage message)
        {
            string msgString = "DO SEND";
            msgString += NEW_LINE;

            if (message.Sender != string.Empty)
            {
                msgString += $"FROM {message.Sender}";
                msgString += NEW_LINE;
            }

            if (message.Channel != string.Empty)
            {
                msgString += $"IN {message.Channel}";
                msgString += NEW_LINE;
            }

            if (message.TimeStamp != 0)
            {
                msgString += $"AT {message.TimeStamp}";
                msgString += NEW_LINE;
            }

            if (message.Hash == string.Empty || message.Content == string.Empty)
            {
                return string.Empty;
            }

            msgString += $"WITH";
            msgString += NEW_LINE;
            msgString += message.Hash;
            msgString += NEW_LINE;
            msgString += message.Content;

            return msgString;
        }

        public static (string, string) GetHashAndMessage(string withPayload)
        {
            var splitList = withPayload.Split("\r\n");
            if (splitList.Length != 2)
            {
                CLogger.Error("Error: Missing message hash!");
                return (string.Empty, withPayload);
            }

            var hash = splitList[0];
            var content = splitList[1];
            return (hash, content);
        }
    }
}