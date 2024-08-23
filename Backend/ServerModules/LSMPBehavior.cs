
using Backend.Models.Messages;
using Backend.Models.Users;
using WebSocketSharp.Server;
using LSMP;

namespace Backend.ServerModules
{
    public class LSMPBehavior : WebSocketBehavior
    {
        #region Senders

        protected void SendAccept()
        {
            Send(Messaging.AcceptMessage());
        }

        protected void SendRefuse(string reason = "") 
        {
            Send(Messaging.RefuseMessage(reason));
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
            AlertMessage alert = new AlertMessage(message);

            SendToAll(alert);
        }

        protected void SendUserListToAll(string channelName = "general-chat")
        {
            string msg = Messaging.UserListMessage(UserManager.UsersList, channelName);

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
    }
}
