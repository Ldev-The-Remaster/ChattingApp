using Backend.ServerModules;
using WebSocketSharp;
using WebSocketSharp.Server;

internal class Program
{
    private static void Main(string[] args)
    {
        ServerStartupOptions serverOptions = ServerStartupOptions.GetServerStartupOptions();
        RunServer(serverOptions);
    }

    private static void RunServer(ServerStartupOptions serverOptions)
    {
        Console.WriteLine($"Server should start on port {serverOptions.Port}");
        WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:" + serverOptions.Port);

        wssv.AddWebSocketService<ServerBehavior>("/");
        wssv.Start();
        Console.WriteLine("Press Esc to shutdown the server");

        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Shutting down the server...");
                wssv.Stop();
                break;
            }
        }
    }



}
