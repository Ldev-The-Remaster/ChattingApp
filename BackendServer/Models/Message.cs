namespace BackendServer.Models
{
    internal abstract class Message
    {
        private readonly string _requestString;
        public string RequestString
        {
            get { return _requestString; }
        }

        private string _do;
        public string Do
        {
            get { return _do; }
            set { _do = value; }
        }

        private string _from;
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        private string _to;
        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        private string _in;
        public string In
        {
            get { return _in; }
            set { _in = value; }
        }

        private DateTime _at;
        public DateTime At
        {
            get { return _at; }
            set { _at = value; }
        }

        private string _with;
        public string With
        {
            get { return _with; }
            set { _with = value; }
        }

        public Message(string requestString)
        {
            _requestString = requestString;
            // Parse requestString here
            _do = "TEST";
            _from = String.Empty;
            _to = String.Empty;
            _in = String.Empty;
            _at = DateTime.Now;
            _with = String.Empty;
        }
    }
}
