using Backend;
using Backend.Database;

internal class Program
{
    private static void Main(string[] args)
    {
        ServerStartupOptions serverOptions = ServerStartupOptions.GetServerStartupOptions();
        RunServer(serverOptions);
    }

    private static void RunServer(ServerStartupOptions serverOptions)
    {
        TextMessageContext.SetUp();

        Console.WriteLine("Hello, World!");
        Console.WriteLine($"Server should start on port {serverOptions.Port}");
    }
}
