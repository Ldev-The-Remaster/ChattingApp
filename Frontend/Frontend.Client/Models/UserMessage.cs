using System.Text;
using System.Security.Cryptography;
using LSMP;

namespace Frontend.Client.Models
{
    public class UserMessage: IMessage
    {
        public string? Sender { get; set; }
        public string Channel { get; set; } 
        public long TimeStamp { get; set; }
        public string Hash { get; set; }
        public string Content { get; set; }
        public bool IsConfirmed { get; set; }

        public UserMessage(string? user, string hash, string channel, string content, long timestamp, bool isConfirmed)
        {
            Sender = user;
            Hash = hash;
            Channel = channel;
            Content = content;
            TimeStamp = timestamp;
            IsConfirmed = true;
        }

        public UserMessage(string user, string channel, string content, bool isConfirmed) 
        { 
            Sender = user;
            Channel = channel;
            Hash =  ComputeSha256Hash(content + DateTimeOffset.Now.ToUnixTimeMilliseconds());
            Content = content;
            TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            IsConfirmed = isConfirmed;
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
