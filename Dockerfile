FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first so this layer is cached until a project file changes.
COPY src/FreeTrack.Domain/FreeTrack.Domain.csproj src/FreeTrack.Domain/
COPY src/FreeTrack.Application/FreeTrack.Application.csproj src/FreeTrack.Application/
COPY src/FreeTrack.Infrastructure/FreeTrack.Infrastructure.csproj src/FreeTrack.Infrastructure/
COPY src/FreeTrack.Web/FreeTrack.Web.csproj src/FreeTrack.Web/
RUN dotnet restore src/FreeTrack.Web/FreeTrack.Web.csproj

COPY src/ src/
RUN dotnet publish src/FreeTrack.Web/FreeTrack.Web.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
# Non-root user (defined by the official .NET 8 images). Render sets PORT; Program.cs binds to it.
USER $APP_UID
ENTRYPOINT ["dotnet", "FreeTrack.Web.dll"]
