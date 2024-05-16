#Dockerfile to publish Soccer League API

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SoccerLeague.API/SoccerLeague.API.csproj", "SoccerLeague.API/"]
COPY ["SoccerLeague.Repository/SoccerLeague.Repository.csproj", "SoccerLeague.Repository/"]
COPY ["SoccerLeague.Core/SoccerLeague.Core.csproj", "SoccerLeague.Core/"]
RUN dotnet restore "./SoccerLeague.API/SoccerLeague.API.csproj"
COPY . .
WORKDIR "/src/SoccerLeague.API"
RUN dotnet build "./SoccerLeague.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SoccerLeague.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SoccerLeague.API.dll"]