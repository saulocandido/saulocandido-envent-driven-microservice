param name string
param location string
param containerAppEnvId string
param imageName string
param containerPort int
param isExternalIngress bool = true
param containerRegistry string = ''
param registryUsername string = ''
@secure()
param registryPassword string = ''
param env array = []

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: name
  location: location
  properties: {
    managedEnvironmentId: containerAppEnvId
    configuration: {
      ingress: isExternalIngress ? {
        external: true
        targetPort: containerPort
      } : null
      registries: !empty(containerRegistry) ? [
        {
          server: containerRegistry
          username: registryUsername
          passwordSecretRef: 'registry-password'
        }
      ] : []
      secrets: !empty(containerRegistry) ? [
        {
          name: 'registry-password'
          value: registryPassword
        }
      ] : []
    }
    template: {
      containers: [
        {
          name: name
          image: imageName
          env: env
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

output fqdn string = isExternalIngress ? containerApp.properties.configuration.ingress.fqdn : containerApp.name
