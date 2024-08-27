using Common.CQRS.Abstration.Queries;
using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.Repositories;

namespace CustomerService.Domain.CustomerAggregate.QueryHandlers;

public class GetCustomersHandler : IQueryHandler<GetCustomers, IEnumerable<Customer>>
{
    private readonly ICustomerRepository _repository;

    public GetCustomersHandler(
        ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Customer>> Handle(GetCustomers query, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAsync();
        return result;
    }
}