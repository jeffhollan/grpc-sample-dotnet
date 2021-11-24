# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app/https-frontend

# Copy csproj and restore as distinct layers
COPY ./https-frontend/https-frontend.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./https-frontend/ ./
COPY ./protos/ ../protos/
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/https-frontend/out .
ENTRYPOINT ["dotnet", "https-frontend.dll"]