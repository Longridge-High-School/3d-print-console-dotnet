using _3d_print_console_dotnet.Components;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

Console.WriteLine ("************************");
Console.WriteLine ("* 3D PRINT CONSOLE.NET *");
Console.WriteLine ("************************\n");
Console.WriteLine ("Version: v0.1.0-beta");
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
    ServerOutput.WriteLine ("Starting 3D Print Console .NET...");

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

    var builder = WebApplication.CreateBuilder (args);

    /*#if !DEBUG

        builder.WebHost.ConfigureKestrel ((context, serverOptions) =>
        {
            serverOptions.Listen (IPAddress.Any, 5000);

            serverOptions.Listen (IPAddress.Any, 5001, listenOptions =>
            {
                listenOptions.UseHttps ("testCert.pfx", "testPassword");
            });
        });

    #endif*/

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    builder.Services.AddHttpContextAccessor ();
    builder.Services.AddSingleton <Cache> ();
    builder.WebHost.UseStaticWebAssets();

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
