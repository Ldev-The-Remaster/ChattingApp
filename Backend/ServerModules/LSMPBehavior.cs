
using Backend.Models.Messages;
using Backend.Models.Users;
using WebSocketSharp.Server;

namespace Backend.ServerModules
{
    public class LSMPBehavior : WebSocketBehavior
    {
        private static readonly string NEW_LINE = "\r\n";
        protected void SendAccept()
        {
            Send("DO ACCEPT");
        }

        protected void SendRefuse(string reason = "") 
        {
            string message = $"DO REFUSE{NEW_LINE}WITH{NEW_LINE}{reason}";
            Send(message);
        }

        protected void SendToAll(TextMessage message)
        {
            foreach (User client in UserManager.UsersList)
            {
                if (!client.IsRegistered)
                {
                    continue;
                }

                client.Socket.Send(message.ToString());
            }
        }

        protected void SendAlert(string message)
        {
            TextMessage alert = new TextMessage(null, "");
            alert.Content = message;

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

                client.Socket.Send(msg.ToString());
            }
        }

        protected bool IsAuthRequest(string message)
        {
            return (message.Substring(3, 4).ToLower() == "auth");
        }

        public static string EncodeArrayToString<T>(List<T> array)
        {
            string arrString = "/*$*/";

            foreach (T item in array)
            {
                if (item == null)
                {
                    continue;
                }
                arrString += NEW_LINE;
                arrString += item.ToString();
                arrString += NEW_LINE + "/*$*/";
            }

            return arrString;
        }
    }
}
