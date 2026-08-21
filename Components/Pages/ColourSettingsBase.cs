using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace PageLogic
{
    public class ColourSettingsBase : PasswordProtectedPage
    {
        [Parameter]
        public int id { get; set; }
        private string filePath;
        public List<PrinterObject> printers;
        public string colourMode;
        public string dropdownValue;
        public List<string> colours = new List<string> ();
        
        public ColourSettingsBase ()
        {
            #if DEBUG
                filePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "printers.json");
            #else
                filePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "printers.json");   
            #endif

            try
            {
                printers = JsonSerializer.Deserialize<List<PrinterObject>> (File.ReadAllText (filePath));
            }
            catch
            {
                printers = new List<PrinterObject> ();
                ServerOutput.WriteLine ("[!] /data/printers.json not found!");
            }
        }

        public void UpdateColourMode ()
        {      
            if (dropdownValue == "multi")
            {
                colourMode = "multi";
                
                if (printers [id].filaments == null)
                {
                    colours = new List<string> {printers [id].filament}; // Add a colour if switching from single to multicolour mode.
                }
                else if (printers [id].filaments.Length == 0)
                {
                    colours = new List<string> {printers [id].filament};
                }
            }
            else
            {
                colourMode = "single";
            }

            StateHasChanged ();
        }

        public void AddColour ()
        {
            colours.Add ("#586497");
            StateHasChanged ();
        }

        public void UpdateColour (int index, string colour)
        {
            colours [index] = colour;
            StateHasChanged ();
        }

        public void RemoveColour (int index)
        {
            colours.RemoveAt (index);
            StateHasChanged ();
        }

        public void Save ()
        {
            if (colourMode == "multi")
            {
                printers [id].filaments = colours.ToArray<string> ();
            }
            else
            {
                printers [id].filaments = null;    
            }

            Console.WriteLine ("Updated printers.json to " + JsonSerializer.Serialize (printers));
            ServerOutput.WriteLine ("Updated printers.json.", false);
            File.WriteAllText (filePath, JsonSerializer.Serialize (printers));
            nav.NavigateTo (nav.Uri, true); // Reload page.
        }

        protected override async Task OnInitializedAsync ()
        {
            await Task.Run (CheckAuthCookieNoRedirect);

            if (printers [id].filaments == null || printers [id].filaments.Length == 0)
            {
                colourMode = "single";
            }
            else
            {
                colourMode = "multi";

                foreach (string colour in printers [id].filaments)
                {
                    colours.Add (colour);
                }
            }
        }
    }
}