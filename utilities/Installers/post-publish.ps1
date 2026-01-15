$publish = "C:\LHS\3d-print-console-dotnet\bin\Release\net8.0\publish"
rm "$publish\wwwroot\console.html"
rm "$publish\wwwroot\3dprintconsole.webmanifest"
rm "$publish\wwwroot\data" -r -force
rm "$publish\wwwroot\img" -r -force
rm "$publish\wwwroot\css" -r -force
rm "$publish\wwwroot\js" -r -force