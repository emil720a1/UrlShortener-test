FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UrlShortener.Web/UrlShortener.Web.csproj", "UrlShortener.Web/"]
COPY ["UrlShortener.Application/UrlShortener.Application.csproj", "UrlShortener.Application/"]
COPY ["UrlShortener.Domain/UrlShortener.Domain.csproj", "UrlShortener.Domain/"]
COPY ["UrlShortener.Shared/UrlShortener.Shared.csproj", "UrlShortener.Shared/"]
COPY ["UrlShortener.Infrastructure/UrlShortener.Infrastructure.csproj", "UrlShortener.Infrastructure/"]
RUN dotnet restore "UrlShortener.Web/UrlShortener.Web.csproj"
COPY . .
WORKDIR "/src/UrlShortener.Web"
RUN dotnet build "./UrlShortener.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UrlShortener.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.Web.dll"]
