using System.Diagnostics.CodeAnalysis;
using Common.CQRS.Abstration.Exceptions;
using CustomerService.Domain.CustomerAggregate;
using MongoDB.Driver;

namespace CustomerService.Repositories;

[ExcludeFromCodeCoverage]
public class CustomerRepository : ICustomerRepository
{
    private readonly IMongoCollection<Customer> _collection;

    public CustomerRepository(IMongoCollection<Customer> collection)
    {
        _collection = collection;
    }

    public async Task<IEnumerable<Customer>> GetAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Customer?> GetAsync(Guid id) =>
        await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();

    public async Task<Customer?> AddAsync(Customer entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing != null) return null;
        if (string.IsNullOrWhiteSpace(entity.ETag))
            entity.ETag = Guid.NewGuid().ToString();
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Customer?> UpdateAsync(Customer entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        var result = await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
        return result.ModifiedCount > 0 ? entity : null;
    }

    public async Task<int> RemoveAsync(Guid id)
    {
        var result = await _collection.DeleteOneAsync(e => e.Id == id);
        return (int)result.DeletedCount;
    }
}
