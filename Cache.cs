public class Cache
{
    private Dictionary <string, string> keyValuePairs = new Dictionary <string, string> (); 

    public Cache ()
    {
    }

    public void Set (string token, string expiry)
    {
        keyValuePairs.Add (token, expiry);
    }

    public string? Get (string token)
    {
        if (keyValuePairs.ContainsKey (token))
        {
            return keyValuePairs [token];
        }
        else
        {
            return null;
        }
    }

    public void Delete (string token)
    {
        keyValuePairs.Remove (token);
    }
}