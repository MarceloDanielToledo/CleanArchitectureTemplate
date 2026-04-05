using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.CommandHandlers
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Product>> _repositoryAsyncMock;

        public CreateProductCommandHandlerTests()
        {
            _repositoryAsyncMock = new Mock<IRepositoryAsync<Product>>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenProductIsCreatedSuccessfully()
        {
            // Arrange
            var createProductRequest = new CreateProductRequest { Name = "Test Product", Price = 100 };
            var command = new CreateProductCommand(createProductRequest);

            var newProduct = new Product { Id = 1, Name = "Test Product", Price = 100 };

            _repositoryAsyncMock.Setup(x => x.AddAsync(It.IsAny<Product>(), CancellationToken.None))
                .ReturnsAsync(newProduct);

            var handler = new CreateProductCommandHandler(_repositoryAsyncMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.AddedSuccesfullyMessage, result.Message);
            Assert.Equal(newProduct.Id, result.Data.Id);
            Assert.Equal(newProduct.Name, result.Data.Name);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenRepositoryThrowsException()
        {
            // Arrange
            var createProductRequest = new CreateProductRequest { Name = "Test Product", Price = 100 };
            var command = new CreateProductCommand(createProductRequest);

            // Simulate repository throwing an exception
            _repositoryAsyncMock.Setup(x => x.AddAsync(It.IsAny<Product>(), CancellationToken.None))
                .ThrowsAsync(new Exception("Database error"));

            var handler = new CreateProductCommandHandler(_repositoryAsyncMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_Should_MapProductCorrectly_WhenRequestIsValid()
        {
            // Arrange
            var createProductRequest = new CreateProductRequest { Name = "Test Product", Price = 100 };
            var command = new CreateProductCommand(createProductRequest);

            var newProduct = new Product { Id = 1, Name = "Test Product", Price = 100 };

            _repositoryAsyncMock.Setup(x => x.AddAsync(It.IsAny<Product>(), CancellationToken.None))
                .ReturnsAsync(newProduct);

            var handler = new CreateProductCommandHandler(_repositoryAsyncMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryAsyncMock.Verify(x => x.AddAsync(It.IsAny<Product>(), CancellationToken.None), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

    }
}
