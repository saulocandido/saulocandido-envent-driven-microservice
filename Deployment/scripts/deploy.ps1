# Define variables
$RESOURCE_GROUP = "rg-eventdriven-shared-dev"
$LOCATION = "EastUS"
$TEMPLATE_FILE = "../global-deployment/bicep/main.bicep"
$PARAMETERS_FILE = "../global-deployment/bicep/parameters/dev.parameters.json"

# Check if the resource group exists
$rgExists = az group exists --name $RESOURCE_GROUP

if ($rgExists -eq "false") {
    # Create the resource group if it doesn't exist
    az group create --name $RESOURCE_GROUP --location $LOCATION
    Write-Output "Resource group $RESOURCE_GROUP created in $LOCATION."
} else {
    Write-Output "Resource group $RESOURCE_GROUP already exists."
}

# Deploy the Bicep template
az deployment group create `
  --resource-group $RESOURCE_GROUP `
  --template-file $TEMPLATE_FILE `
  --parameters @$PARAMETERS_FILE
