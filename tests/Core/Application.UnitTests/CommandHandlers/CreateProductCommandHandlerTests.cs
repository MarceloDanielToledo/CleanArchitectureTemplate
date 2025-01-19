using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.CommandHandlers
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Product>> _repositoryAsyncMock;
        private readonly Mock<IMapper> _mapperMock;

        public CreateProductCommandHandlerTests()
        {
            _repositoryAsyncMock = new Mock<IRepositoryAsync<Product>>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenProductIsCreatedSuccessfully()
        {
            // Arrange
            var createProductRequest = new CreateProductRequest { Name = "Test Product", Price = 100 };
            var command = new CreateProductCommand(createProductRequest);

            var newProduct = new Product { Id = 1, Name = "Test Product", Price = 100 };
            var newProductResponse = new ProductResponse { Id = newProduct.Id, Name = "Test Product", Price = 100 };

            // Mock mappings and repository call
            _mapperMock.Setup(x => x.Map<Product>(createProductRequest))
                .Returns(newProduct);
            _repositoryAsyncMock.Setup(x => x.AddAsync(newProduct, CancellationToken.None))
                .ReturnsAsync(newProduct);
            _mapperMock.Setup(x => x.Map<ProductResponse>(newProduct))
                .Returns(newProductResponse);

            var handler = new CreateProductCommandHandler(_repositoryAsyncMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(ResponseMessages.AddedSuccesfullyMessage, result.Message);
            Assert.Equal(newProductResponse, result.Data);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenRepositoryThrowsException()
        {
            // Arrange
            var createProductRequest = new CreateProductRequest { Name = "Test Product", Price = 100 };
            var command = new CreateProductCommand(createProductRequest);

            var newProduct = new Product { Id = 1, Name = "Test Product", Price = 100 };

            // Simulate repository throwing an exception
            _mapperMock.Setup(x => x.Map<Product>(createProductRequest))
                .Returns(newProduct);
            _repositoryAsyncMock.Setup(x => x.AddAsync(newProduct, CancellationToken.None))
                .ThrowsAsync(new Exception("Database error"));

            var handler = new CreateProductCommandHandler(_repositoryAsyncMock.Object, _mapperMock.Object);

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

            // Mock the mapping
            _mapperMock.Setup(x => x.Map<Product>(createProductRequest))
                .Returns(newProduct);

            _repositoryAsyncMock.Setup(x => x.AddAsync(newProduct, CancellationToken.None))
                .ReturnsAsync(newProduct);

            var handler = new CreateProductCommandHandler(_repositoryAsyncMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(x => x.Map<Product>(createProductRequest), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

    }
}
