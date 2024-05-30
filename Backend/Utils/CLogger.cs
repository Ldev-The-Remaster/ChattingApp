namespace Backend.Utils
{
    public static class CLogger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"{GetTimestamp()} | {message}");
        }

        public static void Chat(string sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(sender);
            Console.ResetColor();
            Console.Write($" > {message}");
            Console.WriteLine();
        }

        public static void Event(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log(message);
            Console.ResetColor();
        }

        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(message);
            Console.ResetColor();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(message);
            Console.ResetColor();
        }

        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
