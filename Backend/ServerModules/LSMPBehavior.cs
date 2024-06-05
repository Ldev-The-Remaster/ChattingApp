
using Backend.Models.Messages;
using Backend.Models.Users;
using WebSocketSharp.Server;

namespace Backend.ServerModules
{
    public class LSMPBehavior: WebSocketBehavior
    {
        private readonly string NEW_LINE = "\r\n";
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

        protected bool IsAuthRequest(string message)
        {
            return (message.Substring(3, 4).ToLower() == "auth");
        }
    }
}
