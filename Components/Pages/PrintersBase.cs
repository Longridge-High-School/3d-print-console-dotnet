using System.Text.Json;

namespace PageLogic
{    
    public class PrintersBase : PasswordProtectedPage
    {
        private string filePath;
        public List<PrinterObject> printers;
        public Dictionary<string, bool> transparencies = new Dictionary<string, bool> ();

        public PrintersBase ()
        {         
            #if DEBUG
                filePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "printers.json");
            #else
                filePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "printers.json");   
            #endif
            try
            {
                printers = JsonSerializer.Deserialize<List<PrinterObject>> (File.ReadAllText (filePath));

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
            catch
            {
                printers = new List<PrinterObject> ();
                ServerOutput.WriteLine ("[!] /data/printers.json not found!");
            }

            Task.Run (CheckAuthCookie);
        }

        public void Save ()
        {
            foreach (PrinterObject printer in printers)
            {
                if (printer.managementURL == "")
                {
                    printer.managementURL = null; // Force managementURL to null if left blank.
                }

                if (transparencies [printer.id] == true)
                {
                    printer.filament += "7E";
                }
            }

            Console.WriteLine ("Updated printers.json to " + JsonSerializer.Serialize (printers));
            ServerOutput.WriteLine ("Updated printers.json.", false);
            File.WriteAllText (filePath, JsonSerializer.Serialize (printers));
            nav.NavigateTo (nav.Uri, true); // Reload page.
        }

        public void AddNewPrinter ()
        {
            Random randomiser = new Random ();
            PrinterObject newPrinter = new PrinterObject ();

            int max = 0;

            foreach (PrinterObject printer in printers)
            {
                int id = int.Parse (printer.id);

                if (id > max)
                {
                    max = id;
                }
            }

            newPrinter.id = (max + 1).ToString ();
            newPrinter.name = "My New Printer";
            newPrinter.host = "http://printer.example.com";
            newPrinter.filament = "#" + randomiser.Next (256).ToString ("X") + randomiser.Next (256).ToString ("X") + randomiser.Next (256).ToString ("X");
            newPrinter.key = "YOUR_OCTOPRINT_KEY_HERE";
            newPrinter.background = "#" + randomiser.Next (256).ToString ("X") + randomiser.Next (256).ToString ("X") + randomiser.Next (256).ToString ("X");
            newPrinter.port = "YOUR_OCTOPRINT_PORT_HERE";
            newPrinter.file = ".gcode";
            newPrinter.locked = false;

            printers.Add (newPrinter);
            transparencies.Add (newPrinter.id, false);
            StateHasChanged ();
        }

        public void DeletePrinter (string id)
        {
            foreach (PrinterObject printer in printers)
            {
                if (printer.id == id)
                {
                    printers.Remove (printer);
                    StateHasChanged ();
                    return;
                }
            }
        }


    }
}