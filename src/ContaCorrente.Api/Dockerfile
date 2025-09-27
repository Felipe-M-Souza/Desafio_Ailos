FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ContaCorrente.Api/ContaCorrente.Api.csproj", "src/ContaCorrente.Api/"]
COPY ["src/ContaCorrente.Application/ContaCorrente.Application.csproj", "src/ContaCorrente.Application/"]
COPY ["src/ContaCorrente.Domain/ContaCorrente.Domain.csproj", "src/ContaCorrente.Domain/"]
COPY ["src/ContaCorrente.Infrastructure/ContaCorrente.Infrastructure.csproj", "src/ContaCorrente.Infrastructure/"]
RUN dotnet restore "src/ContaCorrente.Api/ContaCorrente.Api.csproj"
COPY . .
WORKDIR "/src/src/ContaCorrente.Api"
RUN dotnet build "ContaCorrente.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContaCorrente.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContaCorrente.Api.dll"]


