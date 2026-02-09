using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace PageLogic
{
    public class PasswordProtectedPage : ComponentBase
    {
        [Inject] public IJSRuntime javascript {get; set;}
        [Inject] public NavigationManager nav {get; set;}
        [Inject] public Cache cache {get; set;}
        public bool loggedIn = false;

        public async Task CheckAuthCookie ()
        {
            try
            {
                string token = await javascript.InvokeAsync<string> ("GetCookie");
                
                if (token == "" || token == null)
                {
                    Console.WriteLine ("No access_token cookie!");
                    nav.NavigateTo ("/login", true);
                    return;
                }
                else
                {
                    string? expiry = cache.Get (token);

                    if (expiry == null)
                    {
                        Console.WriteLine ("access_token value not found in cache!");
                        nav.NavigateTo ("/login", true);
                        return;
                    }
                    else
                    {
                        if (DateTime.ParseExact (expiry, "ddd, dd MMM yyyy HH:mm:ss UTC", CultureInfo.InvariantCulture) < DateTime.UtcNow)
                        {
                            Console.WriteLine ("access_token too old!");
                            cache.Delete (token);
                            nav.NavigateTo ("/login", true);
                            return;
                        }
                        else
                        {
                            loggedIn = true;
                            await InvokeAsync (StateHasChanged);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                //Console.WriteLine (error.ToString ());
                return;
            }
        }
    }
}