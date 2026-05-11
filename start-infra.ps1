# Tips & Steps — Start Infrastructure (Windows PowerShell)
# Run from the project root folder

Write-Host ""
Write-Host "Starting Tips & Steps infrastructure..." -ForegroundColor Green
Write-Host ""

docker compose up -d keycloak keycloak-db schema-registry kafka-ui redis seq

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: docker compose failed. Make sure Docker Desktop is running." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Infrastructure started. Waiting ~30s for Keycloak to initialize..." -ForegroundColor Yellow
Write-Host ""
Write-Host "  Keycloak     -> http://localhost:6080  (admin / admin)" -ForegroundColor Cyan
Write-Host "  Kafka UI     -> http://localhost:6092" -ForegroundColor Cyan
Write-Host "  Seq Logs     -> http://localhost:6093" -ForegroundColor Cyan
Write-Host "  Redis        -> localhost:6379" -ForegroundColor Cyan
Write-Host ""
Write-Host "After ~30s, run: .\setup-once.ps1" -ForegroundColor Yellow
