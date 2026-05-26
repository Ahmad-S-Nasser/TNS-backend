FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching
COPY ["src/Services/QA/TipsAndSteps.QA.API/TipsAndSteps.QA.API.csproj", "src/Services/QA/TipsAndSteps.QA.API/"]
COPY ["src/Services/QA/TipsAndSteps.QA.Application/TipsAndSteps.QA.Application.csproj", "src/Services/QA/TipsAndSteps.QA.Application/"]
COPY ["src/Services/QA/TipsAndSteps.QA.Domain/TipsAndSteps.QA.Domain.csproj", "src/Services/QA/TipsAndSteps.QA.Domain/"]
COPY ["src/Services/QA/TipsAndSteps.QA.Infrastructure/TipsAndSteps.QA.Infrastructure.csproj", "src/Services/QA/TipsAndSteps.QA.Infrastructure/"]
COPY ["src/Shared/TipsAndSteps.Shared.Contracts/TipsAndSteps.Shared.Contracts.csproj", "src/Shared/TipsAndSteps.Shared.Contracts/"]
COPY ["src/Shared/TipsAndSteps.Shared.Infrastructure/TipsAndSteps.Shared.Infrastructure.csproj", "src/Shared/TipsAndSteps.Shared.Infrastructure/"]

RUN dotnet restore "src/Services/QA/TipsAndSteps.QA.API/TipsAndSteps.QA.API.csproj"

# Copy the rest of the source
COPY . .
WORKDIR "/src/src/Services/QA/TipsAndSteps.QA.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.QA.API.dll"]

