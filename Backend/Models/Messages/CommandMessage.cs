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
                    break;
                case CommandType.Ipban:
                    break;
                case CommandType.Unban:
                    break;
                case CommandType.Unbanip:
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
            
            if (userToMute.IsMuted == true)
            {
                CLogger.Error("Command not invoked: User is already muted");
                SendRefuse("User is already muted");
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

            UserManager.Kick(userToKick);
            SendAccept();
            CLogger.Event($"User has been Kicked: {_target}. Reason: {_with}");
            SendAlert($"User {_target} has been kicked for: {_with}");
        }
    }
}
