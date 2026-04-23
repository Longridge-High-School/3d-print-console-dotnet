# 3D Print Console for .NET

3D Print Console for .NET is 3D Print Console running on ASP.NET.

<img class = "ImageLarge" src = "https://longridge-high-school.github.io/3d-print-console/img/dotnet/console.jpg" alt = "The same 3D Print Console you know." />

**The same 3D Print Console you know.**

<img class = "ImageLarge" src = "https://longridge-high-school.github.io/3d-print-console/img/dotnet/admin_home.jpg" alt = "The new admin interface." />

**The new admin interface.**

---

## Features

- Password-protected admin user interface. No more having to change files via SFTP!
- Compatible with 3D Print Console v2.2.0 or later. Older versions should be compatible if you use a seperate log server.
- [An easy way of installing widgets](https://longridge-high-school.github.io/3d-print-console/dotnet/widgets#using-3d-print-console-for-net).
- Integrated log viewer.
- Built-in HTTPS, with no proxy needed.
- Runs on Windows and Docker.

## How it Works

3D Print Console for .NET replaces the HTTP server and Log Server of the original 3D Print Console with a whole new backend written in C# and running on ASP.NET. It also adds a full web-based UI for administrators so they don't need to mess about with SFTP or remoting into servers every time someone changes the filament. It uses the base 3D Print Console files for the main console page, app logic and data storage rather than a reimplementation to ensure that it is almost completely compatible with the original.

## Getting Started

For a Windows server, see the guide [here](https://longridge-high-school.github.io/3d-print-console/dotnet/setup/windows).

For other operating systems using Docker, see the guide [here](https://longridge-high-school.github.io/3d-print-console/dotnet/setup/docker).