
using Backend.Interfaces;
using Backend.Models.Messages;
using Backend.Models.Users;
using Backend.Utils;
using WebSocketSharp.Server;

namespace Backend.ServerModules
{
    public class LSMPBehavior : WebSocketBehavior
    {
        #region Fields

        private static readonly string NEW_LINE = "\r\n";

        #endregion

        #region Senders

        protected void SendAccept()
        {
            Send("DO ACCEPT");
        }

        protected void SendRefuse(string reason = "") 
        {
            string message = $"DO REFUSE{NEW_LINE}WITH{NEW_LINE}{reason}";
            Send(message);
        }

        protected void SendToAll(IEncodable message)
        {
            foreach (User client in UserManager.UsersList)
            {
                if (!client.IsRegistered)
                {
                    continue;
                }

                client.Socket.Send(message.EncodeToString());
            }
        }

        protected void SendAlert(string message)
        {
            AlertMessage alert = new AlertMessage( message);

            SendToAll(alert);
        }

        protected void SendUserList()
        {
            string encodedUserList = EncodeArrayToString(UserManager.UsersList);

            string msg = $"DO INTRODUCE{NEW_LINE}WITH{NEW_LINE}{encodedUserList}";
            Send(msg);
        }

        protected void SendUserListToAll()
        {
            string encodedUserList = EncodeArrayToString(UserManager.UsersList);

            string msg = $"DO INTRODUCE{NEW_LINE}WITH{NEW_LINE}{encodedUserList}";

            foreach (User client in UserManager.UsersList)
            {
                if (!client.IsRegistered)
                {
                    continue;
                }

                client.Socket.Send(msg);
            }
        }

        #endregion

        #region Methods

        protected bool IsAuthRequest(string message)
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

        public static string EncodeAlertMessageToString(AlertMessage message)
        {
            string msgString = string.Empty;
            if (message.Content == string.Empty)
            {
                return msgString;
            }

            msgString += "DO SEND";
            msgString += NEW_LINE;
            msgString += $"WITH";
            msgString += NEW_LINE;
            msgString += message.Content;

            return msgString;
        }

        public static string EncodeTextMessageToString(TextMessage message)
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

            if (message.Hash != string.Empty && message.Content != string.Empty)
            {
                msgString += $"WITH";
                msgString += NEW_LINE;
                msgString += message.Hash;
                msgString += NEW_LINE;
                msgString += message.Content;
            }

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

        #endregion
    }
}
