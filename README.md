# gRPC Sample : .NET 6

The following is a sample of a gRPC client calling another container running gRPC server to execute a `SayHello` call.  The solution runs on Azure Container Apps.

## Deploy the sample

### Azure CLI

```bash
RESOURCE_GROUP="grpc-sample"
LOCATION="canadacentral"
LOG_ANALYTICS_WORKSPACE="aca-logs"
ACA_ENVIRONMENT="aca-env"

az extension add \
  --source https://workerappscliextension.blob.core.windows.net/azure-cli-extension/containerapp-0.2.0-py2.py3-none-any.whl
az provider register --namespace Microsoft.Web

az group create \
  --name $RESOURCE_GROUP \
  --location "$LOCATION"

az monitor log-analytics workspace create \
  --resource-group $RESOURCE_GROUP \
  --workspace-name $LOG_ANALYTICS_WORKSPACE

LOG_ANALYTICS_WORKSPACE_CLIENT_ID=`az monitor log-analytics workspace show --query customerId -g $RESOURCE_GROUP -n $LOG_ANALYTICS_WORKSPACE --out tsv`
LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET=`az monitor log-analytics workspace get-shared-keys --query primarySharedKey -g $RESOURCE_GROUP -n $LOG_ANALYTICS_WORKSPACE --out tsv`

# Create the Container Apps Environment
az containerapp env create \
  --name $ACA_ENVIRONMENT \
  --resource-group $RESOURCE_GROUP \
  --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_CLIENT_ID \
  --logs-workspace-key $LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET \
  --location "$LOCATION"

# Create the gRPC backend internal service
az containerapp create \
  --name grpc-backend \
  --resource-group $RESOURCE_GROUP \
  --environment $ACA_ENVIRONMENT \
  --image ghcr.io/jeffhollan/grpc-sample-dotnet/grpc-backend:main \
  --ingress 'internal' \
  --target-port 8085 \
  --transport 'http2'

GRPC_SERVER_ADDRESS=$(az containerapp show \
  --resource-group $RESOURCE_GROUP \
  --name grpc-backend \
  --query configuration.ingress.fqdn -otsv)

# Create the HTTPS frontend gRPC client container
az containerapp create \
  --name https-frontend \
  --resource-group $RESOURCE_GROUP \
  --environment $ACA_ENVIRONMENT \
  --image ghcr.io/jeffhollan/grpc-sample-dotnet/https-frontend:main \
  --environment-variables GRPC_SERVER_ADDRESS=https://$GRPC_SERVER_ADDRESS \
  --target-port 80 \
  --ingress 'external' \
  --query configuration.ingress.fqdn
```