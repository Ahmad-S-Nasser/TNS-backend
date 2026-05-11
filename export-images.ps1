# TNS Project - Image Export Script
# This script builds and exports all microservice images to .tar files for offline deployment.

$services = @(
    "gateway",
    "usermgmt-svc",
    "content-svc",
    "growth-svc",
    "notification-svc",
    "analytics-svc",
    "healthintel-svc",
    "qa-svc"
)

Write-Host "--- Starting TNS Image Export ---" -ForegroundColor Cyan

# 1. Build images
Write-Host "Building images..." -ForegroundColor Yellow
docker compose build

# Create output directory
if (!(Test-Path "deploy-binaries")) {
    New-Item -ItemType Directory -Path "deploy-binaries"
}

# 2. Export each service
foreach ($svc in $services) {
    $imageName = "tnsbackend-$svc"
    $tarPath = "deploy-binaries/$svc.tar"
    
    Write-Host "Exporting $imageName to $tarPath..." -ForegroundColor Green
    docker save -o $tarPath $imageName
}

Write-Host "--- Export Complete! ---" -ForegroundColor Cyan
Write-Host "Please copy the 'deploy-binaries' folder and 'docker-compose.yml' to your server." -ForegroundColor Yellow
