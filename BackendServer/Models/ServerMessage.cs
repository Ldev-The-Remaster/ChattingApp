using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
