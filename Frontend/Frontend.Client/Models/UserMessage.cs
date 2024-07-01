using System.Text;
using System.Security.Cryptography;

namespace Frontend.Client.Models
{
    public class UserMessage
    {
        public string? User { get; set; }
        public string Hash { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public UserMessage(string? user, string hash, string content, DateTime timestamp)
        {
            User = user;
            Hash = hash;
            Content = content;
            Timestamp = timestamp;
        }

        // TO MID: update constructor to take isconfirmed and set it
        public UserMessage(string user, string content) 
        { 
            User = user;
            Hash =  ComputeSha256Hash(content + DateTimeOffset.Now.ToUnixTimeMilliseconds());
            Content = content;
            Timestamp = DateTime.Now;
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
