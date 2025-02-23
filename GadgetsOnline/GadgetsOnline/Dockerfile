# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
#ARG source
ARG source=bin/app.publish
WORKDIR /inetpub/wwwroot
#COPY ${source:-obj/Docker/publish} .
COPY ${source} .
# Remove development-specific settings and debugger
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
