using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace PageLogic
{
    public class LoginBase : ComponentBase
    {
        [Inject] private IJSRuntime javascript {get; set;}
        [Inject] public NavigationManager nav {get; set;}
        public string userPassword = "";
        public string message = "&nbsp;";

        public LoginBase ()
        {
            Task.Run (CheckAuthCookie);
        }

        public void CheckPassword ()
        {
            if (userPassword == EnvVars.password)
            {
                string token = Guid.NewGuid ().ToString ();
                TimeSpan sixHours = new TimeSpan (6, 0, 0);
                string expiry = DateTime.Now.Add (sixHours).ToString ("ddd, dd MMM yyy HH:mm:ss UTC");
                javascript.InvokeVoidAsync ("SetCookie", token, expiry);
                Cache.Set (token, expiry);
            }
            else
            {
                userPassword = "";
                message = "Incorrect Password!";
            }
        }

        public async Task CheckAuthCookie ()
        {
            string token = await javascript.InvokeAsync<string> ("GetCookie");
            
            if (token != "" && token != null)
            {
                nav.NavigateTo ("/admin", true);
            }
        }
    }
}