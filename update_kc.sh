#!/bin/bash
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin
CID=$(/opt/keycloak/bin/kcadm.sh get clients -r tips-steps -q clientId=tns-admin-portal | grep '"id"' | cut -d '"' -f 4)
if [ ! -z "$CID" ]; then
  /opt/keycloak/bin/kcadm.sh update clients/$CID -r tips-steps -s clientId=tns-admin -s publicClient=true -s directAccessGrantsEnabled=true -s 'redirectUris=["http://localhost:8080/*", "http://localhost:3001/*"]'
  echo "Client updated successfully."
else
  echo "Client tns-admin-portal not found."
fi
