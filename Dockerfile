FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build 
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore --use-current-runtime

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app --use-current-runtime --self-contained false --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/nightly/runtime:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "dotnetapp.dll"]