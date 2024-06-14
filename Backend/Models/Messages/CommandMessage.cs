using Backend.Models.Users;
using Backend.ServerModules;
using Backend.Utils;
using WebSocketSharp;

namespace Backend.Models.Messages
{
    public class CommandMessage : Message
    {
        enum CommandType
        {
            Mute,
            Kick,
            Ban,
            Ipban,
            Unban,
            Unbanip,
            Remember,
            Unknown
        }

        private CommandType _command;
        private User? _sender;
        private string? _target;
        private string? _channel;
        private string? _payload;

        public CommandMessage(WebSocket socket, string rawString) : base(socket, rawString)
        {
            _command = GetCommandType();
            _sender = UserManager.GetUserBySocket(socket);
            _target = _to;
            _channel = _in;
            _payload = _with;
        }

        private CommandType GetCommandType()
        {
            switch (_do.ToLower())
            {
                case "mute":
                    return CommandType.Mute;
                case "kick":
                    return CommandType.Kick;
                case "ban":
                    return CommandType.Ban;
                case "ipban":
                    return CommandType.Ipban;
                case "unban":
                    return CommandType.Unban;
                case "unbanip":
                    return CommandType.Unbanip;
                case "remember":
                    return CommandType.Remember;
                default:
                    return CommandType.Unknown;
            }
        }

        public void InvokeCommand()
        {
            switch(_command)
            {
                case CommandType.Mute:
                    ProcessMute();
                    break;
                case CommandType.Kick:
                    ProcessKick();
                    break;
                case CommandType.Ban:
                    ProcessBan();
                    break;
                case CommandType.Ipban:
                    break;
                case CommandType.Unban:
                    ProcessUnban(); 
                    break;
                case CommandType.Unbanip:
                    break;
                case CommandType.Remember:
                    ProcessRemember();
                    break;
                case CommandType.Unknown:
                    break;
            }
        }

        private void SendAccept()
        {
            if (_sender != null)
            {
                _sender.Socket.Send("DO ACCEPT");
            }
        }
        private void SendRefuse(string reason = "")
        {
            if (_sender != null)
            {
                string message = $"DO REFUSE\r\nWITH\r\n{reason}";
                _sender.Socket.Send(message);
            }
        }

        private void SendToAll(TextMessage message)
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

        private void SendAlert(string message)
        {
            TextMessage alert = new TextMessage(null, "");
            alert.Content = message;

            SendToAll(alert);
        }


        private void ProcessMute()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Mute target not specified");
                SendRefuse("Please indicate the user to mute");
                return;
            }

            User? userToMute = UserManager.GetUserByUsername(_target);
            if (userToMute == null)
            {
                CLogger.Error("Mute target not found in DB");
                SendRefuse("User not found");
                return;
            }
            
            if (userToMute.IsMuted)
            {
                CLogger.Error("Command not invoked: User is already muted");
                SendRefuse("User is already muted");
                return;
            }

            if (userToMute == _sender)
            {
                CLogger.Error("Command not invoked: You cannot mute yourself");
                SendRefuse("Did you really want to mute yourself?");
                return;
            }

            UserManager.Mute(userToMute, _with);
            SendAccept();
            CLogger.Event($"User has been Muted: {_target}. Reason: {_with}");
            SendAlert($"User has been Muted: {_target}. Reason: {_with}");
        }

        private void ProcessKick()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Kick target not specified");
                SendRefuse("Please indicate the user to kick");
                return;
            }

            User? userToKick = UserManager.GetUserByUsername(_target);
            if (userToKick == null) 
            {
                CLogger.Error("Kick target not found in DB");
                SendRefuse("User not found");
                return;
            }

            if (userToKick == _sender)
            {
                CLogger.Error("Command not invoked: You cannot kick yourself");
                SendRefuse("You can't kick yourself idiot");
                return;
            }

            UserManager.Kick(userToKick);
            SendAccept();
            CLogger.Event($"User has been Kicked: {_target}. Reason: {_with}");
            SendAlert($"User {_target} has been kicked for: {_with}");
        }

        private void ProcessBan()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Ban target not specified");
                SendRefuse("Please indicate the user to ban");
                return;
            }

            User? userToBan = UserManager.GetUserByUsername(_target);
            if (userToBan == null)
            {
                CLogger.Error("Ban target not found in DB");
                SendRefuse("User not found");
                return;
            }

            if (userToBan == _sender)
            {
                CLogger.Error("Command not invoked: You cannot ban yourself");
                SendRefuse("You can't ban yourself idiot");
                return;
            }

            if (userToBan.IsBanned) 
            {
                CLogger.Error("Command not invoked: User is already banned");
                SendRefuse("User is already banned");
                return;
            }

            UserManager.Ban(userToBan);
            SendAccept();
            CLogger.Event($"User has been Banned: {_target}. Reason: {_with}");
            SendAlert($"User {_target} has been Banned for: {_with}");
        }

        private void ProcessUnban()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Unban target not specified");
                SendRefuse("Please indicate the user to unban");
                return;
            }

            User? userToUnban = UserManager.GetUserByUsername(_target);
            if (userToUnban == null)
            {
                CLogger.Error("Unban target not found in DB");
                SendRefuse("User not found");
                return;
            }

            if (userToUnban == _sender)
            {
                CLogger.Error("Command not invoked: You cannot unban yourself");
                SendRefuse("You can't unban yourself");
                return;
            }

            if (userToUnban.IsBanned == false)
            {
                CLogger.Error("Command not invoked: User is not banned");
                SendRefuse("User is not banned");
                return;
            }

            UserManager.Unban(userToUnban);
            SendAccept();
            CLogger.Event("User has been Unbanned: " + _target);
            SendAlert("User has been Unbanned: " + _target);
        }


        private void ProcessRemember()
        {
            int fromId, toId;
            if (!int.TryParse(_from, out fromId) || !int.TryParse(_to, out toId))
            {
                SendRefuse("Invalid limits provided");
                return;
            }

            string channel = "general-chat";
            if (_in != string.Empty)
            {
                channel = _in;
            }

            List<TextMessage> messageHistory = TextMessage.GetMessageHistory(channel, fromId, toId);

            string msgString = "DO REMIND\r\nWITH\r\n";
            string msgArray = LSMPBehavior.EncodeArrayToString(messageHistory);
            msgString += msgArray;

            if (_sender != null)
            {
                _sender.Socket.Send(msgString);
            }
        }
    }

}
