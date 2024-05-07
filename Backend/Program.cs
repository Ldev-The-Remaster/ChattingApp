using Backend;
using Backend.Utils;

internal class Program
{
    private static void Main(string[] args)
    {
        TextMessageContext.SetUp();

        ServerStartupOptions serverOptions = ServerStartupOptions.GetServerStartupOptions();
        RunServer(serverOptions);
    }

    private static void RunServer(ServerStartupOptions serverOptions)
    {
        Console.WriteLine("Hello, World!");
        Console.WriteLine($"Server should start on port {serverOptions.Port}");
    }
}
