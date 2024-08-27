param location string = resourceGroup().location
param environmentName string
param acrName string
param containerAppEnvName string = 'env-${environmentName}'
param customerServiceAppName string = 'customer-service-${environmentName}'
param sharedResourceGroupName string = 'rg-eventdriven-shared-dev'
param sharedVNetName string = 'dev-vnet'
param containerAppsSubnetName string = 'container-apps-subnet'

// Reference to existing shared VNet
resource vnet 'Microsoft.Network/virtualNetworks@2021-05-01' existing = {
  name: sharedVNetName
  scope: resourceGroup(sharedResourceGroupName)
}

// Reference to existing container apps subnet
resource containerAppsSubnet 'Microsoft.Network/virtualNetworks/subnets@2021-05-01' existing = {
  parent: vnet
  name: containerAppsSubnetName
}

// Deploy ACR
module acrModule 'container/acr.bicep' = {
  name: 'acrDeployment'
  params: {
    acrName: acrName
    location: location
  }
}

// Deploy Container App Environment
module containerAppEnv 'container/containerappenv.bicep' = {
  name: 'containerAppEnvDeployment'
  params: {
    name: containerAppEnvName
    location: location
    vnetSubnetId: containerAppsSubnet.id
  }
}

// Deploy Customer Service Container App
module customerServiceApp 'container/containerapp.bicep' = {
  name: 'customerServiceAppDeployment'
  params: {
    name: customerServiceAppName
    location: location
    containerAppEnvId: containerAppEnv.outputs.id
    containerRegistry: acrModule.outputs.loginServer
    imageName: 'customerservice:latest'
    containerPort: 80
  }
}

output acrLoginServer string = acrModule.outputs.loginServer
output customerServiceFqdn string = customerServiceApp.outputs.fqdn
