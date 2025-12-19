using _3d_print_console_dotnet.Components;

Console.WriteLine ("************************");
Console.WriteLine ("* 3D PRINT CONSOLE.NET *");
Console.WriteLine ("************************\n");
Console.WriteLine ("Version: v1.0.0");
Console.WriteLine ("Copyright (C) Longridge High School 2025");
Console.WriteLine ("Licensed under the M.I.T license.\n");

ServerOutput.WriteLine ("Starting 3D Print Console .NET...", false);

if (File.Exists ("3d-print-console.cfg"))
{
    ServerOutput.WriteLine ("Loading settings from 3d-print-console.cfg...");
    Globals.LoadFromFile ();
}
else
{
    ServerOutput.WriteLine ("Loading settings from environment variables...");
    Globals.LoadFromEnvironment ();
}

Cache.Connect ();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor ();

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
