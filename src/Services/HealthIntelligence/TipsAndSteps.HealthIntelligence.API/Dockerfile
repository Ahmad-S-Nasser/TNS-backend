FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching
COPY ["src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.API/TipsAndSteps.HealthIntelligence.API.csproj", "src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.API/"]
COPY ["src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.Application/TipsAndSteps.HealthIntelligence.Application.csproj", "src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.Application/"]
COPY ["src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.Domain/TipsAndSteps.HealthIntelligence.Domain.csproj", "src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.Domain/"]
COPY ["src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.Infrastructure/TipsAndSteps.HealthIntelligence.Infrastructure.csproj", "src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.Infrastructure/"]
COPY ["src/Shared/TipsAndSteps.Shared.Contracts/TipsAndSteps.Shared.Contracts.csproj", "src/Shared/TipsAndSteps.Shared.Contracts/"]
COPY ["src/Shared/TipsAndSteps.Shared.Infrastructure/TipsAndSteps.Shared.Infrastructure.csproj", "src/Shared/TipsAndSteps.Shared.Infrastructure/"]

RUN dotnet restore "src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.API/TipsAndSteps.HealthIntelligence.API.csproj"

# Copy the rest of the source
COPY . .
WORKDIR "/src/src/Services/HealthIntelligence/TipsAndSteps.HealthIntelligence.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.HealthIntelligence.API.dll"]

