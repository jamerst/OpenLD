FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
RUN dotnet tool install -g dotnet-ef --version 3.1.0
ENV PATH "$PATH:/root/.dotnet/tools"

RUN curl -sL https://deb.nodesource.com/setup_13.x |  bash -
RUN apt-get install -y nodejs unzip libc6-dev libgdiplus librsvg2-bin
RUN npm i -g svgo --unsafe-perm=true --allow-root
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

RUN mkdir -p /openld-data/fixture-images /openld-data/fixture-symbols /openld-data/fixture-symbol-bitmaps

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

ENTRYPOINT dotnet watch run --urls "https://0.0.0.0:5000"