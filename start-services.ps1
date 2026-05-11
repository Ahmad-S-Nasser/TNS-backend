# Tips & Steps — Start all 8 microservices in Docker

Write-Host ""
Write-Host "Building and starting microservices..." -ForegroundColor Green
Write-Host "(First run will take a few minutes to build Docker images)"
Write-Host ""

docker compose up -d --build `
    gateway usermgmt-svc content-svc growth-svc `
    notification-svc analytics-svc healthintel-svc qa-svc

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed. Check the output above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Services started:" -ForegroundColor Green
Write-Host "  Gateway          -> http://localhost:6000" -ForegroundColor Cyan
Write-Host "  UserManagement   -> http://localhost:6001/swagger" -ForegroundColor Cyan
Write-Host "  Content          -> http://localhost:6002/swagger" -ForegroundColor Cyan
Write-Host "  GrowthMatrix     -> http://localhost:6003/swagger" -ForegroundColor Cyan
Write-Host "  Notification     -> http://localhost:6004/swagger" -ForegroundColor Cyan
Write-Host "  Analytics        -> http://localhost:6005/swagger" -ForegroundColor Cyan
Write-Host "  HealthIntel      -> http://localhost:6006/swagger" -ForegroundColor Cyan
Write-Host "  QA               -> http://localhost:6007/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Run .\health.ps1 to verify all services are up." -ForegroundColor Yellow
