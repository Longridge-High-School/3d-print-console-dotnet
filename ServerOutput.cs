public static class ServerOutput
{
    public static void WriteLine (string message, bool? useConsole = true)
    { 
        if (useConsole != false)
        {
            Console.WriteLine (message);
        }

        if (Globals.GetBool ("RECORD_SERVER_LOGS"))
        {
            message = DateTime.UtcNow.ToString () + " - " + message;
            File.AppendAllText (Globals.GetString ("LOG_PATH"), "\n" + message);   
        }
    }
}