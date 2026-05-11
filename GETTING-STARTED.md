# Getting Started — Tips & Steps Backend
## (Windows PowerShell)

## Prerequisites (already on your machine)
- ✅ Docker Desktop running
- ✅ Kafka running in Docker on `localhost:9092`
- ✅ MongoDB installed locally on `localhost:27017`
- .NET 8 SDK → [download](https://dotnet.microsoft.com/download/dotnet/8.0)
- mongosh (ships with MongoDB) — verify: `mongosh --version`

> **Note:** All commands below use PowerShell. Open a terminal in the project root folder.

---

## Step 1 — Start infrastructure

```powershell
.\start-infra.ps1
```

Starts: **Keycloak** (`:6080`), **Redis** (`:6379`), **Seq** (`:6093`), **Schema Registry** (`:6091`), **Kafka UI** (`:6092`)

Wait ~30 seconds, then open http://localhost:6080 → login `admin / admin`.  
The `tips-steps` realm is auto-imported (4 roles: parent, doctor, admin, superadmin).

---

## Step 2 — One-time database setup *(run once)*

```powershell
.\setup-once.ps1
```

This will:
- Create all 19 Kafka topics (`tns.*`)
- Create MongoDB indexes for all 8 databases
- Seed 15 bilingual Growth Matrix skills

---

## Step 3A — Run all services in Docker

```powershell
.\start-services.ps1
```

First run builds all Docker images (~3-5 min).

---

## Step 3B — Run a single service locally (faster for development)

```powershell
cd src\Services\GrowthMatrix\TipsAndSteps.GrowthMatrix.API
dotnet run
```

`dotnet run` automatically uses `appsettings.Development.json` → connects to `localhost:27017` (MongoDB) and `localhost:9092` (Kafka).

---

## Step 4 — Verify everything

```powershell
.\health.ps1
```

---

## Other commands

```powershell
.\stop.ps1              # Stop all Docker containers
.\logs.ps1              # Tail all container logs
.\logs.ps1 growth-svc  # Tail a specific service
```

---

## Service URLs

| Service | URL | Swagger |
|---------|-----|---------|
| Gateway (YARP) | http://localhost:6000 | — |
| UserManagement | http://localhost:6001 | http://localhost:6001/swagger |
| Content | http://localhost:6002 | http://localhost:6002/swagger |
| GrowthMatrix | http://localhost:6003 | http://localhost:6003/swagger |
| Notification | http://localhost:6004 | http://localhost:6004/swagger |
| Analytics | http://localhost:6005 | http://localhost:6005/swagger |
| HealthIntelligence | http://localhost:6006 | http://localhost:6006/swagger |
| Q&A | http://localhost:6007 | http://localhost:6007/swagger |
| Keycloak | http://localhost:6080 | admin / admin |
| Kafka UI | http://localhost:6092 | — |
| Seq (Logs) | http://localhost:6093 | — |
| MongoDB | mongodb://localhost:27017 | local, not Docker |
| Kafka | localhost:9092 | existing Docker container |

---

## MongoDB databases (all on `localhost:27017`)

| Database | Service |
|----------|---------|
| `tips-steps-usermgmt` | UserManagement |
| `tips-steps-content` | Content |
| `tips-steps-growth` | GrowthMatrix |
| `tips-steps-notification` | Notification |
| `tips-steps-analytics` | Analytics |
| `tips-steps-analytics-read` | Analytics CQRS read-only (Kafka projected) |
| `tips-steps-health-intel` | HealthIntelligence |
| `tips-steps-qa` | Q&A |
