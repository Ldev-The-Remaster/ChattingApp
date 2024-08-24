
using LSMP.Utils;
using System.Security.Cryptography;
using System.Text;

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

        public static string UserListMessage<T>(List<T> userList, string channel) where T : IEncodable
        {
            string msg = $"DO INTRODUCE";
            msg += NEW_LINE;
            msg += $"IN {channel}";
            msg += NEW_LINE;
            msg += "WITH";
            msg += NEW_LINE;
            msg += EncodeArrayToString(userList);

            return msg;
        }

        public static string RequestMessageHistory(string channel, int from, int to)
        {
            string msg = "DO REMEMBER";
            msg += NEW_LINE;
            msg += $"FROM {from}" ;
            msg += NEW_LINE;
            msg += $"TO {to}";
            msg += NEW_LINE;
            msg += "IN " + channel;

            return msg;
        }

        public static string RemindMessage<T>(List<T> messageHistory, string channel) where T : IEncodable
        {
            string msg = "DO REMIND";
            msg += NEW_LINE;
            msg += $"IN {channel}";
            msg += NEW_LINE;
            msg += "WITH";
            msg += NEW_LINE;
            msg += EncodeArrayToString(messageHistory);

            return msg;
        }

        public static string MuteMessage(string targetUser, string reason)
        {
            string msg = "DO MUTE";
            msg += NEW_LINE;
            msg += "TO " + targetUser;
            msg += NEW_LINE;
            msg += "WITH";
            msg += NEW_LINE;
            msg += reason;
            return msg;
        }

        public static string BanMessage(string targetUser, string reason)
        {
            string msg = "DO BAN";
            msg += NEW_LINE;
            msg += "TO " + targetUser;
            msg += NEW_LINE;
            msg += "WITH";
            msg += NEW_LINE;
            msg += reason;
            return msg;
        }

        public static string KickMessage(string targetUser, string reason)
        {
            string msg = "DO KICK";
            msg += NEW_LINE;
            msg += "TO " + targetUser;
            msg += NEW_LINE;
            msg += "WITH";
            msg += NEW_LINE;
            msg += reason;
            return msg;
        }

        public static string BanIpMessage(string targetUser, string reason)
        {
            string msg = "DO BANIP";
            msg += NEW_LINE;
            msg += "TO " + targetUser;
            msg += NEW_LINE;
            msg += "WITH";
            msg += NEW_LINE;
            msg += reason;
            return msg;
        }

        public static string AuthMessage(string username)
        {
            string msg = "DO AUTH";
            msg += NEW_LINE;
            msg += $"FROM {username}";

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

                arrString += item.EncodeToString();
                arrString += "/*$*/";
            }

            return arrString;
        }

        public static List<string> DecodeUserList(string rawUserList)
        {
            return rawUserList.Split(new[] { "/*$*/", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static List<string> DecodeMessageHistory(string rawMessageHistory)
        {
            return rawMessageHistory.Split(new[] { "/*$*/" },StringSplitOptions.RemoveEmptyEntries).ToList();
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

        public static string GetDirectMessageChannelHash(string from, string to)
        {
            string[] users = { from, to };
            Array.Sort(users);

            string combinedString = users[0] + users[1];

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
                return $"dm-{BitConverter.ToString(hashBytes).ToLower()}";
            }
        }
    }
}