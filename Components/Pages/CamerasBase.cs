using System.Text.Json;

namespace PageLogic
{
    public class CamerasBase : PasswordProtectedPage
    {
        private string filePath;
        public List<CameraObject> cameras;
        
        public CamerasBase ()
        {
            #if DEBUG
                filePath = System.IO.Path.Combine (System.IO.Directory.GetCurrentDirectory (), "wwwroot", "data", "cameras.json");
            #else
                filePath = System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "cameras.json");   
            #endif

            cameras = JsonSerializer.Deserialize<List<CameraObject>> (File.ReadAllText (filePath));

            Task.Run (CheckAuthCookie);
        }

        public void Save ()
        {
            Console.WriteLine ("Updated cameras.json to " + JsonSerializer.Serialize (cameras));
            File.WriteAllText (filePath, JsonSerializer.Serialize (cameras));
        }

        public void AddNewCamera ()
        {
            CameraObject newCamera = new CameraObject ();
            newCamera.title = "New Camera";
            newCamera.url = "http://example.com/stream";

            cameras.Add (newCamera);
            StateHasChanged ();
        }

        public void DeleteCamera (string title, string url)
        {
            foreach (CameraObject camera in cameras)
            {
                if (camera.title == title && camera.url == url)
                {
                    cameras.Remove (camera);
                    StateHasChanged ();
                    return;
                }
            }
        }
    }
}