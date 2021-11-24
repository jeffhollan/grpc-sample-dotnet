# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app/grpc-backend

# Copy csproj and restore as distinct layers
COPY ./grpc-backend/grpc-backend.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./grpc-backend/ ./
COPY ./protos/ ../protos/
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/grpc-backend/out .
EXPOSE 8085
ENTRYPOINT ["dotnet", "grpc-backend.dll"]