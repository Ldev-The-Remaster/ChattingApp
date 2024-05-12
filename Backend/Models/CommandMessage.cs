namespace Backend.Models
{
    enum CommandType
    {
        TEST,
    }

    public class CommandMessage : Message
    {
        private CommandType _command;
        private string _sender;
        private string? _target;
        private string? _channel;
        private string? _payload;

        private CommandType ParseCommandType()
        {
            // TODO: Implement parser and commandtypes
            return CommandType.TEST;
        }
        
        public CommandMessage(string rawString) : base(rawString)
        {
            _command = ParseCommandType();
            _sender = _from;
            _target = _to;
            _channel = _in;
            _payload = _with;
        }
    }
}
