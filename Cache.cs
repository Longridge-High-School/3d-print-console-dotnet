using System.Linq.Expressions;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

public static class Cache
{
    private static IDatabase database;

    public static void Connect ()
    {
        ServerOutput.WriteLine ("Connecting to Redis on " + Globals.redisURL + "...");

        try
        {
            ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect (Globals.redisURL.ToString ());
            database = muxer.GetDatabase ();            
        }
        catch
        {
            ServerOutput.WriteLine ("[!] Could not connect to Redis, exiting...");
            Environment.Exit (1);   
        }
    }

    public static void Set (string token, string expiry)
    {
        database.StringSet (token, expiry);
    }

    public static string? Get (string token)
    {
        string? result =  database.StringGet (token);
        return result;
    }

}