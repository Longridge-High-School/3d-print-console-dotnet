public static class ServerOutput
{
    public static void WriteLine (string message, bool? useConsole = true)
    { 
        if (useConsole != false)
        {
            Console.WriteLine (message);
        }

        if (Globals.recordServerLogs)
        {
            message = DateTime.UtcNow.ToString () + " - " + message;
            File.AppendAllText (Globals.logPath, "\n" + message);   
        }
    }
}