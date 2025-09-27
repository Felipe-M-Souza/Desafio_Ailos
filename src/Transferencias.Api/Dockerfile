FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Transferencias.Api/Transferencias.Api.csproj", "src/Transferencias.Api/"]
COPY ["src/Transferencias.Api/", "src/Transferencias.Api/"]

# Restore dependencies
RUN dotnet restore "src/Transferencias.Api/Transferencias.Api.csproj"

# Build application
WORKDIR "/src/src/Transferencias.Api"
RUN dotnet build "Transferencias.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Transferencias.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create data directory
RUN mkdir -p /app/data

ENTRYPOINT ["dotnet", "Transferencias.Api.dll"]
