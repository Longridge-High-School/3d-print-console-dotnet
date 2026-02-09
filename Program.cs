using _3d_print_console_dotnet.Components;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Runtime.InteropServices;
using System.IO.Compression;

Console.WriteLine ("************************");
Console.WriteLine ("* 3D PRINT CONSOLE.NET *");
Console.WriteLine ("************************\n");
Console.WriteLine ("Version: v0.0.3-alpha");
Console.WriteLine ("Copyright (C) Longridge High School 2026");
Console.WriteLine ("Licensed under the M.I.T license.\n");

ServerOutput.WriteLine ("Starting 3D Print Console .NET...");
ServerOutput.WriteLine ("Loading settings from environment variables...");
Globals.LoadFromEnvironment ();
ServerOutput.WriteLine ("Settings loaded from environment variables.", false);

if (File.Exists ("3d-print-console.cfg"))
{
    ServerOutput.WriteLine ("Loading settings from 3d-print-console.cfg...");
    Globals.LoadFromFile ();
    ServerOutput.WriteLine ("Settings loaded from 3d-print-console.cfg.");
}

if (Globals.password == "")
{
    Globals.password = "admin";
    ServerOutput.WriteLine ("[!] Admin password not set! Defaulting to \"admin\" which is insecure!");
}

if (!File.Exists (Globals.logPath))
{
    try
    {
        File.Create (Globals.logPath);   
    }
    catch
    {
        ServerOutput.WriteLine ("[!] Could not create a log file at " + Globals.logPath + "!");
    }
}

var builder = WebApplication.CreateBuilder (args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor ();
builder.Services.AddSingleton <Cache> ();
builder.WebHost.UseStaticWebAssets();

if (RuntimeInformation.IsOSPlatform (OSPlatform.Windows))
{
    builder.Services.AddWindowsService ();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
