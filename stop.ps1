# Tips & Steps — Stop all Docker containers

Write-Host "Stopping all containers..." -ForegroundColor Yellow
docker compose down
Write-Host "Done." -ForegroundColor Green
