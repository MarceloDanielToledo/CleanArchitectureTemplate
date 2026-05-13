using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Orders.Specifications;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.CommandHandlers
{
    public class EditOrderCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Order>> _orderRepositoryMock;

        public EditOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepositoryAsync<Order>>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenOrderIsUpdatedSuccessfully()
        {
            // Arrange
            var editOrderRequest = new EditOrderRequest
            {
                Id = 1,
                Comment = "Updated order comment"
            };
            var command = new EditOrderCommand(editOrderRequest);

            var existingOrder = new Order { Id = 1, Comment = "Original comment" };

            // Mock the repository behavior
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);
            _orderRepositoryMock.Setup(x => x.UpdateAsync(existingOrder, CancellationToken.None))
                .ReturnsAsync(0);

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.UpdatedSuccessfullyMessage, result.Message);
            Assert.Equal("Updated order comment", result.Data.Comment);
        }

        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var editOrderRequest = new EditOrderRequest
            {
                Id = 1,
                Comment = "Updated order comment"
            };
            var command = new EditOrderCommand(editOrderRequest);

            // Mock repository to return null (order not found)
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync((Order)null);

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.HandleAsync(command, CancellationToken.None));
            Assert.Equal(ResponseMessages.NotFoundMessage, exception.Message);
        }

        [Fact]
        public async Task Handle_Should_UpdateOrderCommentCorrectly_WhenValidRequest()
        {
            // Arrange
            var editOrderRequest = new EditOrderRequest
            {
                Id = 1,
                Comment = "Updated order comment"
            };
            var command = new EditOrderCommand(editOrderRequest);

            var existingOrder = new Order { Id = 1, Comment = "Original comment" };

            // Mock the repository
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);
            _orderRepositoryMock.Setup(x => x.UpdateAsync(existingOrder, CancellationToken.None))
                .ReturnsAsync(0);

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object);

            // Act
            await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Order>(o => o.Comment == "Updated order comment"), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenUpdateFails()
        {
            // Arrange
            var editOrderRequest = new EditOrderRequest
            {
                Id = 1,
                Comment = "Updated order comment"
            };
            var command = new EditOrderCommand(editOrderRequest);

            var existingOrder = new Order { Id = 1, Comment = "Original comment" };

            // Mock the repository to throw an exception during update
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);
            _orderRepositoryMock.Setup(x => x.UpdateAsync(existingOrder, CancellationToken.None))
                .ThrowsAsync(new Exception("Error updating order"));

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(command, CancellationToken.None));
            Assert.Equal("Error updating order", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_Should_VerifyUpdateCalledOnce()
        {
            // Arrange
            var editOrderRequest = new EditOrderRequest
            {
                Id = 1,
                Comment = "Updated order comment"
            };
            var command = new EditOrderCommand(editOrderRequest);

            var existingOrder = new Order { Id = 1, Comment = "Original comment" };

            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);
            _orderRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>(), CancellationToken.None))
                .ReturnsAsync(0);

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object);

            // Act
            await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Order>(), CancellationToken.None), Times.Once);
        }
    }

}
