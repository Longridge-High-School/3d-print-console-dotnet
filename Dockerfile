FROM mcr.microsoft.com/dotnet/sdk:8.0

EXPOSE 5000

RUN mkdir /opt/3d-print-console-dotnet
RUN mkdir /tmp/src
WORKDIR /tmp/src
ADD . /tmp/src
RUN dotnet publish
RUN rm "./bin/Release/net8.0/publish/wwwroot/console.html" -f
RUN rm "./bin/Release/net8.0/publish/wwwroot/3dprintconsole.webmanifest" -f
RUN rm "./bin/Release/net8.0/publish/wwwroot/data" -r -f
RUN rm "./bin/Release/net8.0/publish/wwwroot/img" -r -f
RUN rm "./bin/Release/net8.0/publish/wwwroot/css" -r -f
RUN rm "./bin/Release/net8.0/publish/wwwroot/js" -r -f
RUN mv ./bin/Release/net8.0/publish/* /opt/3d-print-console-dotnet

WORKDIR /opt/3d-print-console-dotnet
RUN touch ./log.txt
RUN rm -rf /tmp/src

CMD ["dotnet", "/opt/3d-print-console-dotnet/3d-print-console-dotnet.dll"]