# Tips & Steps — Service Health Check

Write-Host ""
Write-Host "=== Service Health Check ===" -ForegroundColor Green
Write-Host ""

function Check-Service($name, $port, $path = "/health") {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$port$path" `
            -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
        Write-Host "  [OK] $name (port $port)" -ForegroundColor Green
    } catch {
        Write-Host "  [--] $name (port $port) - not responding" -ForegroundColor Red
    }
}

Check-Service "Gateway"          6000
Check-Service "UserManagement"   6001
Check-Service "Content"          6002
Check-Service "GrowthMatrix"     6003
Check-Service "Notification"     6004
Check-Service "Analytics"        6005
Check-Service "HealthIntel"      6006
Check-Service "QA"               6007

Write-Host ""
Check-Service "Keycloak"         6080 "/realms/tips-steps"
Check-Service "Schema Registry"  6091 "/"
Check-Service "Kafka UI"         6092 "/"
Check-Service "Seq Logs"         6093 "/"
Check-Service "Redis"            6379 ""

Write-Host ""
