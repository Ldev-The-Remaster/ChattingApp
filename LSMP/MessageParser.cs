namespace LSMP
{
    public struct MessageParser
    {
        public string Do = "";
        public string From = "";
        public string To = "";
        public string In = "";
        public long At = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string With = "";

        public MessageParser(string rawString)
        {
            try
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
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Parsing: {e.Message}");
            }
        }
    }
}
