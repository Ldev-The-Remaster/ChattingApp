using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
