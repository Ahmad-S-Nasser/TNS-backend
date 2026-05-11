# Tips & Steps — Tail logs from all containers
# Usage: .\logs.ps1
# Usage: .\logs.ps1 usermgmt-svc   (single service)

param([string]$service = "")

if ($service) {
    docker compose logs -f --tail=100 $service
} else {
    docker compose logs -f --tail=50
}
