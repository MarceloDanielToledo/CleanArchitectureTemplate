using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.UseCases.Products.Specifications;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.CommandHandlers
{
    public class EditProductCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Product>> _repositoryAsyncMock;

        public EditProductCommandHandlerTests()
        {
            _repositoryAsyncMock = new Mock<IRepositoryAsync<Product>>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenProductIsUpdatedSuccessfully()
        {
            // Arrange
            var editProductRequest = new EditProductRequest
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150,
                StockQuantity = 50,
                IsActive = true
            };
            var command = new EditProductCommand(editProductRequest);

            var product = new Product
            {
                Id = editProductRequest.Id,
                Name = "Original Product",
                Description = "Original Description",
                Price = 100,
                StockQuantity = 30,
                IsActive = true
            };

            // Mock the repository behavior
            _repositoryAsyncMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(product);
            _repositoryAsyncMock.Setup(x => x.UpdateAsync(product, CancellationToken.None))
                .Returns(Task.CompletedTask);

            var handler = new EditProductCommandHandler(_repositoryAsyncMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.UpdatedSuccessfullyMessage, result.Message);
            Assert.Equal(editProductRequest.Name, result.Data.Name);
            Assert.Equal(editProductRequest.Price, result.Data.Price);
        }

        [Fact]
        public async Task Handle_Should_ThrowKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var editProductRequest = new EditProductRequest
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150,
                StockQuantity = 50,
                IsActive = true
            };
            var command = new EditProductCommand(editProductRequest);

            // Mock the repository to return null (product not found)
            _repositoryAsyncMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync((Product)null);

            var handler = new EditProductCommandHandler(_repositoryAsyncMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal(ResponseMessages.NotFoundMessage, exception.Message);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenRepositoryThrowsError()
        {
            // Arrange
            var editProductRequest = new EditProductRequest
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150,
                StockQuantity = 50,
                IsActive = true
            };
            var command = new EditProductCommand(editProductRequest);

            var product = new Product
            {
                Id = editProductRequest.Id,
                Name = "Original Product",
                Description = "Original Description",
                Price = 100,
                StockQuantity = 30,
                IsActive = true
            };

            // Mock the repository to throw an exception during the update
            _repositoryAsyncMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(product);
            _repositoryAsyncMock.Setup(x => x.UpdateAsync(product, CancellationToken.None))
                .ThrowsAsync(new Exception("Database error"));

            var handler = new EditProductCommandHandler(_repositoryAsyncMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_Should_MapProductCorrectly_WhenRequestIsValid()
        {
            // Arrange
            var editProductRequest = new EditProductRequest
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150,
                StockQuantity = 50,
                IsActive = true
            };
            var command = new EditProductCommand(editProductRequest);

            var product = new Product
            {
                Id = editProductRequest.Id,
                Name = "Original Product",
                Description = "Original Description",
                Price = 100,
                StockQuantity = 30,
                IsActive = true
            };

            // Mock the repository behavior
            _repositoryAsyncMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(product);

            var handler = new EditProductCommandHandler(_repositoryAsyncMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(editProductRequest.Name, result.Data.Name);
        }
    }


}
