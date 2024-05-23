﻿using Backend.Models.Users;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Backend.ServerModules
{
    public class ServerBehavior : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
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