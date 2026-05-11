#!/bin/bash
# Authenticate to Keycloak
/opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user admin --password admin

# Create user admin in tips-steps realm
/opt/keycloak/bin/kcadm.sh create users -r tips-steps -s username=admin -s enabled=true -s emailVerified=true -s firstName=Super -s lastName=Admin -s email=admin@tipsandsteps.eg

# Set password to "admin"
/opt/keycloak/bin/kcadm.sh set-password -r tips-steps --username admin --new-password admin

# Assign the superadmin role to the user
/opt/keycloak/bin/kcadm.sh add-roles -r tips-steps --uusername admin --rolename superadmin

echo "Admin user created successfully!"
