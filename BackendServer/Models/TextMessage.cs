namespace BackendServer.Models
{
    internal class TextMessage(string sender, string content) : Message(content)
    {

        private string sender = sender;
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        public override string ParseRawMessage() { return Content; }
    }
}
