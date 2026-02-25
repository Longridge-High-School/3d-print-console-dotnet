if (!(Get-Module -ListAvailable -Name 7Zip4PowerShell))
{
    Install-Module -Name 7Zip4PowerShell -Scope CurrentUser
}

$publish = "C:\LHS\3d-print-console-dotnet\bin\Release\net8.0\publish"
rm "$publish\wwwroot\console.html"
rm "$publish\wwwroot\3dprintconsole.webmanifest"
rm "$publish\wwwroot\data" -r -force
rm "$publish\wwwroot\img" -r -force
rm "$publish\wwwroot\css" -r -force
rm "$publish\wwwroot\js" -r -force

Compress-7Zip -Path "$publish" -ArchiveFileName ".\3d-print-console-dotnet.zip" -Format Zip