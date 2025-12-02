using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

public static class Cache
{
    private static IDatabase database;

    public static void Connect ()
    {
        Console.WriteLine ("Connecting to Redis on port " + EnvVars.redisPort.ToString () + "...");
        ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect("localhost:" + EnvVars.redisPort.ToString ());
        database = muxer.GetDatabase ();
    }

    public static void Set (string token, string expiry)
    {
        Console.WriteLine ("Saving key " + token + " with expiry " + expiry + "...");
        database.StringSet (token, expiry);
    }

    public static string? Get (string token)
    {
        Console.WriteLine ("Attempting to read expiry of key " + token + "...");
        string? result =  database.StringGet (token);

        if (result != null)
        {
            Console.WriteLine ("Found " + result + "!");
        }
        else
        {
            Console.WriteLine ("Could not find key.");
        }

        return result;
    }

}