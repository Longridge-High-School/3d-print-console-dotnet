using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PageLogic
{
    public class PasswordProtectedPage : ComponentBase
    {
        [Inject] private IJSRuntime javascript {get; set;}
        [Inject] public NavigationManager nav {get; set;}
        
        public async Task CheckAuthCookie ()
        {
            string token = await javascript.InvokeAsync<string> ("GetCookie");
            
            if (token == "" || token == null)
            {
                Console.WriteLine ("No access_token cookie!");
                nav.NavigateTo ("/login", true);
            }
            else
            {
                string? expiry = Cache.Get (token);

                if (expiry == null)
                {
                    Console.WriteLine ("access_token value not found in cache!");
                    nav.NavigateTo ("/login", true);
                }
                else
                {
                    if (DateTime.Parse (expiry) < DateTime.UtcNow)
                    {
                        Console.WriteLine ("access_token too old!");
                        nav.NavigateTo ("/login", true);
                    }
                }
            }
        }
    }
}