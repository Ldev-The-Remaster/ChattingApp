using Backend.Database;
using Backend.ServerModules;
using Backend.Utils;
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
        SetUpDatabase();

        WebSocketServer wssv = new WebSocketServer("ws://0.0.0.0:" + serverOptions.Port);
        wssv.AddWebSocketService<ServerBehavior>("/");
        wssv.Start();
        CLogger.Log($"Server is up and listening on port: {serverOptions.Port}");
        CLogger.Log("Press Esc to shutdown the server");

        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Escape)
            {
                CLogger.Warn("Shutting down the server...");
                wssv.Stop();
                break;
            }
        }
    }

    #region Local Methods

    private static void SetUpDatabase()
    {
        UserContext.SetUp();
        BannedIpContext.SetUp();
        TextMessageContext.SetUp();
    }

    #endregion
}
