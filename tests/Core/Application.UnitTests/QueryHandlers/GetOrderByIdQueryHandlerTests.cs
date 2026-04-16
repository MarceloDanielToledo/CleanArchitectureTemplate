using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Queries;
using Application.UseCases.Orders.Specifications;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.QueryHandlers
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Order>> _orderRepositoryMock;

        public GetOrderByIdQueryHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepositoryAsync<Order>>();
        }

        [Fact]
        public async Task HandleAsync_Should_ReturnSuccess_WhenOrderFound()
        {
            // Arrange
            var orderId = 1;
            var query = new GetOrderByIdQuery(orderId);
            var existingOrder = new Order
            {
                Id = orderId,
                Comment = "Test order",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Id = 1, OrderId = orderId, ProductId = 1, Quantity = 2, UnitPrice = 50m }
                }
            };

            _orderRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);

            var handler = new GetOrderByIdQueryHandler(_orderRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(existingOrder.Id, result.Data.Id);
            Assert.Equal(existingOrder.Comment, result.Data.Comment);
            Assert.Equal(100m, result.Data.TotalAmount);
            Assert.NotNull(result.Data.OrderItems);
            Assert.Single(result.Data.OrderItems);
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 99;
            var query = new GetOrderByIdQuery(orderId);

            _orderRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync((Order)null);

            var handler = new GetOrderByIdQueryHandler(_orderRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(query, CancellationToken.None));
            Assert.Equal(ResponseMessages.NotFoundMessage, exception.Message);
        }

        [Fact]
        public async Task HandleAsync_Should_ReturnZeroTotalAmount_WhenOrderHasNoItems()
        {
            // Arrange
            var orderId = 1;
            var query = new GetOrderByIdQuery(orderId);
            var existingOrder = new Order
            {
                Id = orderId,
                Comment = "Empty order",
                OrderItems = new List<OrderItem>()
            };

            _orderRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);

            var handler = new GetOrderByIdQueryHandler(_orderRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(0m, result.Data.TotalAmount);
            Assert.Empty(result.Data.OrderItems);
        }

        [Fact]
        public async Task HandleAsync_Should_CallRepositoryExactlyOnce_WhenQueryIsHandled()
        {
            // Arrange
            var orderId = 1;
            var query = new GetOrderByIdQuery(orderId);
            var existingOrder = new Order { Id = orderId, Comment = "Test" };

            _orderRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);

            var handler = new GetOrderByIdQueryHandler(_orderRepositoryMock.Object);

            // Act
            await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            _orderRepositoryMock.Verify(
                x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None),
                Times.Once);
        }
    }
}
