using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Requests;
using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.CommandHandlers
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Order>> _orderRepositoryMock;
        private readonly Mock<IRepositoryAsync<Product>> _productRepositoryMock;

        public CreateOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepositoryAsync<Order>>();
            _productRepositoryMock = new Mock<IRepositoryAsync<Product>>();
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

            // Mock the repository behavior
            _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
                .ReturnsAsync(order);

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.AddedSuccesfullyMessage, result.Message);
            Assert.Equal(order.Id, result.Data.Id);
            Assert.Equal(order.Comment, result.Data.Comment);
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

            // Mock the repository to throw an exception during the creation
            _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
                .ThrowsAsync(new Exception("Error creating order"));

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object);

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

            // Mock the repository behavior
            _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
                .ReturnsAsync(order);

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRepositoryThrowsOnAdd()
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

            // Mock the repository to throw an exception
            _orderRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
                .Throws(new Exception("Repository error"));

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Repository error", exception.Message);
        }
    }

}
