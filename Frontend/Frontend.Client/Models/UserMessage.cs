using System.Text;
using System.Security.Cryptography;
using LSMP;

namespace Frontend.Client.Models
{
    public class UserMessage: IMessage
    {
        public string? Sender { get; set; }
        public string Channel { get; set; } = string.Empty;
        public long TimeStamp { get; set; }
        public string Hash { get; set; }
        public string Content { get; set; }

        public UserMessage(string? user, string hash, string content, long timestamp)
        {
            Sender = user;
            Hash = hash;
            Content = content;
            TimeStamp = timestamp;
        }

        // TO MID: update constructor to take isconfirmed and set it
        public UserMessage(string user, string content) 
        { 
            Sender = user;
            Hash =  ComputeSha256Hash(content + DateTimeOffset.Now.ToUnixTimeMilliseconds());
            Content = content;
            TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        private string ComputeSha256Hash(string hashInput)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
