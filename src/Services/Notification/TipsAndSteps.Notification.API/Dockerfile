FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching
COPY ["src/Services/Notification/TipsAndSteps.Notification.API/TipsAndSteps.Notification.API.csproj", "src/Services/Notification/TipsAndSteps.Notification.API/"]
COPY ["src/Services/Notification/TipsAndSteps.Notification.Application/TipsAndSteps.Notification.Application.csproj", "src/Services/Notification/TipsAndSteps.Notification.Application/"]
COPY ["src/Services/Notification/TipsAndSteps.Notification.Domain/TipsAndSteps.Notification.Domain.csproj", "src/Services/Notification/TipsAndSteps.Notification.Domain/"]
COPY ["src/Services/Notification/TipsAndSteps.Notification.Infrastructure/TipsAndSteps.Notification.Infrastructure.csproj", "src/Services/Notification/TipsAndSteps.Notification.Infrastructure/"]
COPY ["src/Shared/TipsAndSteps.Shared.Contracts/TipsAndSteps.Shared.Contracts.csproj", "src/Shared/TipsAndSteps.Shared.Contracts/"]
COPY ["src/Shared/TipsAndSteps.Shared.Infrastructure/TipsAndSteps.Shared.Infrastructure.csproj", "src/Shared/TipsAndSteps.Shared.Infrastructure/"]

RUN dotnet restore "src/Services/Notification/TipsAndSteps.Notification.API/TipsAndSteps.Notification.API.csproj"

# Copy the rest of the source
COPY . .
WORKDIR "/src/src/Services/Notification/TipsAndSteps.Notification.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.Notification.API.dll"]

