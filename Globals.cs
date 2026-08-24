public static class Globals
{
    #if DEBUG
        private static string filePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "3d-print-console.cfg");
        private static string defaultLogPath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "log.txt");
    #else
        private static string filePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "3d-print-console.cfg");
        private static string defaultLogPath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "log.txt");
    #endif

    private static Dictionary<string, string> vars = new Dictionary<string, string>
    {
        {"LOG_PATH", defaultLogPath},
        {"ADMIN_PASSWORD", ""},
        {"RECORD_SERVER_LOGS", "false"},
        {"USING_PROXY", "false"},
        {"PROXY_IP", "127.0.0.1"},
        {"HTTP_PORT", "5000"},
        {"HTTPS_PORT", "5001"},
        {"USE_HTTP", "true"},
        {"USE_HTTPS", "false"},
        {"SSL_PATH", ""},
        {"SSL_PASSWORD", ""}
    };

    public static string version = "v1.1.1";

    public static void LoadFromEnvironment ()
    {
        foreach (string key in vars.Keys)
        {
            string? env = Environment.GetEnvironmentVariable (key);

            if (env != null)
            {
                vars [key] = env;
            }
        }
    }

    public static void LoadFromFile ()
    {
        string[] lines = File.ReadAllLines (filePath);

        foreach (string line in lines)
        {
            string[] kv = line.Split ('=');

            if (vars.ContainsKey (kv [0].ToUpperInvariant ()))
            {
                vars [kv [0].ToUpperInvariant ()] = kv [1];
            }
            else
            {
                ServerOutput.WriteLine ("[!] Unrecognised variable \"" + kv [0].ToUpperInvariant () + "\" in 3d-print-console.cfg.");
            }
        }
    }

    public static string GetString (string key)
    {
        return vars [key];
    }

    public static bool GetBool (string key)
    {
        return Boolean.Parse (vars [key]);
    }

    public static int GetInt (string key)
    {
        return int.Parse (vars [key]);
    }

    public static void Set (string key, object value)
    {
        vars [key] = value.ToString ();
    }

    public static void Write ()
    {
        ServerOutput.WriteLine ("Updating 3d-print-console.cfg...");
        string config = "";

        foreach (string key in vars.Keys)
        {
            config += key + "=" + vars [key] + "\n";
        }
        
        File.WriteAllText (filePath, config);

        ServerOutput.WriteLine ("Saved 3d-print-console.cfg.");
    }
}