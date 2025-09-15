# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy source code and project files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]
COPY ["src/DzoukrCz.MoonServer/", "DzoukrCz.MoonServer/"]
COPY ["src/DzoukrCz.Libraries/", "DzoukrCz.Libraries/"]

# Restore packages
RUN dotnet restore "DzoukrCz.MoonServer/DzoukrCz.MoonServer.fsproj"

# Build and publish
WORKDIR "/src/DzoukrCz.MoonServer"
RUN dotnet publish "DzoukrCz.MoonServer.fsproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Copy the published app
COPY --from=build /app/publish .

# Environment variables for configuration
ENV StorageAccount=""
ENV TableName="MoonServerMedia"
ENV ContainerName="$web"
ENV PathPrefix="https://media.dzoukr.cz/"
ENV ApiKey=""
ENV ApiSecret=""

ENTRYPOINT ["dotnet", "DzoukrCz.MoonServer.dll"]
