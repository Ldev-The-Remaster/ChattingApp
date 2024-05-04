namespace BackendServer.Models
{
    internal abstract class Message
    {
        private readonly string _requestString;

        protected string _do;
        protected string _from;
        protected string _to;
        protected string _in;
        protected DateTime _at;
        protected string _with;

        public Message(string requestString)
        {
            _requestString = requestString;
            // Parse requestString here (akram kaif tmam?)
            _do = "TEST";
            _from = String.Empty;
            _to = String.Empty;
            _in = String.Empty;
            _at = DateTime.Now;
            _with = String.Empty;
        }
    }
}
