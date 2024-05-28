using Backend.Models.Messages;
using Backend.Models.Users;
using WebSocketSharp;
using WebSocketSharp.Server;
using static Backend.Models.Messages.Message;

namespace Backend.ServerModules
{
    public class ServerBehavior : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            string rawString= e.Data;
            WebSocket currentSocket = Context.WebSocket;
            User? user = UserManager.GetUserBySocket(currentSocket);

            if (user == null)
            {
                Send("DO REFUSE\r\nWITH\r\nInvalid connection, please reconnect");
                Console.WriteLine($"Invalid connection from: {Context.UserEndPoint.Address}");
                return;
            }

            if (!user.IsRegistered && !Authenticate(user, rawString))
            {
                Send("DO REFUSE\r\nWITH\r\nYou must authenticate first by sending AUTH verb WITH username");
                Console.WriteLine($"Failed send attempt from unregistered user at: {user.Ip}");
                return;
            }

            switch(Message.GetMessageType(rawString))
            {
                case MessageType.TextMessage:
                    if (user.IsMuted)
                    {
                        Send("DO REFUSE\r\nWITH\r\nYou are muted!");
                        Console.WriteLine($"Failed send attempt from muted user: {user.Username}");
                        return;
                    }

                    var textMessage = new TextMessage(currentSocket, rawString);
                    Console.WriteLine($"{textMessage.Sender}: {textMessage.Content}");

                    foreach (User client in UserManager.UsersList)
                    {
                        if (client.Socket == currentSocket)
                        {
                            continue;
                        }

                        if (!client.IsRegistered)
                        {
                            continue;
                        }

                        client.Socket.Send(textMessage.ToString());
                    }
                    break;
                case MessageType.CommandMessage:
                    var commandMessage = new CommandMessage(currentSocket, rawString);
                    commandMessage.InvokeCommand();
                    break;
            }
        }

        protected override void OnOpen()
        {
            WebSocket socket = Context.WebSocket;
            string ip = Context.UserEndPoint.Address.ToString();

            User newUser = UserManager.Connect(socket, ip);
            Console.WriteLine($"New client connected from: {newUser.Ip}");
        }

        private bool Authenticate(User user, string message)
        {
            if (message.Substring(3, 4).ToLower() != "auth") return false;
            string username = message.Substring(14);
            return UserManager.Authenticate(user.Socket, username);
        }
    }
}
