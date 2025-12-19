using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using System.IO.Compression;

namespace PageLogic
{
    public class WidgetsBase : PasswordProtectedPage
    {
        private string filePath;
        public string widgetDirectory;
        public string[] directories = [];
        public List<WidgetObject> widgets;

        public WidgetsBase ()
        {
            #if DEBUG
                filePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "widgets.json");
                widgetDirectory = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "widgets");
            #else
                filePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "widgets.json");
                widgetDirectory = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "widgets");   
            #endif

            try
            {
                directories = Directory.GetDirectories (widgetDirectory);
            }
            catch
            {
                ServerOutput.WriteLine ("[!] /data/widgets/ directory not found!");
            }

            try
            {
                widgets = JsonSerializer.Deserialize<List<WidgetObject>> (File.ReadAllText (filePath));
            }
            catch (JsonException)
            {
                FixBools (); // Remove the "s from around the booleans. They were in there because JavaScript doesn't care. C#, however, does.
                widgets = JsonSerializer.Deserialize<List<WidgetObject>> (File.ReadAllText (filePath));
            }
            catch
            {
                widgets = new List<WidgetObject> ();
                ServerOutput.WriteLine ("[!] /data/widgets.json not found!");
            }
            
            Task.Run (CheckAuthCookie);
        }

        private void FixBools ()
        {
            string content = File.ReadAllText (filePath);
            content = content.Replace ("\"enabled\": \"true\"", "\"enabled\": true");
            content = content.Replace ("\"enabled\": \"false\"", "\"enabled\": false");
            content = content.Replace ("\"enabled\":\"true\"", "\"enabled\":true");
            content = content.Replace ("\"enabled\":\"false\"", "\"enabled\":false");
            File.WriteAllText (filePath, content);
        }

        public void Save ()
        {
            Console.WriteLine ("Updated widgets.json to " + JsonSerializer.Serialize (widgets));
            ServerOutput.WriteLine ("Updated widgets.json.", false);
            File.WriteAllText (filePath, JsonSerializer.Serialize (widgets));
            nav.NavigateTo (nav.Uri, true); // Reload page.
        }

        public void AddNewWidget ()
        {
            WidgetObject newWidget = new WidgetObject ();
            newWidget.title = "New Widget";
            newWidget.url = "/helloworld/hello.html";
            newWidget.args = "";
            newWidget.enabled = false;

            widgets.Add (newWidget);
            StateHasChanged ();
        }

        public void DeleteWidget (string title, string url)
        {
            foreach (WidgetObject widget in widgets)
            {
                if (widget.title == title && widget.url == url)
                {
                    widgets.Remove (widget);
                    StateHasChanged ();
                    return;
                }
            }
        }

        public async Task UploadAndExtract (InputFileChangeEventArgs input)
        {
            try
            {
                ServerOutput.WriteLine ("Uploading " + input.File.Name + " as widget...");
                FileStream archive = new FileStream (Path.GetTempPath () + input.File.Name, FileMode.Create, FileAccess.Write);
                await input.File.OpenReadStream ().CopyToAsync (archive);
                archive.Close ();

                ServerOutput.WriteLine ("Extracting archive...");
                ZipFile.ExtractToDirectory (Path.GetTempPath () + input.File.Name, Path.Combine (widgetDirectory, input.File.Name.Substring (0, input.File.Name.Length - 4)));

                File.Delete (Path.GetTempPath () + input.File.Name);
                ServerOutput.WriteLine ("Install Succesful!");
            }
            catch (Exception error)
            {
                ServerOutput.WriteLine ("Widget upload failed!");
                Console.WriteLine (error.ToString ());
                try
                {
                    await javascript.InvokeAsync<string>("alert", ["⚠️ Failed to process file."]);
                }
                catch
                {
                    
                }
            }

            nav.NavigateTo (nav.Uri, true);
        }

        public async void DeleteDirectory (string path)
        {
            try
            {
                ServerOutput.WriteLine ("Removing directory " + path + "...");
                Directory.Delete (path, true);
                ServerOutput.WriteLine ("Directory succesfully removed!");
                nav.NavigateTo (nav.Uri, true);
            }
            catch (Exception error)
            {
                try
                {
                    ServerOutput.WriteLine ("Deleting directory failed!");
                    Console.WriteLine (error.ToString ());
                    await javascript.InvokeAsync<string>("alert", ["⚠️ Failed to remove widget files."]);
                }
                catch
                {
                    
                }
            }
        }
    }
}