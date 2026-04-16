using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Queries;
using Application.UseCases.OrderItems.Specifications;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.QueryHandlers
{
    public class GetOrderItemByIdQueryHandlerTests
    {
        private readonly Mock<IRepositoryAsync<OrderItem>> _orderItemRepositoryMock;

        public GetOrderItemByIdQueryHandlerTests()
        {
            _orderItemRepositoryMock = new Mock<IRepositoryAsync<OrderItem>>();
        }

        [Fact]
        public async Task HandleAsync_Should_ReturnSuccess_WhenOrderItemFound()
        {
            // Arrange
            var orderId = 1;
            var itemId = 10;
            var query = new GetOrderItemByIdQuery(orderId, itemId);
            var existingItem = new OrderItem
            {
                Id = itemId,
                OrderId = orderId,
                ProductId = 5,
                Quantity = 3,
                UnitPrice = 25.00m
            };

            _orderItemRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderItemByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingItem);

            var handler = new GetOrderItemByIdQueryHandler(_orderItemRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(existingItem.Id, result.Data.Id);
            Assert.Equal(existingItem.OrderId, result.Data.OrderId);
            Assert.Equal(existingItem.ProductId, result.Data.ProductId);
            Assert.Equal(existingItem.Quantity, result.Data.Quantity);
            Assert.Equal(existingItem.UnitPrice, result.Data.UnitPrice);
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenOrderItemDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            var itemId = 99;
            var query = new GetOrderItemByIdQuery(orderId, itemId);

            _orderItemRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderItemByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync((OrderItem)null);

            var handler = new GetOrderItemByIdQueryHandler(_orderItemRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(query, CancellationToken.None));
            Assert.Equal(ResponseMessages.NotFoundMessage, exception.Message);
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenOrderItemBelongsToDifferentOrder()
        {
            // Arrange
            var requestedOrderId = 1;
            var actualOrderId = 2;
            var itemId = 10;
            var query = new GetOrderItemByIdQuery(requestedOrderId, itemId);
            var existingItem = new OrderItem
            {
                Id = itemId,
                OrderId = actualOrderId,
                ProductId = 5,
                Quantity = 1,
                UnitPrice = 10m
            };

            _orderItemRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderItemByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingItem);

            var handler = new GetOrderItemByIdQueryHandler(_orderItemRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(query, CancellationToken.None));
            Assert.Equal(ResponseMessages.NotFoundMessage, exception.Message);
        }

        [Fact]
        public async Task HandleAsync_Should_CallRepositoryExactlyOnce_WhenQueryIsHandled()
        {
            // Arrange
            var orderId = 1;
            var itemId = 10;
            var query = new GetOrderItemByIdQuery(orderId, itemId);
            var existingItem = new OrderItem { Id = itemId, OrderId = orderId, ProductId = 1 };

            _orderItemRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderItemByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingItem);

            var handler = new GetOrderItemByIdQueryHandler(_orderItemRepositoryMock.Object);

            // Act
            await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            _orderItemRepositoryMock.Verify(
                x => x.FirstOrDefaultAsync(It.IsAny<GetOrderItemByIdSpecification>(), CancellationToken.None),
                Times.Once);
        }
    }
}
