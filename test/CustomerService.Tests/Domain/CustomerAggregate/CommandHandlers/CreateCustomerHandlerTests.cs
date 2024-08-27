using System.Threading;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Domain.CustomerAggregate.Events;
using CustomerService.Repositories;
using MediatR;
using Moq;
using Xunit;
using Common.CQRS.Abstration.command;

namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers;

public class CreateCustomerHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<IMediator> _mediatorMock;

    public CreateCustomerHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new CreateCustomerHandler(_repositoryMock.Object, _mediatorMock.Object);
        Assert.NotNull(handler);
        Assert.IsType<CreateCustomerHandler>(handler);
    }

    [Fact]
    public async Task WhenCreatingEntityFails_ThenShouldReturnFailure()
    {
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer)null);

        var handler = new CreateCustomerHandler(_repositoryMock.Object, _mediatorMock.Object);
        var cmdResult = await handler.Handle(new CreateCustomer(new Customer()), CancellationToken.None);

        Assert.Equal(CommandOutcome.InvalidCommand, cmdResult.Outcome);
        Assert.Null(cmdResult.Entity);
    }

    [Fact]
    public async Task WhenEntityIsCreated_ThenShouldReturnSuccess()
    {
        var customer = new Customer { Id = System.Guid.NewGuid() };
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(customer);

        var handler = new CreateCustomerHandler(_repositoryMock.Object, _mediatorMock.Object);
        var cmdResult = await handler.Handle(new CreateCustomer(customer), CancellationToken.None);

        Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
        Assert.NotNull(cmdResult.Entity);
        Assert.Equal(customer.Id, cmdResult.Entity.Id);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<CustomerCreated>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenEntityIsCreated_ThenShouldPublishEvent()
    {
        var customer = new Customer { Id = System.Guid.NewGuid() };
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(customer);

        var handler = new CreateCustomerHandler(_repositoryMock.Object, _mediatorMock.Object);
        await handler.Handle(new CreateCustomer(customer), CancellationToken.None);

        _mediatorMock.Verify(m => m.Publish(It.Is<CustomerCreated>(e => e.Entity == customer), It.IsAny<CancellationToken>()), Times.Once);
    }
}