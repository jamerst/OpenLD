FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app
RUN dotnet tool install -g dotnet-ef
ENV PATH "$PATH:/root/.dotnet/tools"

RUN curl -sL https://deb.nodesource.com/setup_13.x |  bash -
RUN apt-get install -y nodejs

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
# ADD . ./
ENTRYPOINT dotnet watch run --urls "https://0.0.0.0:5000"

# Build runtime image
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "openld.dll"]
