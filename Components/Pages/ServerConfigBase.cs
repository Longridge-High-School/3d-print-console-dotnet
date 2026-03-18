using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using System.Web;

namespace PageLogic
{
    public class ServerConfigBase : PasswordProtectedPage
    {
        public string adminPassword = "";
        public string logPath = Globals.GetString ("LOG_PATH");
        public bool recordServerLogs = Globals.GetBool ("RECORD_SERVER_LOGS");
        public bool usingProxy = Globals.GetBool ("USING_PROXY");
        public string proxyIP = Globals.GetString ("PROXY_IP").Replace (",", ", ");
        public int httpPort = Globals.GetInt ("HTTP_PORT");
        public int httpsPort = Globals.GetInt ("HTTPS_PORT");
        public bool useHTTP = Globals.GetBool ("USE_HTTP");
        public bool useHTTPS = Globals.GetBool ("USE_HTTPS");
        public string sslPath = Globals.GetString ("SSL_PATH");
        public string certName = "None";
        public byte [] cert = [];
        public string certPassword = "";

        protected override async Task OnInitializedAsync ()
        {
            await Task.Run (CheckAuthCookie);
        }

        public void Save ()
        {
            Globals.Set ("LOG_PATH", logPath);
            Globals.Set ("RECORD_SERVER_LOGS", recordServerLogs);
            Globals.Set ("USING_PROXY", usingProxy);
            Globals.Set ("PROXY_IP", proxyIP.Replace (" ", ""));
            Globals.Set ("HTTP_PORT", httpPort);
            Globals.Set ("HTTPS_PORT", httpsPort);
            Globals.Set ("USE_HTTP", useHTTP);
            Globals.Set ("USE_HTTPS", useHTTPS);

            Globals.Write ();
            StateHasChanged ();
        }

        public async void ChangeAdminPassword ()
        {
            Globals.Set ("ADMIN_PASSWORD", adminPassword);
            Globals.Write ();
            await javascript.InvokeVoidAsync ("LogOut");
        }

        public async Task UploadCert (InputFileChangeEventArgs input)
        {
            certName = HttpUtility.HtmlEncode (input.File.Name);

            using MemoryStream memoryStream = new MemoryStream ();
            await input.File.OpenReadStream ().CopyToAsync (memoryStream);
            cert = memoryStream.ToArray ();
        }

        public void SaveCert ()
        {
            #if DEBUG
                string certPath = Path.Combine (Directory.GetCurrentDirectory (), "certs");
            #else
                string certPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "certs");   
            #endif

            Directory.CreateDirectory (certPath);

            File.WriteAllBytes (Path.Combine (certPath, certName), cert);

            Globals.Set ("SSL_PATH", Path.Combine (certPath, certName));
            Globals.Set ("SSL_PASSWORD", certPassword);

            Globals.Write ();

            certName = "None";
            cert = [];
            certPassword = "";

            StateHasChanged ();
        }
    }
}