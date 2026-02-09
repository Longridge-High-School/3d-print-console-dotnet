FROM mcr.microsoft.com/dotnet/aspnet:8.0

EXPOSE 8080

RUN mkdir /opt/3d-print-console-dotnet
COPY ./bin/Release/net8.0/publish/  /opt/3d-print-console-dotnet
WORKDIR /opt/3d-print-console-dotnet

CMD ["dotnet", "/opt/3d-print-console-dotnet/3d-print-console-dotnet.dll"]