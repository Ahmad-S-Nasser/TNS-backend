#!/bin/bash
# Create all Kafka topics for the Tips & Steps platform
# Run after Kafka broker is up: docker compose exec kafka bash /topics.sh

KAFKA_HOST="${KAFKA_HOST:-kafka:9092}"
PARTITIONS="${PARTITIONS:-3}"
REPLICATION="${REPLICATION:-1}"

create_topic() {
  local TOPIC=$1
  local RETENTION="${2:-604800000}"  # default 7 days
  echo "Creating topic: $TOPIC"
  kafka-topics --bootstrap-server "$KAFKA_HOST" \
    --create --if-not-exists \
    --topic "$TOPIC" \
    --partitions "$PARTITIONS" \
    --replication-factor "$REPLICATION" \
    --config retention.ms="$RETENTION"
}

# User & Profile events
create_topic "tns.user.registered"
create_topic "tns.user.profile-updated"
create_topic "tns.child.profile-created"
create_topic "tns.child.profile-updated"

# Content events
create_topic "tns.content.viewed"
create_topic "tns.content.published"
create_topic "tns.content.rated"

# Growth Matrix events
create_topic "tns.growth.assessment-completed"
create_topic "tns.growth.alert-triggered"

# Q&A events
create_topic "tns.qa.question-submitted"
create_topic "tns.qa.question-answered"

# Notification events
create_topic "tns.notification.requested"
create_topic "tns.notification.sent"

# Health Intelligence
create_topic "tns.health.data-reported"

# Analytics Projections (internal — longer retention: 30 days)
create_topic "tns.analytics.daily-metrics"      "2592000000"
create_topic "tns.analytics.content-engagement" "2592000000"

# Dead-Letter Topics (for failed message handling)
create_topic "tns.dlq.notification"
create_topic "tns.dlq.analytics"
create_topic "tns.dlq.growth"

echo "All topics created successfully."
kafka-topics --bootstrap-server "$KAFKA_HOST" --list
