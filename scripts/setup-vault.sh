#!/bin/bash
set -e

echo "🚀 Installing HashiCorp Vault into the cluster (Dev Mode)..."
helm repo add hashicorp https://helm.releases.hashicorp.com
helm install vault hashicorp/vault \
  --set "server.dev.enabled=true" \
  --set "injector.enabled=true" \
  --namespace vault --create-namespace || true

echo "⏳ Waiting for Vault pod to be ready..."
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=vault -n vault --timeout=120s

echo "🔐 Configuring Vault Database Engine for ideal-service-dev-postgresql..."
kubectl exec vault-0 -n vault -- sh -c '
  # Enable Kubernetes Auth
  vault auth enable kubernetes || true
  vault write auth/kubernetes/config kubernetes_host="https://$KUBERNETES_SERVICE_HOST:$KUBERNETES_SERVICE_PORT"
  
  # Create Policy for ideal-service
  vault policy write ideal-service-policy - <<EOF
path "database/creds/ideal-service-role" {
  capabilities = ["read"]
}
EOF
  
  # Bind Policy to Service Account
  vault write auth/kubernetes/role/ideal-service-role \
      bound_service_account_names="ideal-service-dev,ideal-service-staging,ideal-service-prod" \
      bound_service_account_namespaces="dev,staging,prod" \
      policies="ideal-service-policy" \
      ttl=24h
  
  # Enable Database Secrets Engine
  vault secrets enable database || true
  
  # Configure PostgreSQL Connection (Using the default postgres password from the chart)
  vault write database/config/ideal-service-role \
      plugin_name=postgresql-database-plugin \
      allowed_roles="ideal-service-role" \
      connection_url="postgresql://{{username}}:{{password}}@ideal-service-dev-postgresql.dev.svc.cluster.local:5432/idealdb?sslmode=disable" \
      username="postgres" \
      password="postgres"
  
  # Create Role for Dynamic Credentials
  vault write database/roles/ideal-service-role \
      db_name=ideal-service-role \
      creation_statements="CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '\''{{password}}'\'' VALID UNTIL '\''{{expiration}}'\''; GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO \"{{name}}\";" \
      default_ttl="1h" \
      max_ttl="24h"
'

echo "✅ Vault is fully configured for Zero-Downtime Dynamic Credentials!"
