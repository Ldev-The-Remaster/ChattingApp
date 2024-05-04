using System.Threading.Channels;

namespace BackendServer.Models
{
    enum CommandType
    {
        TEST,
    }

    internal class CommandMessage : Message
    {
        private CommandType _command;
        private string _sender;
        private string? _target;
        private string? _channel;
        private string? _payload;

        private CommandType ParseCommandType()
        {
            return CommandType.TEST;
        }
        
        public CommandMessage(string rawString) : base(rawString)
        {
            _command = ParseCommandType(); // Should be parsed from rawString
            _sender = "_from";
            _target = _to;
            _channel = _in;
            _payload = _with;
        }
    }
}
