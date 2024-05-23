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
            switch(Message.GetMessageType(rawString))
            {
                case MessageType.TextMessage:
                    var textMessage = new TextMessage(rawString);
                    break;
                case MessageType.CommandMessage:
                    var commandMessage = new CommandMessage(rawString);
                    commandMessage.InvokeCommand();
                    break;
            }

            Console.WriteLine("Recieved message from client: " + e.Data);
            Send(e.Data);
        }

        protected override void OnOpen()
        {
            WebSocket socket = Context.WebSocket;
            string ip = Context.UserEndPoint.Address.ToString();

            User newUser = UserManager.Connect(socket, ip);
        }
    }
}
