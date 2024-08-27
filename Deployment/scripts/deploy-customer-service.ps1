# Define variables
$RESOURCE_GROUP = "rg-customer-service-dev"
$LOCATION = "EastUS"
$TEMPLATE_FILE = "../customer-service/deployment/bicep/main.bicep"
$PARAMETERS_FILE = "../customer-service/deployment/bicep/parameters/dev.parameters.json"

# Ensure you're logged in to Azure
Write-Host "Ensuring you're logged in to Azure..."
az account show
if ($LASTEXITCODE -ne 0) {
    Write-Host "You're not logged in. Please run 'az login' first."
    exit 1
}

# Check if the resource group exists
$rgExists = az group exists --name $RESOURCE_GROUP

if ($rgExists -eq "false") {
    # Create the resource group if it doesn't exist
    Write-Host "Creating resource group $RESOURCE_GROUP in $LOCATION..."
    az group create --name $RESOURCE_GROUP --location $LOCATION
    Write-Host "Resource group $RESOURCE_GROUP created in $LOCATION."
} else {
    Write-Host "Resource group $RESOURCE_GROUP already exists."
}

# Deploy the Bicep template
Write-Host "Deploying customer service infrastructure..."
$deployment = az deployment group create `
    --resource-group $RESOURCE_GROUP `
    --template-file $TEMPLATE_FILE `
    --output json | ConvertFrom-Json

if ($LASTEXITCODE -ne 0) {
    Write-Host "Deployment failed. Please check the error messages above."
    exit 1
}

# Extract necessary information from the deployment output
$acrLoginServer = $deployment.properties.outputs.acrLoginServer.value
$customerServiceFqdn = $deployment.properties.outputs.customerServiceFqdn.value

Write-Host "Deployment completed successfully."
Write-Host "ACR Login Server: $acrLoginServer"
Write-Host "Customer Service FQDN: $customerServiceFqdn"

# Build and push the Docker image
$imageName = "$acrLoginServer/customerservice:dev"

Write-Host "Building Docker image..."
docker build -t $imageName ../../reference-architecture/CustomerService

Write-Host "Logging in to ACR..."
az acr login --name $acrLoginServer

Write-Host "Pushing image to ACR..."
docker push $imageName

Write-Host "Image pushed successfully."

Write-Host "Deployment and image push completed successfully."
Write-Host "You can access your customer service at: https://$customerServiceFqdn"
