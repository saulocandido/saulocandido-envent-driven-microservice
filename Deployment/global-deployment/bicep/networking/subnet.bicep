
@description('The name of the Virtual Network.')
param vnetName string

@description('The name of the Subnet.')
param subnetName string

@description('The address prefix for the Subnet.')
param addressPrefix string = '10.0.1.0/24'

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-11-01' = {
  name: '${vnetName}/${subnetName}'
  properties: {
    addressPrefix: addressPrefix
  }
}

output subnetId string = subnet.id
