#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Syntaq.Falcon.Web.Host/Syntaq.Falcon.Web.Host.csproj", "src/Syntaq.Falcon.Web.Host/"]
COPY ["src/Syntaq.Falcon.Web.Core/Syntaq.Falcon.Web.Core.csproj", "src/Syntaq.Falcon.Web.Core/"]
COPY ["src/Syntaq.Falcon.Application/Syntaq.Falcon.Application.csproj", "src/Syntaq.Falcon.Application/"]
COPY ["src/Syntaq.Falcon.Application.Shared/Syntaq.Falcon.Application.Shared.csproj", "src/Syntaq.Falcon.Application.Shared/"]
COPY ["src/Syntaq.Falcon.Core.Shared/Syntaq.Falcon.Core.Shared.csproj", "src/Syntaq.Falcon.Core.Shared/"]
COPY ["src/Syntaq.Falcon.Core/Syntaq.Falcon.Core.csproj", "src/Syntaq.Falcon.Core/"]
COPY ["src/Syntaq.Falcon.EntityFrameworkCore/Syntaq.Falcon.EntityFrameworkCore.csproj", "src/Syntaq.Falcon.EntityFrameworkCore/"]
COPY ["src/Syntaq.Falcon.GraphQL/Syntaq.Falcon.GraphQL.csproj", "src/Syntaq.Falcon.GraphQL/"]
RUN dotnet restore "src/Syntaq.Falcon.Web.Host/Syntaq.Falcon.Web.Host.csproj"
COPY . .
WORKDIR "/src/src/Syntaq.Falcon.Web.Host"
RUN dotnet build "Syntaq.Falcon.Web.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Syntaq.Falcon.Web.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Syntaq.Falcon.Web.Host.dll"]
