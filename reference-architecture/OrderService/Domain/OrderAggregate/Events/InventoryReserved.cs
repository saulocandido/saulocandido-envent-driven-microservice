namespace OrderService.Domain.OrderAggregate.Events
{

    public record InventoryReserved(Guid EntityId) : DomainEvent<Order>(null, EntityId);
}
