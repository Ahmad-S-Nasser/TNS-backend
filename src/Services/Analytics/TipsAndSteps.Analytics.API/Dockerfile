FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching
COPY ["src/Services/Analytics/TipsAndSteps.Analytics.API/TipsAndSteps.Analytics.API.csproj", "src/Services/Analytics/TipsAndSteps.Analytics.API/"]
COPY ["src/Services/Analytics/TipsAndSteps.Analytics.Application/TipsAndSteps.Analytics.Application.csproj", "src/Services/Analytics/TipsAndSteps.Analytics.Application/"]
COPY ["src/Services/Analytics/TipsAndSteps.Analytics.Domain/TipsAndSteps.Analytics.Domain.csproj", "src/Services/Analytics/TipsAndSteps.Analytics.Domain/"]
COPY ["src/Services/Analytics/TipsAndSteps.Analytics.Infrastructure/TipsAndSteps.Analytics.Infrastructure.csproj", "src/Services/Analytics/TipsAndSteps.Analytics.Infrastructure/"]
COPY ["src/Shared/TipsAndSteps.Shared.Contracts/TipsAndSteps.Shared.Contracts.csproj", "src/Shared/TipsAndSteps.Shared.Contracts/"]
COPY ["src/Shared/TipsAndSteps.Shared.Infrastructure/TipsAndSteps.Shared.Infrastructure.csproj", "src/Shared/TipsAndSteps.Shared.Infrastructure/"]

RUN dotnet restore "src/Services/Analytics/TipsAndSteps.Analytics.API/TipsAndSteps.Analytics.API.csproj"

# Copy the rest of the source
COPY . .
WORKDIR "/src/src/Services/Analytics/TipsAndSteps.Analytics.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.Analytics.API.dll"]

