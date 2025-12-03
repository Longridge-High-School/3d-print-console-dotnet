public static class EnvVars
{
    public static string logPath = "log.txt";
    public static string password = "admin";
    public static int redisPort = 6379;
    
    public static void Load ()
    {
        string? logPath = Environment.GetEnvironmentVariable ("LOG_PATH");

        if (logPath != null)
        {
            EnvVars.logPath = logPath;
        }
        else
        {
            Console.WriteLine ("[!] LOG_PATH not set!");
        }

        string? password = Environment.GetEnvironmentVariable ("ADMIN_PASSWORD");

        if (password != null)
        {
            EnvVars.password = password;
        }
        else
        {
            Console.WriteLine ("[!] ADMIN_PASSWORD not set! Using \"admin\", which is insecure.");
        }

        string? redisPort = Environment.GetEnvironmentVariable ("REDIS_PORT");

        if (redisPort != null)
        {
            try
            {
                EnvVars.redisPort = Int16.Parse (redisPort);
            }
            catch
            {
                Console.WriteLine ("[!] REDIS_PORT is set, but isn't a number!");
            }
        }
    }
}