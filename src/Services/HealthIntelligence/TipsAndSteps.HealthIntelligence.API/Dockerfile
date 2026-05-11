FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.API/TipsAndSteps.HealthIntelligence.API.csproj"
WORKDIR "/src/src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.API"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.HealthIntelligence.API.dll"]
