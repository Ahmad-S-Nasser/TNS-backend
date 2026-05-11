# Tips & Steps (خطوات ونصائح) — Developer Makefile
# Prerequisites: Docker Desktop running, MongoDB on localhost:27017, Kafka on localhost:9092
#
# Usage: make <target>

.PHONY: help infra infra-down services services-down all down build logs \
        kafka-topics mongo-indexes seed health swagger

# ── Colours ───────────────────────────────────────────────────────────────────
GREEN  := \033[0;32m
YELLOW := \033[1;33m
NC     := \033[0m

help:
	@echo ""
	@echo "$(GREEN)  Tips & Steps (خطوات ونصائح) — Backend$(NC)"
	@echo "  ══════════════════════════════════════════════════"
	@echo ""
	@echo "$(YELLOW)  QUICK START:$(NC)"
	@echo "    make infra           Start Keycloak, Redis, Seq, Schema Registry, Kafka UI"
	@echo "    make kafka-topics    Create all Kafka topics (run once after Kafka is up)"
	@echo "    make mongo-indexes   Create MongoDB indexes (run once)"
	@echo "    make seed            Seed growth skills into MongoDB"
	@echo "    make services        Start all 8 .NET microservices in Docker"
	@echo "    make health          Check health of all running services"
	@echo ""
	@echo "$(YELLOW)  INDIVIDUAL COMMANDS:$(NC)"
	@echo "    make all             Start infra + all services"
	@echo "    make infra-down      Stop infrastructure containers only"
	@echo "    make services-down   Stop microservice containers only"
	@echo "    make down            Stop everything"
	@echo "    make build           Rebuild all Docker images"
	@echo "    make logs            Tail logs from all containers"
	@echo "    make swagger         Open all Swagger UIs in browser"
	@echo ""
	@echo "$(YELLOW)  PORTS:$(NC)"
	@echo "    Gateway      http://localhost:6000"
	@echo "    UserMgmt     http://localhost:6001/swagger"
	@echo "    Content      http://localhost:6002/swagger"
	@echo "    GrowthMatrix http://localhost:6003/swagger"
	@echo "    Notification http://localhost:6004/swagger"
	@echo "    Analytics    http://localhost:6005/swagger"
	@echo "    HealthIntel  http://localhost:6006/swagger"
	@echo "    QA           http://localhost:6007/swagger"
	@echo "    Keycloak     http://localhost:6080  (admin / admin)"
	@echo "    Schema Reg   http://localhost:6091"
	@echo "    Kafka UI     http://localhost:6092"
	@echo "    Seq Logs     http://localhost:6093"
	@echo "    MongoDB      mongodb://localhost:27017  (local, not in Docker)"
	@echo "    Kafka        localhost:9092             (existing Docker container)"
	@echo ""

# ── Infrastructure only (Keycloak, Redis, Seq, Schema Registry, Kafka UI) ────
infra:
	@echo "$(GREEN)Starting infrastructure...$(NC)"
	docker compose up -d keycloak keycloak-db schema-registry kafka-ui redis seq
	@echo ""
	@echo "  $(GREEN)✓$(NC) Keycloak    → http://localhost:6080  (admin/admin — wait ~30s)"
	@echo "  $(GREEN)✓$(NC) Kafka UI    → http://localhost:6092"
	@echo "  $(GREEN)✓$(NC) Seq Logs    → http://localhost:6093"
	@echo "  $(GREEN)✓$(NC) Redis       → localhost:6379"

infra-down:
	docker compose stop keycloak keycloak-db schema-registry kafka-ui redis seq

# ── All microservices ─────────────────────────────────────────────────────────
services:
	@echo "$(GREEN)Starting microservices...$(NC)"
	docker compose up -d gateway usermgmt-svc content-svc growth-svc \
	                       notification-svc analytics-svc healthintel-svc qa-svc

services-down:
	docker compose stop gateway usermgmt-svc content-svc growth-svc \
	                    notification-svc analytics-svc healthintel-svc qa-svc

# ── Everything ────────────────────────────────────────────────────────────────
all: infra services
	@echo "$(GREEN)All services started.$(NC)"

down:
	docker compose down

build:
	docker compose build --no-cache

logs:
	docker compose logs -f --tail=100

# ── Kafka topics (run once after Kafka is up) ─────────────────────────────────
kafka-topics:
	@echo "$(GREEN)Creating Kafka topics...$(NC)"
	docker run --rm --network host confluentinc/cp-kafka:7.7.0 \
	  bash -c "\
	    for TOPIC in \
	      tns.user.registered tns.user.profile-updated \
	      tns.child.profile-created tns.child.profile-updated \
	      tns.content.viewed tns.content.published tns.content.rated \
	      tns.growth.assessment-completed tns.growth.alert-triggered \
	      tns.qa.question-submitted tns.qa.question-answered \
	      tns.notification.requested tns.notification.sent \
	      tns.health.data-reported \
	      tns.analytics.daily-metrics tns.analytics.content-engagement \
	      tns.dlq.notification tns.dlq.analytics tns.dlq.growth; do \
	      kafka-topics --bootstrap-server localhost:9092 \
	        --create --if-not-exists --topic \$$TOPIC \
	        --partitions 3 --replication-factor 1; \
	    done"
	@echo "$(GREEN)✓ All Kafka topics created$(NC)"

# ── MongoDB indexes & seed data ───────────────────────────────────────────────
mongo-indexes:
	@echo "$(GREEN)Creating MongoDB indexes...$(NC)"
	mongosh "mongodb://localhost:27017" --file infra/mongodb/init-indexes.js
	@echo "$(GREEN)✓ Indexes created$(NC)"

seed:
	@echo "$(GREEN)Seeding growth skills...$(NC)"
	mongosh "mongodb://localhost:27017" --file infra/mongodb/seed-growth-skills.js
	@echo "$(GREEN)✓ Growth skills seeded$(NC)"

# ── Health checks ─────────────────────────────────────────────────────────────
health:
	@echo "$(GREEN)Checking service health...$(NC)"
	@curl -sf http://localhost:6000/health > /dev/null 2>&1 && echo "  ✓ Gateway (6000)"      || echo "  ✗ Gateway (6000) — not running"
	@curl -sf http://localhost:6001/health > /dev/null 2>&1 && echo "  ✓ UserMgmt (6001)"    || echo "  ✗ UserMgmt (6001) — not running"
	@curl -sf http://localhost:6002/health > /dev/null 2>&1 && echo "  ✓ Content (6002)"     || echo "  ✗ Content (6002) — not running"
	@curl -sf http://localhost:6003/health > /dev/null 2>&1 && echo "  ✓ Growth (6003)"      || echo "  ✗ Growth (6003) — not running"
	@curl -sf http://localhost:6004/health > /dev/null 2>&1 && echo "  ✓ Notification (6004)"|| echo "  ✗ Notification (6004) — not running"
	@curl -sf http://localhost:6005/health > /dev/null 2>&1 && echo "  ✓ Analytics (6005)"   || echo "  ✗ Analytics (6005) — not running"
	@curl -sf http://localhost:6006/health > /dev/null 2>&1 && echo "  ✓ HealthIntel (6006)" || echo "  ✗ HealthIntel (6006) — not running"
	@curl -sf http://localhost:6007/health > /dev/null 2>&1 && echo "  ✓ QA (6007)"          || echo "  ✗ QA (6007) — not running"
	@echo ""
	@curl -sf http://localhost:6080/realms/tips-steps > /dev/null 2>&1 && echo "  ✓ Keycloak realm (6080)" || echo "  ✗ Keycloak (6080) — not running or realm not imported yet"
	@curl -sf http://localhost:6093 > /dev/null 2>&1 && echo "  ✓ Seq (6093)"           || echo "  ✗ Seq (6093) — not running"
	@curl -sf http://localhost:6379 > /dev/null 2>&1 && echo "  ✓ Redis (6379)"         || echo "  ✗ Redis (6379) — check locally"

# ── Open swagger UIs ─────────────────────────────────────────────────────────
swagger:
	@echo "Opening Swagger UIs..."
	start http://localhost:6001/swagger
	start http://localhost:6002/swagger
	start http://localhost:6003/swagger
	start http://localhost:6007/swagger
