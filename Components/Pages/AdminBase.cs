using System.Text.Json;
using System.Net;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;

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

            try
            {
                printers = JsonSerializer.Deserialize<List<PrinterObject>> (File.ReadAllText (printerFilePath));
                
                foreach (PrinterObject printer in printers) // Set transparency values seperately because the HTML colour element does not support alpha channels???
                {
                    if (printer.filament.Length > 7) // If the string is too short, there is no alpha applied.
                    {
                        try
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
                        catch
                        {
                            ServerOutput.WriteLine ("[!] Failed to covert transparency on " + printer.name + ". Try manually setting the filament colour?");
                            transparencies.Add (printer.id, false);
                        }
                    }
                    else
                    {
                        transparencies.Add (printer.id, false);
                    }
                }
            }
            catch (Exception error)
            {
                printers = new List<PrinterObject> ();
                ServerOutput.WriteLine ("[!] /data/printers.json not found!");
                Console.WriteLine (error.ToString ());
            }

            try
            {
                cameras = JsonSerializer.Deserialize<List<CameraObject>> (File.ReadAllText (cameraFilePath));
            }
            catch
            {
                cameras = new List<CameraObject> ();
                ServerOutput.WriteLine ("[!] /data/cameras.json not found!");
            }

            try
            {
                string[] content = File.ReadAllLines (Globals.logPath);

                int logDisplayLength = 12 * printers.Count ();

                if (logDisplayLength == 0)
                {
                    logDisplayLength = 12;
                }

                if (content.Length > logDisplayLength)
                {
                    for (int i = content.Length - 1; i >= content.Length - logDisplayLength; i--)
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
            }
            catch
            {
                log = "‼️ No log file found!";
                ServerOutput.WriteLine ("[!] Log file not found!");
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

        public void UpdateConsole ()
        {
            #if DEBUG
                string data = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data");
                string wwwroot = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot");
            #else
                string data = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data");   
                string wwwroot = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot");   
            #endif

            try
            {
                File.Delete (Path.Combine (Path.GetTempPath (), "3dprintconsole.zip"));
                Directory.Delete (Path.Combine (Path.GetTempPath () + "3dprintconsole"), true);
            }
            catch
            {
            }

            try
            {
                ServerOutput.WriteLine ("Downloading 3D Print Console...");

                using (WebClient client = new WebClient ())
                {
                    client.DownloadFile ("https://github.com/Longridge-High-School/3d-print-console/archive/refs/heads/main.zip", Path.GetTempPath () + "3dprintconsole.zip");
                }

                ServerOutput.WriteLine ("Extracting archive...");
                ZipFile.ExtractToDirectory (Path.Combine (Path.GetTempPath (), "3dprintconsole.zip"), Path.Combine (Path.GetTempPath () + "3dprintconsole"));

                ServerOutput.WriteLine ("Copying files...");

                File.Copy (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "console", "index.html"), Path.Combine (wwwroot, "console.html"), true);
                File.Copy (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "console", "3dprintconsole.webmanifest"), Path.Combine (wwwroot, "3dprintconsole.webmanifest"), true);
                FileSystem.CopyDirectory (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "console", "css"), Path.Combine (wwwroot, "css"), true);
                FileSystem.CopyDirectory (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "console", "img"), Path.Combine (wwwroot, "img"), true);
                FileSystem.CopyDirectory (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "console", "js"), Path.Combine (wwwroot, "js"), true);

                if (!Directory.Exists (data))
                {
                    Directory.CreateDirectory (data);
                    Directory.CreateDirectory (Path.Combine (data, "widgets"));
                    File.Copy (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "default", "cameras.json"), Path.Combine (data, "cameras.json"));
                    File.Copy (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "default", "config.json"), Path.Combine (data, "config.json"));
                    File.Copy (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "default", "printers.json"), Path.Combine (data, "printers.json"));
                    File.Copy (Path.Combine (Path.GetTempPath (), "3dprintconsole", "3d-print-console-main", "default", "widgets.json"), Path.Combine (data, "widgets.json"));
                }

                ServerOutput.WriteLine ("Cleaning up...");
                File.Delete (Path.Combine (Path.GetTempPath (), "3dprintconsole.zip"));
                Directory.Delete (Path.Combine (Path.GetTempPath () + "3dprintconsole"), true);
            }
            catch (Exception error)
            {
                ServerOutput.WriteLine ("[!] Could not update 3D Print Console version!");
                Console.WriteLine (error.ToString ());
            }

            nav.NavigateTo (nav.Uri, true);
        }
    }
}