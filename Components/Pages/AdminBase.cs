using System.Text.Json;

namespace PageLogic
{    
    public class AdminBase : PasswordProtectedPage
    {
        private string printerFilePath;
        private string cameraFilePath;
        public string log = "";
        public List<PrinterObject> printers;
        public List<CameraObject> cameras;
        public Dictionary<string, bool> transparencies = new Dictionary<string, bool> ();

        public AdminBase ()
        {         
            #if DEBUG
                printerFilePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "printers.json");
                cameraFilePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "cameras.json");
            #else
                printerFilePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "printers.json");   
                cameraFilePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "cameras.json");   
            #endif

            printers = JsonSerializer.Deserialize<List<PrinterObject>> (File.ReadAllText (printerFilePath));

            foreach (PrinterObject printer in printers) // Set transparency values seperately because the HTML colour element does not support alpha channels???
            {
                if (printer.filament.Length > 7) // If the string is too short, there is no alpha applied.
                {
                    int transparency = int.Parse (printer.filament.Substring (printer.filament.Length - 2), System.Globalization.NumberStyles.HexNumber);

                    if (transparency <= 128)
                    {
                        transparencies.Add (printer.id, true);
                    }
                    else
                    {
                        transparencies.Add (printer.id, false);
                    }

                    printer.filament = printer.filament.Substring (0, 7); // Trim alpha values.
                }
                else
                {
                    transparencies.Add (printer.id, false);
                }
            }

            cameras = JsonSerializer.Deserialize<List<CameraObject>> (File.ReadAllText (cameraFilePath));

            string[] content = File.ReadAllLines (Globals.logPath);

            if (content.Length > 16)
            {
                for (int i = content.Length - 1; i >= content.Length - 16; i--)
                {
                    log += content [i] + "<br/>";
                }
            }
            else
            {
                for (int i = content.Length - 1; i >= 0; i--)
                {
                    log += content [i] + "<br/>";
                }
            }

            Task.Run (CheckAuthCookie);
        }

        public void SavePrinters ()
        {
            foreach (PrinterObject printer in printers)
            {
                if (transparencies [printer.id] == true)
                {
                    printer.filament += "7E";
                }
            }

            Console.WriteLine ("Updated printers.json to " + JsonSerializer.Serialize (printers));
            ServerOutput.WriteLine ("Updated printers.json.", false);
            File.WriteAllText (printerFilePath, JsonSerializer.Serialize (printers));
            nav.NavigateTo (nav.Uri, true); // Reload page.
        }
    }
}