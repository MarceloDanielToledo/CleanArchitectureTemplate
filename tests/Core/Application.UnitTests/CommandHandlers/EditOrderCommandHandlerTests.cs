using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Orders.Specifications;
using AutoMapper;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.CommandHandlers
{
    public class EditOrderCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Order>> _orderRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public EditOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IRepositoryAsync<Order>>();
            _mapperMock = new Mock<IMapper>();
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
            var updatedOrderResponse = new OrderResponse { Id = 1, Comment = "Updated order comment" };

            // Mock the repository and mapping behavior
            _orderRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetOrderByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingOrder);
            _orderRepositoryMock.Setup(x => x.UpdateAsync(existingOrder, CancellationToken.None))
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<OrderResponse>(existingOrder))
                .Returns(updatedOrderResponse);

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.UpdatedSuccessfullyMessage, result.Message);
            Assert.Equal(updatedOrderResponse, result.Data);
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

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
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
                .Returns(Task.CompletedTask);

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object, _mapperMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

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

            var handler = new EditOrderCommandHandler(_orderRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Error updating order", exception.Message);
        }
    }

}
