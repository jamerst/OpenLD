FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

RUN curl -sL https://deb.nodesource.com/setup_13.x |  bash -
RUN apt-get install -y nodejs unzip

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
ADD . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
RUN apt-get update
RUN curl -sL https://deb.nodesource.com/setup_13.x |  bash -
RUN apt-get install -y nodejs unzip libc6-dev libgdiplus librsvg2-bin
RUN npm i -g svgo --unsafe-perm=true --allow-root
COPY init-data/openld-data /openld-data
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "openld.dll"]