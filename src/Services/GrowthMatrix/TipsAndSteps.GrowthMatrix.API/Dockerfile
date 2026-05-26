FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching
COPY ["src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.API/TipsAndSteps.GrowthMatrix.API.csproj", "src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.API/"]
COPY ["src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.Application/TipsAndSteps.GrowthMatrix.Application.csproj", "src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.Application/"]
COPY ["src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.Domain/TipsAndSteps.GrowthMatrix.Domain.csproj", "src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.Domain/"]
COPY ["src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.Infrastructure/TipsAndSteps.GrowthMatrix.Infrastructure.csproj", "src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.Infrastructure/"]
COPY ["src/Shared/TipsAndSteps.Shared.Contracts/TipsAndSteps.Shared.Contracts.csproj", "src/Shared/TipsAndSteps.Shared.Contracts/"]
COPY ["src/Shared/TipsAndSteps.Shared.Infrastructure/TipsAndSteps.Shared.Infrastructure.csproj", "src/Shared/TipsAndSteps.Shared.Infrastructure/"]

RUN dotnet restore "src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.API/TipsAndSteps.GrowthMatrix.API.csproj"

# Copy the rest of the source
COPY . .
WORKDIR "/src/src/Services/GrowthMatrix/TipsAndSteps.GrowthMatrix.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.GrowthMatrix.API.dll"]

