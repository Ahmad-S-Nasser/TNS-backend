#!/bin/bash
# TNS Project - Image Export Script (Linux/macOS)

services=(
    "gateway"
    "usermgmt-svc"
    "content-svc"
    "growth-svc"
    "notification-svc"
    "analytics-svc"
    "healthintel-svc"
    "qa-svc"
)

echo "--- Starting TNS Image Export ---"

# 1. Build images
echo "Building images..."
docker compose build

# Create output directory
mkdir -p deploy-binaries

# 2. Export each service
for svc in "${services[@]}"
do
    imageName="tnsbackend-$svc"
    tarPath="deploy-binaries/$svc.tar"
    
    echo "Exporting $imageName to $tarPath..."
    docker save -o "$tarPath" "$imageName"
done

echo "--- Export Complete! ---"
echo "Please copy the 'deploy-binaries' folder and 'docker-compose.yml' to your server."
