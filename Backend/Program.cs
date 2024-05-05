internal class Program
{
    struct ServerOptions
    {
        public int Port;
    }

    private static void Main(string[] args)
    {
        ServerOptions serverOptions = GetServerOptions();
        RunServer(serverOptions);
    }

    private static ServerOptions GetServerOptions()
    {
        ServerOptions serverOptions = new ServerOptions();

        // TODO: Try getting options from CLI arguments first

        while (serverOptions.Port == 0 || serverOptions.Port < 1 || serverOptions.Port > 65535)
        {
            Console.WriteLine("Input server port:");
            string? portInput = Console.ReadLine();

            try
            {
                serverOptions.Port = Convert.ToInt32(portInput);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input");
                continue;
            }
            
            if (serverOptions.Port < 1 || serverOptions.Port > 65535)
            {
                Console.WriteLine("Invalid port, input number between 1 and 65535");
            }
        }
        
        return serverOptions;
    }

    private static void RunServer(ServerOptions serverOptions)
    {
        Console.WriteLine("Hello, World!");
        Console.WriteLine($"Server should start on port {serverOptions.Port}");
    }
}