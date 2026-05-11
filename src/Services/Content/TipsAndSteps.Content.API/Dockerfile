FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Content/TipsAndSteps.Content.API/TipsAndSteps.Content.API.csproj", "src/Services/Content/TipsAndSteps.Content.API/"]
COPY ["src/Services/Content/TipsAndSteps.Content.Application/TipsAndSteps.Content.Application.csproj", "src/Services/Content/TipsAndSteps.Content.Application/"]
COPY ["src/Services/Content/TipsAndSteps.Content.Domain/TipsAndSteps.Content.Domain.csproj", "src/Services/Content/TipsAndSteps.Content.Domain/"]
COPY ["src/Services/Content/TipsAndSteps.Content.Infrastructure/TipsAndSteps.Content.Infrastructure.csproj", "src/Services/Content/TipsAndSteps.Content.Infrastructure/"]
COPY ["src/Shared/TipsAndSteps.Shared.Contracts/TipsAndSteps.Shared.Contracts.csproj", "src/Shared/TipsAndSteps.Shared.Contracts/"]
COPY ["src/Shared/TipsAndSteps.Shared.Infrastructure/TipsAndSteps.Shared.Infrastructure.csproj", "src/Shared/TipsAndSteps.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Content/TipsAndSteps.Content.API/TipsAndSteps.Content.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Content/TipsAndSteps.Content.API"
RUN dotnet build -c Release -o /app/build
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.Content.API.dll"]
