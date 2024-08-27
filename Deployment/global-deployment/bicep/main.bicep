param location string
param vnetName string
param storageAccountName string
param appGatewayName string
param publicIpName string
param nsgName string

module vnetModule 'networking/vnet.bicep' = {
  name: 'vnetDeployment'
  params: {
    vnetName: vnetName
  }
}

module nsgModule 'networking/nsg.bicep' = {
  name: 'nsgDeployment'
  params: {
    nsgName: nsgName
    location: location
  }
}

module storageModule 'storage/storageaccount.bicep' = {
  name: 'storageAccountDeployment'
  params: {
    storageAccountName: storageAccountName
    location: location
  }
}

resource publicIp 'Microsoft.Network/publicIPAddresses@2022-07-01' = {
  name: publicIpName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
  }
}

module sharedResourcesModule 'sharedresources.bicep' = {
  name: 'sharedResourcesDeployment'
  params: {
    appGatewayName: appGatewayName
    location: location
    subnetId: vnetModule.outputs.subnetIds[1]  // Use AppGatewaySubnet
    publicIpId: publicIp.id
  }
}

output vnetId string = vnetModule.outputs.vnetId
output subnetIds array = vnetModule.outputs.subnetIds
output nsgId string = nsgModule.outputs.nsgId
output storageAccountId string = storageModule.outputs.storageAccountId
output appGatewayId string = sharedResourcesModule.outputs.appGatewayId
output publicIpId string = publicIp.id
