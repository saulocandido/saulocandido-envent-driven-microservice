@description('The name of the Virtual Network.')
param vnetName string

resource vnet 'Microsoft.Network/virtualNetworks@2020-11-01' = {
  name: vnetName
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '10.0.1.0/24'
        }
      }
      {
        name: 'AppGatewaySubnet'
        properties: {
          addressPrefix: '10.0.2.0/24'
        }
      }
    ]
  }
}

output vnetId string = vnet.id
output subnetIds array = [
  resourceId('Microsoft.Network/virtualNetworks/subnets', vnet.name, 'default')
  resourceId('Microsoft.Network/virtualNetworks/subnets', vnet.name, 'AppGatewaySubnet')
]
