using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer.Models
{
    internal abstract class Message(string content)
    {
        private string content = content;
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        private DateTime timestamp;
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public abstract string ParseRawMessage();
    }
}
