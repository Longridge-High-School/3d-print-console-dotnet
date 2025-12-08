namespace PageLogic
{
    public class LogViewerBase : PasswordProtectedPage
    {
        public string log = "";

        public LogViewerBase ()
        {
            string[] content = File.ReadAllLines (Globals.logPath);

            for (int i = content.Length - 1; i >= 0; i--)
            {
                log += content [i] + "<br/>";
            }

            Task.Run (CheckAuthCookie);
        }

        public void Reload ()
        {
            nav.NavigateTo (nav.Uri, true);
        }
    }
}