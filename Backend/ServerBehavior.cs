
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Backend
{
    public class ServerBehavior : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Recieved message from client: " + e.Data);
            Send(e.Data);
        }
    }
}
