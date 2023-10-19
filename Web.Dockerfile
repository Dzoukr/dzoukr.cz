FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

# Install node & yarn
RUN set -uex; \
    apt-get update; \
    apt-get install -y ca-certificates curl gnupg; \
    mkdir -p /etc/apt/keyrings; \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key \
     | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg; \
    NODE_MAJOR=18; \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x nodistro main" \
     > /etc/apt/sources.list.d/nodesource.list; \
    apt-get update; \
    apt-get install nodejs -y;

RUN npm install -g yarn

WORKDIR /workspace
COPY . .
RUN dotnet tool restore

RUN dotnet run Publish


FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
COPY --from=build /workspace/publish/app /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT [ "dotnet", "DzoukrCz.Server.dll" ]