namespace Frontend.Client.Models
{
    public static class ClientManager
    {
        struct MessageParams
        {
            public string Do = "";
            public string From = "";
            public string To = "";
            public string In = "";
            public long At = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            public string With = "";

            public MessageParams(string rawString)
            {
                string[] commandPairs = rawString.Split("\r\n");
                foreach (string line in commandPairs)
                {
                    string[] commandPair = line.Split(' ');
                    switch (commandPair[0])
                    {
                        case "DO":
                            Do = commandPair[1];
                            break;
                        case "FROM":
                            From = commandPair[1];
                            break;
                        case "TO":
                            To = commandPair[1];
                            break;
                        case "IN":
                            In = commandPair[1];
                            break;
                        case "AT":
                            At = long.Parse(commandPair[1]);
                            break;
                        case "WITH":
                            With = rawString.Split("WITH\r\n", 2)[1];
                            break;
                    }
                }
            }
        }
        private static readonly List<string> _users = new List<string>();
        public static event Action? OnUserListUpdate;
        public static List<string> CurrentUsersList
            { get { return _users; } }

        public static void UpdateUserList(string introduceMessage)
        {
            var users = GetUsersList(introduceMessage);
            Console.WriteLine("Parsed users: " + string.Join(", ", users));

            _users.Clear();
            _users.AddRange(users);

            OnUserListUpdate?.Invoke();
        }

        public static List<string> GetUsersList(string introduceMessage) 
        {
            var msg = new MessageParams(introduceMessage);
            if (msg.Do == "INTRODUCE")
            {
                var usersList = msg.With.Split(new[] { "/*$*/" }, StringSplitOptions.None);

                return new List<string>(usersList);
            }
            return new List<string>();
        }
    }
}
