using Backend.Models.Users;
using Backend.Utils;
using WebSocketSharp;

namespace Backend.Models.Messages
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

    public class CommandMessage : Message
    {
        private CommandType _command;
        private User? _sender;
        private string? _target;
        private string? _channel;
        private string? _payload;

        public void InvokeCommand()
        {
            switch(_command)
            {
                case CommandType.Mute:
                    if (_target == null)
                    {
                        CLogger.Error("Mute target not specified");
                        return;
                    }
                    User? userToMute = UserManager.GetUserByUsername(_target);
                    if (userToMute == null)
                    {
                        CLogger.Error("Mute target not found in DB");
                        return;
                    }
                    UserManager.Mute(userToMute);
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
                case CommandType.Unknown:
                    break;
            }
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

        public CommandMessage(WebSocket socket, string rawString) : base(socket, rawString)
        {
            _command = GetCommandType();
            _sender = UserManager.GetUserBySocket(socket);
            _target = _to;
            _channel = _in;
            _payload = _with;
        }
    }
}
