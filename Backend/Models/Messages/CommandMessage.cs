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
                    break;
                case CommandType.Ban:
                    break;
                case CommandType.Ipban:
                    break;
                case CommandType.Unban:
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

            UserManager.Mute(userToMute);
            SendAccept();
            CLogger.Event("User Muted: " + _target);
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

            string msgString = "DO POPULATE\r\nWITH\r\n";
            string msgArray = LSMPBehavior.EncodeArrayToString(messageHistory);
            msgString += msgArray;

            if (_sender != null)
            {
                _sender.Socket.Send(msgString);
            }
        }
    }

}
