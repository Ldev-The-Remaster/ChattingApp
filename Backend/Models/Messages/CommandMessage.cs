using Backend.Models.Users;
using Backend.ServerModules;
using LSMP.Utils;
using LSMP;
using WebSocketSharp;

namespace Backend.Models.Messages
{
    public class CommandMessage : Message
    {
        #region Fields

        enum CommandType
        {
            Mute,
            Kick,
            Ban,
            Banip,
            Unban,
            Unbanip,
            Remember,
            Unmute,
            Unknown
        }

        private CommandType _command;
        private User? _sender;
        private string? _target;
        private string? _channel;
        private string? _payload;

        #endregion

        #region Constructors

        public CommandMessage(WebSocket socket, string rawString) : base(socket, rawString)
        {
            _command = GetCommandType();
            _sender = UserManager.GetUserBySocket(socket);
            _target = _to;
            _channel = _in;
            _payload = _with;
        }

        #endregion

        #region Methods

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
                case "banip":
                    return CommandType.Banip;
                case "unban":
                    return CommandType.Unban;
                case "unbanip":
                    return CommandType.Unbanip;
                case "remember":
                    return CommandType.Remember;
                case "unmute":
                    return CommandType.Unmute;
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
                case CommandType.Banip:
                    ProcessBanIp();
                    break;
                case CommandType.Unban:
                    ProcessUnban(); 
                    break;
                case CommandType.Unbanip:
                    ProcessUnbanIp();
                    break;
                case CommandType.Remember:
                    ProcessRemember();
                    break;
                case CommandType.Unmute:
                    ProcessUnmute();
                    break;
                case CommandType.Unknown:
                    break;
            }
        }

        #endregion

        #region Senders

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
                _sender.Socket.Send(Messaging.RefuseMessage(reason));
            }
        }

        private void SendToAll(IEncodable message)
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
            AlertMessage alert = new AlertMessage(message);

            SendToAll(alert);
        }

        #endregion

        #region Commands

        private void ProcessMute()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("Command not invoked: User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Command not invoked: Mute target not specified");
                SendRefuse("Please indicate the user to mute");
                return;
            }

            User? userToMute = UserManager.GetUserByUsername(_target);
            if (userToMute == null)
            {
                CLogger.Error("Command not invoked: Mute target not found in DB");
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
                CLogger.Error("Command not invoked: User cannot mute themself");
                SendRefuse("Did you really want to mute yourself?");
                return;
            }

            UserManager.Mute(userToMute, _with);
            SendAccept();
            CLogger.Event($"User has been Muted: {_target}. Reason: {_with}");
            SendAlert($"User has been Muted: {_target}. Reason: {_with}");
        }

        private void ProcessUnmute()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("Command not invoked: User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Command not invoked: Mute target not specified");
                SendRefuse("Please indicate the user to unmute");
                return;
            }

            User? userToUnmute = UserManager.GetUserByUsername(_target);
            if (userToUnmute == null)
            {
                CLogger.Error("Command not invoked: Unmute target not found in DB");
                SendRefuse("User not found");
                return;
            }

            if (!userToUnmute.IsMuted)
            {
                CLogger.Error("Command not invoked: User is not muted");
                SendRefuse("User is not muted");
                return;
            }

            if (userToUnmute == _sender)
            {
                CLogger.Error("Command not invoked: User cannot unmute themself");
                SendRefuse("Did you really mute yourself?");
                return;
            }

            UserManager.Unmute(userToUnmute);
            SendAccept();
            CLogger.Event($"User has been Unmuted: {_target}.");
            SendAlert($"User has been Muted: {_target}.");
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
                CLogger.Error("Command not invoked: User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Command not invoked: Kick target not specified");
                SendRefuse("Please indicate the user to kick");
                return;
            }

            User? userToKick = UserManager.GetUserByUsername(_target);
            if (userToKick == null) 
            {
                CLogger.Error("Command not invoked: Kick target not found in DB");
                SendRefuse("User not found");
                return;
            }

            if (userToKick == _sender)
            {
                CLogger.Error("Command not invoked: User cannot kick themself");
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
                CLogger.Error("Command not invoked: User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Command not invoked: Ban target not specified");
                SendRefuse("Please indicate the user to ban");
                return;
            }

            User? userToBan = UserManager.GetUserByUsername(_target);
            if (userToBan == null)
            {
                CLogger.Error("Command not invoked: Ban target not found in DB");
                SendRefuse("User not found");
                return;
            }

            if (userToBan == _sender)
            {
                CLogger.Error("Command not invoked: User cannot ban themself");
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
                CLogger.Error("Command not invoked: User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Command not invoked: Unban target not specified");
                SendRefuse("Please indicate the user to unban");
                return;
            }

            User? userToUnban = User.GetUserFromDB(_target);
            if (userToUnban == null)
            {
                CLogger.Error("Command not invoked: Unban target not found in DB");
                SendRefuse("User not found");
                return;
            }

            if (userToUnban == _sender)
            {
                CLogger.Error("Command not invoked: User cannot unban themself");
                SendRefuse("You can't unban yourself");
                return;
            }

            if (!userToUnban.IsBanned)
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

        private void ProcessBanIp()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("Command not invoked: User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Command not invoked: Ban IP not specified");
                SendRefuse("Please indicate the IP to ban");
                return;
            }

            User? userToBan = UserManager.GetUserByUsername(_target);
            if (userToBan == null)
            {
                CLogger.Error("Command not invoked: BanIP target not found in DB");
                SendRefuse("User not found");
                return;
            }

            string ipToBan = userToBan.Ip;

            if (ipToBan == _sender.Ip)
            {
                CLogger.Error("Command not invoked: User cannot ban themself");
                SendRefuse("You can't ban yourself idiot");
                return;
            }

            if (BannedIp.AlreadyExists(ipToBan))
            {
                CLogger.Error("Command not invoked: IP is already banned");
                SendRefuse("IP is already banned");
                return;
            }

            UserManager.BanIp(ipToBan);
            SendAccept();
            CLogger.Event($"IP has been Banned: {ipToBan}. Reason: {_with}");
            SendAlert($"IP {ipToBan} has been Banned: Reason: {_with}");
        }

        private void ProcessUnbanIp()
        {
            if (_sender == null)
            {
                CLogger.Error("Command not invoked: Missing sender");
                return;
            }

            if (UserManager.IsUserAdmin(_sender) == false)
            {
                CLogger.Error("Command not invoked: User must be an adminstrator to use this command");
                SendRefuse("You must be an adminstrator to use this command");
                return;
            }

            if (_target == null)
            {
                CLogger.Error("Command not invoked: UnbanIP target not specified");
                SendRefuse("Please indicate the IP to unban");
                return;
            }

            string ipToUnban = _target;

            if (ipToUnban == _sender.Ip)
            {
                CLogger.Error("Command not invoked: User cannot unban themself");
                SendRefuse("How'd you manage to ban yourself?");
                return;
            }

            if (!BannedIp.AlreadyExists(ipToUnban))
            {
                CLogger.Error("Command not invoked: IP is not banned");
                SendRefuse("IP is not banned");
                return;
            }

            UserManager.UnbanIp(ipToUnban);
            SendAccept();
            CLogger.Event($"IP has been Unbanned: {ipToUnban}");
            SendAlert($"IP has been Unbanned: {ipToUnban}");
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

            var messageHistory = TextMessage.GetMessageHistory(channel, fromId, toId);

            if (_sender != null)
            {
                _sender.Socket.Send(Messaging.RemindMessage(messageHistory));
            }
        }

        #endregion
    }

}
