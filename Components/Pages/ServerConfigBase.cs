namespace PageLogic
{
    public class ServerConfigBase : PasswordProtectedPage
    {
        public string config = "";

        public ServerConfigBase ()
        {
            if (File.Exists ("3d-print-console.cfg"))
            {
                config = File.ReadAllText ("3d-print-console.cfg");
            }
        }

        protected override async Task OnInitializedAsync ()
        {
            await Task.Run (CheckAuthCookie);
        }

        public void Save ()
        {
            File.WriteAllText ("3d-print-console.cfg", config);
        }
    }
}