using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Requests;
using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.CommandHandlers
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Order>> _orderRepositoryMock;
        private readonly Mock<IRepositoryAsync<Product>> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public CreateOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepositoryAsync<Order>>();
            _productRepositoryMock = new Mock<IRepositoryAsync<Product>>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenOrderIsCreatedSuccessfully()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                Comment = "Test order",
                OrderItems = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 1, Quantity = 2, UnitPrice = 100 }
            }
            };
            var command = new CreateOrderCommand(createOrderRequest);

            var order = new Order { Id = 1, Comment = "Test order" };
            var orderResponse = new OrderResponse { Id = 1, Comment = "Test order" };

            // Mock the repository and mapping behavior
            _mapperMock.Setup(x => x.Map<Order>(createOrderRequest))
                .Returns(order);
            _orderRepositoryMock.Setup(x => x.AddAsync(order, CancellationToken.None))
                .ReturnsAsync(order);
            _mapperMock.Setup(x => x.Map<OrderResponse>(order))
                .Returns(orderResponse);

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.AddedSuccesfullyMessage, result.Message);
            Assert.Equal(orderResponse, result.Data);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenOrderCannotBeCreated()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                Comment = "Test order",
                OrderItems = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 1, Quantity = 2, UnitPrice = 100 }
            }
            };
            var command = new CreateOrderCommand(createOrderRequest);

            var order = new Order { Id = 1, Comment = "Test order" };

            // Mock the repository to throw an exception during the creation
            _mapperMock.Setup(x => x.Map<Order>(createOrderRequest))
                .Returns(order);
            _orderRepositoryMock.Setup(x => x.AddAsync(order, CancellationToken.None))
                .ThrowsAsync(new Exception("Error creating order"));

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Error creating order", exception.Message);
        }

        [Fact]
        public async Task Handle_Should_MapOrderCorrectly_WhenRequestIsValid()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                Comment = "Test order",
                OrderItems = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 1, Quantity = 2, UnitPrice = 100 }
            }
            };
            var command = new CreateOrderCommand(createOrderRequest);

            var order = new Order { Id = 1, Comment = "Test order" };

            // Mock the repository and mapping behavior
            _mapperMock.Setup(x => x.Map<Order>(createOrderRequest))
                .Returns(order);
            _orderRepositoryMock.Setup(x => x.AddAsync(order, CancellationToken.None))
                .ReturnsAsync(order);

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(x => x.Map<Order>(createOrderRequest), Times.Once);
            _mapperMock.Verify(x => x.Map<OrderResponse>(order), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenMappingFails()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                Comment = "Test order",
                OrderItems = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 1, Quantity = 2, UnitPrice = 100 }
            }
            };
            var command = new CreateOrderCommand(createOrderRequest);

            // Mock the mapper to throw an exception
            _mapperMock.Setup(x => x.Map<Order>(createOrderRequest))
                .Throws(new Exception("Mapping error"));

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Mapping error", exception.Message);
        }
    }

}
