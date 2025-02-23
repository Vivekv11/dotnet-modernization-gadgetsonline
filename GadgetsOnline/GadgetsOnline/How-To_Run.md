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

cleared docker ignore file also to make it work.

Docker build command:

docker build -f "C:\Users\vdabhi\source\repos\Vivekv11\gadgetsonline-MT\GadgetsOnline\GadgetsOnline\Dockerfile" -t gadgetsonline1:dev1 "C:\Users\vdabhi\source\repos\Vivekv11\gadgetsonline-MT\GadgetsOnline\GadgetsOnline"

Docker run:

docker run -dt --add-host=sql-server:<IP> --name GadgetsOnline -p 59872:80 gadgetsonline1:dev1
docker run -dt --add-host=sql-server:<IP> --name GadgetsOnline -e "DB_CONNECTION_STRING=Server=<SERVER>;Database=gadgetsonlinedb;User Id=sa;Password=<PASS>;TrustServerCertificate=True;Connection Timeout=60;" -p 59872:80 gadgetsonline1:dev1
