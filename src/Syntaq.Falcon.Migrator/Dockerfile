#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Syntaq.Falcon.Migrator/Syntaq.Falcon.Migrator.csproj", "src/Syntaq.Falcon.Migrator/"]
COPY ["src/Syntaq.Falcon.EntityFrameworkCore/Syntaq.Falcon.EntityFrameworkCore.csproj", "src/Syntaq.Falcon.EntityFrameworkCore/"]
COPY ["src/Syntaq.Falcon.Core/Syntaq.Falcon.Core.csproj", "src/Syntaq.Falcon.Core/"]
COPY ["src/Syntaq.Falcon.Core.Shared/Syntaq.Falcon.Core.Shared.csproj", "src/Syntaq.Falcon.Core.Shared/"]
RUN dotnet restore "src/Syntaq.Falcon.Migrator/Syntaq.Falcon.Migrator.csproj"
COPY . .
WORKDIR "/src/src/Syntaq.Falcon.Migrator"
RUN dotnet build "Syntaq.Falcon.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Syntaq.Falcon.Migrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Syntaq.Falcon.Migrator.dll", "-s"]
