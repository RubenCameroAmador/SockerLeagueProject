#Dockerfile to publish Soccer League Client

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SoccerLeague.Client/SoccerLeague.Client.csproj", "SoccerLeague.Client/"]
COPY ["SoccerLeague.Core/SoccerLeague.Core.csproj", "SoccerLeague.Core/"]
RUN dotnet restore "./SoccerLeague.Client/SoccerLeague.Client.csproj"
COPY . .
WORKDIR "/src/SoccerLeague.Client"
RUN dotnet build "./SoccerLeague.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SoccerLeague.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa de ejecución
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Elimina los archivos predeterminados de NGINX
RUN rm -rf ./*

# Copia los archivos publicados desde la etapa de compilación
COPY --from=publish /app/publish/wwwroot .

RUN mv appsettings.docker.json appsettings.json

# Exponer el puerto 80
EXPOSE 80

# Inicia NGINX
ENTRYPOINT ["nginx", "-g", "daemon off;"]