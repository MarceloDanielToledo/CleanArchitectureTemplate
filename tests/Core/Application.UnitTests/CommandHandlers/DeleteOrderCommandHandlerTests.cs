using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Specifications;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.CommandHandlers
{
    public class DeleteOrderCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Order>> _orderRepositoryMock;

        public DeleteOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepositoryAsync<Order>>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenOrderIsDeletedSuccessfully()
        {
            // Arrange
            var orderId = 1;
            var command = new DeleteOrderCommand(orderId);
            var existingOrder = new Order { Id = orderId };

            // Mock repository behavior
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);
            _orderRepositoryMock.Setup(x => x.DeleteAsync(existingOrder, CancellationToken.None))
                .Returns(Task.CompletedTask);

            var handler = new DelteOrderCommandHandler(_orderRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.SuccessMessage, result.Message);
        }

        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            var command = new DeleteOrderCommand(orderId);

            // Mock repository to return null (order not found)
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync((Order)null);

            var handler = new DelteOrderCommandHandler(_orderRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal(ResponseMessages.NotFoundMessage, exception.Message);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenDeleteFails()
        {
            // Arrange
            var orderId = 1;
            var command = new DeleteOrderCommand(orderId);
            var existingOrder = new Order { Id = orderId };

            // Mock repository to throw an exception during delete
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);
            _orderRepositoryMock.Setup(x => x.DeleteAsync(existingOrder, CancellationToken.None))
                .ThrowsAsync(new Exception("Error deleting order"));

            var handler = new DelteOrderCommandHandler(_orderRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Error deleting order", exception.Message);
        }
    }

}
