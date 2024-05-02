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

        public override string ParseRawMessage() { return Content; }
    }
}
