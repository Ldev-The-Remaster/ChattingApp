namespace Backend.ServerModules
{
    public class ServerStartupOptions
    {
        public int Port;

        bool IsValid()
        {
            if (Port < 1 || Port > 65535)
            {
                Console.WriteLine("Invalid port, input number between 1 and 65535");
                return false;
            }

            return true;
        }

        public static ServerStartupOptions GetServerStartupOptions()
        {
            ServerStartupOptions serverOptions = new ServerStartupOptions();

            // MAYBE: Try getting options from CLI arguments first

            do
            {
                Console.WriteLine("Input server port:");
                if (!int.TryParse(Console.ReadLine(), out serverOptions.Port))
                {
                    continue;
                }
            } while (!serverOptions.IsValid());

            return serverOptions;
        }
    }
}
