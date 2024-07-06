using Backend.Models.Messages;
using Backend.Models.Users;
using LSMP.Utils;
using LSMP;
using WebSocketSharp;
using static Backend.Models.Messages.Message;

namespace Backend.ServerModules
{
    public class ServerBehavior : LSMPBehavior
    {
        #region Callbacks

        protected override void OnOpen()
        {
            WebSocket socket = Context.WebSocket;
            string ip = Context.UserEndPoint.Address.ToString();

            UserManager.Connect(socket, ip);
            CLogger.Event($"New client connected from: {ip}");
        }

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

            if (Messaging.IsAuthRequest(rawString))
            {
                string username = rawString.Substring(14);
                username = username.Trim();
                Authenticate(user, username);
            }

            if(!user.IsRegistered)
            {
                SendRefuse("You are not authenticated!");
                CLogger.Error($"Message refused, user is not authenticated: {user.Ip}");
                return;
            }

            switch(GetMessageType(rawString))
            {
                case MessageType.TextMessage:
                {
                    if (user.IsMuted)
                    {
                        SendRefuse("You are muted!");
                        CLogger.Error($"Failed send attempt from muted user: {user.Username}");
                        return;
                    }

                    var textMessage = new TextMessage(socket, rawString);

                    SendToAll(textMessage);
                    textMessage.SaveToDb();
                    CLogger.Chat(textMessage.Sender, textMessage.Content);
                    break;
                }
                case MessageType.CommandMessage:
                {
                    var commandMessage = new CommandMessage(socket, rawString);
                    commandMessage.InvokeCommand();
                    break;
                }
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            WebSocket socket = Context.WebSocket;
            User? user = UserManager.GetUserBySocket(socket);
            if (user == null)
            {
                CLogger.Error("Disconnected user not found in UserList");
                return;
            }

            UserManager.UsersList.Remove(user);
            CLogger.Event($"User disconnected: {user.Username}");

            SendUserListToAll();
            SendAlert($"User {user.Username} has disconnected");
        }

        #endregion

        #region Local Methods

        private void Authenticate(User user, string username)
        {
            if (user.IsRegistered)
            {
                SendRefuse("Authentication failed: Already authenticated");
                CLogger.Error($"Failed authentication request from already authentiated user: {user.Username}");
                return;
            }

            if (!UserManager.InitializeUser(user.Socket, username))
            {
                SendRefuse("Authentication failed: Username taken");
                CLogger.Error($"Failed authentication attempt, username taken: {username}");
                return;
            }

            if (user.IsBanned)
            {
                SendRefuse("Authentication failed: Username is banned");
                CLogger.Error($"Connection refused from banned user: {user.Username}");
                UserManager.Disconnect(user);
                return;
            }

            if (BannedIp.AlreadyExists(user.Ip)) 
            {
                SendRefuse("Authentication failed: IP is banned");
                CLogger.Error($"Connection refused from banned IP: {user.Ip}");
                UserManager.Disconnect(user);
                return;
            }

            SendAccept();
            SendUserListToAll();
            SendAlert($"User {username} has connected");
        }

        #endregion
    }
}
