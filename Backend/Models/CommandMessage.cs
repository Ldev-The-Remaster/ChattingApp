namespace Backend.Models
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
        private string _sender;
        private string? _target;
        private string? _channel;
        private string? _payload;

        private CommandType GetCommandType()
        {
            switch(_do.ToLower())
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
        
        public CommandMessage(string rawString) : base(rawString)
        {
            _command = GetCommandType();
            _sender = _from;
            _target = _to;
            _channel = _in;
            _payload = _with;
        }
    }
}
