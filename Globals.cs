public static class Globals
{
    public static string logPath = "log.txt";
    public static string password = "admin";
    public static bool recordServerLogs = false;
    
    public static void LoadFromEnvironment ()
    {
        string? logPath = Environment.GetEnvironmentVariable ("LOG_PATH");

        if (logPath != null)
        {
            Globals.logPath = logPath;
        }
        else
        {
            ServerOutput.WriteLine ("[!] LOG_PATH not set!");
        }

        string? password = Environment.GetEnvironmentVariable ("ADMIN_PASSWORD");

        if (password != null)
        {
            Globals.password = password;
        }
        else
        {
            ServerOutput.WriteLine ("[!] ADMIN_PASSWORD not set! Using \"admin\", which is insecure.");
        }

        string? recordServerLogs = Environment.GetEnvironmentVariable ("RECORD_SERVER_LOGS");

        if (recordServerLogs != null)
        {
            try
            {
                Globals.recordServerLogs = Boolean.Parse (recordServerLogs);
            }
            catch
            {
                ServerOutput.WriteLine ("[!] RECORD_SERVER_LOGS is not a boolean.");
            }
        }
    }

    public static void LoadFromFile ()
    {
        string[] lines = File.ReadAllLines ("3d-print-console.cfg");

        foreach (string line in lines)
        {
            string[] kv = line.Split ('=');

            switch (kv [0].ToUpperInvariant ())
            {
                case "LOG_PATH":
                    logPath = kv [1];
                    break;
                
                case "ADMIN_PASSWORD":
                    password = kv [1];
                    break;

                case "RECORD_SERVER_LOGS":
                    try
                    {
                        Globals.recordServerLogs = Boolean.Parse (kv [1]);
                    }
                    catch
                    {
                        ServerOutput.WriteLine ("[!] Key RECORD_SERVER_LOGS in 3d-print-console.cfg is not a boolean.");
                    }
                    break;

                default:

                    ServerOutput.WriteLine ("[!] Unrecognised variable \"" + kv [0].ToUpperInvariant () + "\" in 3d-print-console.cfg.");
                    break;
            }
        }
    }
}