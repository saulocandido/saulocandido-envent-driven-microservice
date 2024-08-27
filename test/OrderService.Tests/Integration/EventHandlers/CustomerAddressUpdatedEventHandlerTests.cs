using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Common.Integration.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Domain.OrderAggregate;
using OrderService.Integration.EventHandlers;
using OrderService.Repositories;
using OrderService.Tests.Helpers;
using Xunit;
using Address = Common.Integration.Models.Address;
using MediatR;

namespace OrderService.Tests.Integration.EventHandlers
{
    public class CustomerAddressUpdatedEventHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<ILogger<CustomerAddressUpdatedEventHandler>> _logger;
        private readonly IMapper _mapper;
        private readonly Mock<IOrderRepository> _repositoryMock;

        public CustomerAddressUpdatedEventHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _mapper = MappingHelper.GetMapper();
            _logger = new Mock<ILogger<CustomerAddressUpdatedEventHandler>>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new CustomerAddressUpdatedEventHandler(_repositoryMock.Object, _mapper, _logger.Object);
            Assert.NotNull(handler);
            Assert.IsAssignableFrom<INotificationHandler<CustomerAddressUpdated>>(handler);
            Assert.IsType<CustomerAddressUpdatedEventHandler>(handler);
        }

        [Fact]
        public async Task WhenEventIsHandled_ThenOrderAddressShouldGetUpdated()
        {
            var address = _fixture.Create<Address>();
            var updatedEvent = new CustomerAddressUpdated(Guid.NewGuid(), address);
            var addressWasUpdated = false;

            _repositoryMock.Setup(x => x.GetByCustomerAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new[] { _fixture.Create<Order>() });
            _repositoryMock.Setup(x => x.UpdateAddressAsync(It.IsAny<Guid>(), It.IsAny<OrderService.Domain.OrderAggregate.Address>()))
                .Callback<Guid, OrderService.Domain.OrderAggregate.Address>((_, _) => { addressWasUpdated = true; });

            var handler = new CustomerAddressUpdatedEventHandler(_repositoryMock.Object, _mapper, _logger.Object);
            await handler.HandleAsync(updatedEvent);

            Assert.True(addressWasUpdated);
        }
    }
}