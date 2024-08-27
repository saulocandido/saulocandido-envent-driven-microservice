using System.Diagnostics.CodeAnalysis;
using Common.CQRS.Abstration.Exceptions;
using MongoDB.Driver;
using OrderService.Domain.OrderAggregate;

namespace OrderService.Repositories;

[ExcludeFromCodeCoverage]
public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _collection;

    public OrderRepository(IMongoCollection<Order> collection)
    {
        _collection = collection;
    }

    public async Task<IEnumerable<Order>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId) =>
        await _collection.Find(e => e.CustomerId == customerId).ToListAsync();

    public async Task<Order?> GetAsync(Guid id) =>
        await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();

    public async Task<Order?> AddAsync(Order entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing != null) return null;
        if (string.IsNullOrWhiteSpace(entity.ETag))
            entity.ETag = Guid.NewGuid().ToString();
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Order?> UpdateAsync(Order entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        var result = await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
        return result.ModifiedCount > 0 ? entity : null;
    }

    public async Task<Order?> UpdateAddressAsync(Guid orderId, Address address)
    {
        var existing = await GetAsync(orderId);
        if (existing == null) return null;
        existing.ShippingAddress = address;
        var result = await _collection.ReplaceOneAsync(e => e.Id == orderId, existing);
        return result.ModifiedCount > 0 ? existing : null;
    }

    public async Task<int> RemoveAsync(Guid id)
    {
        var result = await _collection.DeleteOneAsync(e => e.Id == id);
        return (int)result.DeletedCount;
    }

    public async Task<Order?> UpdateOrderStateAsync(Order entity, OrderState orderState)
    {
        var existing = await GetAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        entity.OrderState = orderState;
        var result = await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
        return result.ModifiedCount > 0 ? entity : null;
    }
}