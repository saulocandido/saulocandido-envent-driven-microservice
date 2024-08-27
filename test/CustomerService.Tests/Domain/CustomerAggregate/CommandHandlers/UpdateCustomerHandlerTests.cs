using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Common.Integration.Events;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Repositories;
using CustomerService.Tests.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Common.CQRS.Abstration.Exceptions;
using Common.CQRS.Abstration.command;

namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers;

public class UpdateCustomerHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Fixture _fixture = new();
    private readonly Mock<ILogger<UpdateCustomerHandler>> _loggerMock;
    private readonly IMapper _mapper = MappingHelper.GetMapper();
    private readonly Mock<ICustomerRepository> _repositoryMock;

    public UpdateCustomerHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateCustomerHandler>>();
        _repositoryMock = new Mock<ICustomerRepository>();
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = GetHandler();

        Assert.NotNull(handler);
        Assert.IsType<UpdateCustomerHandler>(handler);
    }

    [Fact]
    public async Task WhenNoExistingCustomerIsFound_ThenShouldReturnNotFound()
    {
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(GenerateCustomer());

        var handler = GetHandler();

        var cmdResult = await handler.Handle(new UpdateCustomer(GenerateCustomer()), CancellationToken.None);

        Assert.Equal(CommandOutcome.NotFound, cmdResult.Outcome);
    }

    [Fact]
    public async Task WhenTheAddressIsUpdated_ThenEventShouldBePublished()
    {
        var existingCustomer = GenerateCustomer();
        var updatedCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(updatedCustomer);

        _mediatorMock.Setup(x => x.Publish(It.IsAny<CustomerAddressUpdated>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = GetHandler();

        await handler.Handle(new UpdateCustomer(updatedCustomer), CancellationToken.None);

        _mediatorMock.Verify(x => x.Publish(It.IsAny<CustomerAddressUpdated>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenTheAddressIsNotUpdated_ThenEventShouldNotBePublished()
    {
        var existingCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(existingCustomer);

        var handler = GetHandler();

        await handler.Handle(new UpdateCustomer(existingCustomer), CancellationToken.None);

        _mediatorMock.Verify(x => x.Publish(It.IsAny<CustomerAddressUpdated>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WhenTheCustomerIsUpdated_ThenShouldReturnSuccess()
    {
        var existingCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(existingCustomer);
        var handler = GetHandler();

        var cmdResult = await handler.Handle(new UpdateCustomer(existingCustomer), CancellationToken.None);

        Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
    }

    [Fact]
    public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict()
    {
        var existingCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ThrowsAsync(new ConcurrencyException());
        var handler = GetHandler();

        var cmdResult = await handler.Handle(new UpdateCustomer(existingCustomer), CancellationToken.None);

        Assert.Equal(CommandOutcome.Conflict, cmdResult.Outcome);
    }

    private UpdateCustomerHandler GetHandler() =>
       null;

    private Customer GenerateCustomer() =>
        _fixture.Build<Customer>()
            .With(x => x.ShippingAddress)
            .With(x => x.Id)
            .Create();
}