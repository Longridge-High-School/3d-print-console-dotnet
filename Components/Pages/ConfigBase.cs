using System.Text.Json;

namespace PageLogic
{
    public class ConfigBase : PasswordProtectedPage
    {
        public ConfigObject currentConfig;
        private string filePath;
        public string smallScreenLiveColourNoAlpha;
        public int transparency;

        public ConfigBase ()
        {
            #if DEBUG
                filePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "config.json");
            #else
                filePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "config.json");   
            #endif

            currentConfig = JsonSerializer.Deserialize<ConfigObject> (File.ReadAllText (filePath));
            smallScreenLiveColourNoAlpha = currentConfig.smallScreenLiveColour.Substring (0, currentConfig.smallScreenLiveColour.Length - 2);
            transparency = int.Parse (currentConfig.smallScreenLiveColour.Substring (currentConfig.smallScreenLiveColour.Length - 2), System.Globalization.NumberStyles.HexNumber);
            Task.Run (CheckAuthCookie);
        }

        public void Save ()
        {
            currentConfig.smallScreenLiveColour = smallScreenLiveColourNoAlpha + transparency.ToString ("X");
            Console.WriteLine ("Updated config.json to " + JsonSerializer.Serialize (currentConfig));
            File.WriteAllText (filePath, JsonSerializer.Serialize (currentConfig));
        }
    }
}