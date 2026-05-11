FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Gateway/TipsAndSteps.Gateway/TipsAndSteps.Gateway.csproj", "src/Gateway/TipsAndSteps.Gateway/"]
RUN dotnet restore "src/Gateway/TipsAndSteps.Gateway/TipsAndSteps.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Gateway/TipsAndSteps.Gateway"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TipsAndSteps.Gateway.dll"]
