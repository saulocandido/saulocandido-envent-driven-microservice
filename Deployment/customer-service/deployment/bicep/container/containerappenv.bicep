param name string
param location string
param vnetSubnetId string

resource env 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: name
  location: location
  properties: {
    vnetConfiguration: {
      infrastructureSubnetId: vnetSubnetId
    }
    zoneRedundant: false
  }
}

output id string = env.id
