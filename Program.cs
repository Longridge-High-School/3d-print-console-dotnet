using _3d_print_console_dotnet.Components;

Console.WriteLine ("************************");
Console.WriteLine ("* 3D PRINT CONSOLE.NET *");
Console.WriteLine ("************************\n");
Console.WriteLine ("Version: v0.1.0 using 3D Print Console v2.2.0");
Console.WriteLine ("Copyright (C) Longridge High School 2025");
Console.WriteLine ("Licensed under the whatever license.\n");

EnvVars.Load ();
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
