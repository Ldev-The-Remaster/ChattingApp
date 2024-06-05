using Backend.Models.Messages;
using Backend.Models.Users;
using Backend.Utils;
using WebSocketSharp;
using static Backend.Models.Messages.Message;

namespace Backend.ServerModules
{
    public class ServerBehavior : LSMPBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            string rawString = e.Data;
            WebSocket socket = Context.WebSocket;
            User? user = UserManager.GetUserBySocket(socket);

            if (user == null)
            {
                SendRefuse("Invalid connection, please reconnect");
                CLogger.Error($"Invalid connection from: {Context.UserEndPoint.Address}");
                return;
            }

            if (!user.IsRegistered)
            {
                if (IsAuthRequest(rawString))
                {
                    string username = rawString.Substring(14);
                    UserManager.Authenticate(user.Socket, username);
                    SendAccept();
                }
                else
                {
                    SendRefuse("You must authenticate first by sending AUTH verb WITH a unique username");
                    CLogger.Error($"Failed send attempt from unregistered user at: {user.Ip}");
                }
                return;
                
            }

            switch(Message.GetMessageType(rawString))
            {
                case MessageType.TextMessage:
                    if (user.IsMuted)
                    {
                        SendRefuse("You are muted!");
                        CLogger.Error($"Failed send attempt from muted user: {user.Username}");
                        return;
                    }

                    var textMessage = new TextMessage(socket, rawString);

                    CLogger.Chat(textMessage.Sender, textMessage.Content);
                    SendToAll(textMessage);
                    break;
                case MessageType.CommandMessage:
                    var commandMessage = new CommandMessage(socket, rawString);
                    commandMessage.InvokeCommand();
                    break;
            }
        }

        protected override void OnOpen()
        {
            WebSocket socket = Context.WebSocket;
            string ip = Context.UserEndPoint.Address.ToString();

            UserManager.Connect(socket, ip);
            CLogger.Event($"New client connected from: {ip}");
        }
    }
}
