# Tips & Steps — One-time Setup (run once after infra is started)
# Requires: mongosh installed (comes with MongoDB)

Write-Host ""
Write-Host "=== Tips & Steps One-Time Setup ===" -ForegroundColor Green
Write-Host ""

# ── Step 1: Kafka topics ───────────────────────────────────────────────────────
Write-Host "Step 1/3: Creating Kafka topics..." -ForegroundColor Yellow

$topics = @(
    "tns.user.registered", "tns.user.profile-updated",
    "tns.child.profile-created", "tns.child.profile-updated",
    "tns.content.viewed", "tns.content.published", "tns.content.rated",
    "tns.growth.assessment-completed", "tns.growth.alert-triggered",
    "tns.qa.question-submitted", "tns.qa.question-answered",
    "tns.notification.requested", "tns.notification.sent",
    "tns.health.data-reported",
    "tns.analytics.daily-metrics", "tns.analytics.content-engagement",
    "tns.dlq.notification", "tns.dlq.analytics", "tns.dlq.growth"
)

foreach ($topic in $topics) {
    docker run --rm --network host confluentinc/cp-kafka:7.7.0 `
        kafka-topics --bootstrap-server localhost:9092 `
        --create --if-not-exists --topic $topic `
        --partitions 3 --replication-factor 1 2>$null
    Write-Host "  + $topic" -ForegroundColor Gray
}

Write-Host "  Kafka topics done." -ForegroundColor Green
Write-Host ""

# ── Step 2: MongoDB indexes ────────────────────────────────────────────────────
Write-Host "Step 2/3: Creating MongoDB indexes..." -ForegroundColor Yellow

$mongoshAvailable = Get-Command mongosh -ErrorAction SilentlyContinue
if ($mongoshAvailable) {
    mongosh "mongodb://localhost:27017" --file "infra\mongodb\init-indexes.js"
    Write-Host "  MongoDB indexes done." -ForegroundColor Green
} else {
    Write-Host "  WARNING: mongosh not found. Run manually:" -ForegroundColor Yellow
    Write-Host "    mongosh mongodb://localhost:27017 --file infra\mongodb\init-indexes.js" -ForegroundColor Gray
}

Write-Host ""

# ── Step 3: Seed growth skills ─────────────────────────────────────────────────
Write-Host "Step 3/3: Seeding Growth Matrix skills..." -ForegroundColor Yellow

if ($mongoshAvailable) {
    mongosh "mongodb://localhost:27017" --file "infra\mongodb\seed-growth-skills.js"
    Write-Host "  Growth skills seeded." -ForegroundColor Green
} else {
    Write-Host "  WARNING: mongosh not found. Run manually:" -ForegroundColor Yellow
    Write-Host "    mongosh mongodb://localhost:27017 --file infra\mongodb\seed-growth-skills.js" -ForegroundColor Gray
}

Write-Host ""
Write-Host "=== Setup complete! ===" -ForegroundColor Green
Write-Host "Now run: .\start-services.ps1" -ForegroundColor Yellow
