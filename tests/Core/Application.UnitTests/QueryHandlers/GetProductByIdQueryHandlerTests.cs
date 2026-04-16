using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Queries;
using Application.UseCases.Products.Specifications;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.QueryHandlers
{
    public class GetProductByIdQueryHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Product>> _productRepositoryMock;

        public GetProductByIdQueryHandlerTests()
        {
            _productRepositoryMock = new Mock<IRepositoryAsync<Product>>();
        }

        [Fact]
        public async Task HandleAsync_Should_ReturnSuccess_WhenProductFound()
        {
            // Arrange
            var productId = 1;
            var query = new GetProductByIdQuery(productId);
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                StockQuantity = 10,
                IsActive = true
            };

            _productRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingProduct);

            var handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(existingProduct.Id, result.Data.Id);
            Assert.Equal(existingProduct.Name, result.Data.Name);
            Assert.Equal(existingProduct.Description, result.Data.Description);
            Assert.Equal(existingProduct.Price, result.Data.Price);
            Assert.Equal(existingProduct.StockQuantity, result.Data.StockQuantity);
            Assert.Equal(existingProduct.IsActive, result.Data.IsActive);
        }

        [Fact]
        public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 99;
            var query = new GetProductByIdQuery(productId);

            _productRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync((Product)null);

            var handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(query, CancellationToken.None));
            Assert.Equal(ResponseMessages.NotFoundMessage, exception.Message);
        }

        [Fact]
        public async Task HandleAsync_Should_CallRepositoryExactlyOnce_WhenQueryIsHandled()
        {
            // Arrange
            var productId = 1;
            var query = new GetProductByIdQuery(productId);
            var existingProduct = new Product { Id = productId, Name = "Test Product" };

            _productRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None))
                .ReturnsAsync(existingProduct);

            var handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);

            // Act
            await handler.HandleAsync(query, CancellationToken.None);

            // Assert
            _productRepositoryMock.Verify(
                x => x.FirstOrDefaultAsync(It.IsAny<GetProductByIdSpecification>(), CancellationToken.None),
                Times.Once);
        }
    }
}
