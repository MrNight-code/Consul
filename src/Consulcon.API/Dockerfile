# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["scripts/", "/scripts/"]
COPY ["src/Consulcon.API/Consulcon.API.csproj", "Consulcon.API/"]
COPY ["src/Consulcon.Application/Consulcon.Application.csproj", "Consulcon.Application/"]
COPY ["src/Consulcon.Domain/Consulcon.Domain.csproj", "Consulcon.Domain/"]
COPY ["src/Consulcon.Infrastructure/Consulcon.Infrastructure.csproj", "Consulcon.Infrastructure/"]
RUN dotnet restore "./Consulcon.API/Consulcon.API.csproj"
COPY src/ .
WORKDIR "/src/Consulcon.API"
RUN dotnet build "./Consulcon.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Consulcon.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy migrations folder for TenantMigrationService
COPY --from=build /scripts/database/migrations /app/migrations
ENTRYPOINT ["dotnet", "Consulcon.API.dll"]
