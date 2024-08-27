var builder = DistributedApplication.CreateBuilder(args);

var pubSub = builder.AddDaprPubSub("pubsub");

builder.AddProject<Projects.CustomerService>("customer-service")
    .WithReference(pubSub);
builder.AddProject<Projects.OrderService>("order-service")
    .WithReference(pubSub);

builder.Build().Run();