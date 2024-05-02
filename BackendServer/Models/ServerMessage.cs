namespace BackendServer.Models
{
    enum MessageType
    {
        Info,
        Warn,
        Error
    }

    internal class ServerMessage(MessageType messageType, string content) : Message(content)
    {
        private MessageType messageType = messageType;
        public MessageType MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        protected override void ParseRawMessage(string rawMessage) { }
    }
}
