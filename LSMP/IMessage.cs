namespace LSMP
{
    public interface IMessage
    {
        public string Sender { get; set; }
        public string Channel { get; set; }
        public long TimeStamp { get; set; }
        public string Hash { get; }
        public string Content { get; set; }


    }
}
