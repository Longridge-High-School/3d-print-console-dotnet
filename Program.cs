using _3d_print_console_dotnet.Components;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.AspNetCore.Components.Server;

Console.WriteLine ("*****************************");
Console.WriteLine ("* 3D PRINT CONSOLE FOR .NET *");
Console.WriteLine ("*****************************\n");
Console.WriteLine ("Version: v0.2.0-beta");
Console.WriteLine ("Copyright (C) Longridge High School 2026");
Console.WriteLine ("Licensed under the M.I.T license.\n");

try
{
    Globals.LoadFromEnvironment ();

    if (File.Exists ("3d-print-console.cfg"))
    {
        Globals.LoadFromFile ();
    }

    ServerOutput.WriteLine ("Settings loaded!");
    ServerOutput.WriteLine ("Starting 3D Print Console for .NET...");

    if (Globals.GetString ("ADMIN_PASSWORD") == "")
    {
        Globals.Set ("ADMIN_PASSWORD", "admin");
        ServerOutput.WriteLine ("[!] Admin password not set! Defaulting to \"admin\" which is insecure!");
    }

    if (!File.Exists (Globals.GetString ("LOG_PATH")))
    {
        try
        {
            File.Create (Globals.GetString ("LOG_PATH"));   
        }
        catch
        {
            ServerOutput.WriteLine ("[!] Could not create a log file at " + Globals.GetString ("LOG_PATH") + "!");
        }
    }

    if (!Globals.GetBool ("USE_HTTP") && !Globals.GetBool ("USE_HTTPS"))
    {
        ServerOutput.WriteLine ("[!] ERROR: Variables \"USE_HTTP\" and \"USE_HTTPS\" are both set to false. Stopping...");
        Environment.Exit (1);
    }

    var builder = WebApplication.CreateBuilder (args);

    if (Globals.GetBool ("USING_PROXY"))
    {
        builder.Services.Configure  <ForwardedHeadersOptions> (options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownProxies.Add (IPAddress.Parse (Globals.GetString ("PROXY_IP")));
        });
    }

    #if !DEBUG

        builder.WebHost.ConfigureKestrel ((context, serverOptions) =>
        {
            if (Globals.GetBool ("USE_HTTP"))
            {
                serverOptions.Listen (IPAddress.Any, Globals.GetInt ("HTTP_PORT"));
            }

            if (Globals.GetBool ("USE_HTTPS"))
            {
                serverOptions.Listen (IPAddress.Any, Globals.GetInt ("HTTPS_PORT"), listenOptions =>
                {
                    listenOptions.UseHttps (Globals.GetString ("SSL_PATH"), Globals.GetString ("SSL_PASSWORD"));
                });
            }
        });

    #endif

    // Add services to the container.
    builder.Services.AddRazorComponents ().AddInteractiveServerComponents ();


    builder.Services.AddHttpContextAccessor ();
    builder.Services.AddSingleton <Cache> ();
    builder.WebHost.UseStaticWebAssets ();

    builder.Services.AddSignalR (options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromMinutes (30);
    });

    if (RuntimeInformation.IsOSPlatform (OSPlatform.Windows))
    {
        builder.Services.AddWindowsService ();
    }

    var app = builder.Build();

    app.UseForwardedHeaders();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment ())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception error)
{
    ServerOutput.WriteLine ("[!] ERROR: " + error.ToString ());
    Environment.Exit (1);
}
