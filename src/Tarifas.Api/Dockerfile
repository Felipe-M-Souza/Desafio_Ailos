FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5011

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Tarifas.Api/Tarifas.Api.csproj", "src/Tarifas.Api/"]
COPY ["src/Tarifas.Api/", "src/Tarifas.Api/"]

# Restore dependencies
RUN dotnet restore "src/Tarifas.Api/Tarifas.Api.csproj"

# Build application
WORKDIR "/src/src/Tarifas.Api"
RUN dotnet build "Tarifas.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tarifas.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create data directory
RUN mkdir -p /app/data

ENTRYPOINT ["dotnet", "Tarifas.Api.dll"]
