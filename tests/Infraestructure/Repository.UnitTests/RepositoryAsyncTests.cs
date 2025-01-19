using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository.Contexts;
using Repository.Services;
using Shared.Helpers;

namespace Repository.UnitTests
{
    public class RepositoryAsyncTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepositoryAsync<OrderItem> _orderRepository;
        private readonly IRepositoryAsync<OrderItem> _orderItemRepository;
        private readonly IRepositoryAsync<Product> _productRepository;

        public RepositoryAsyncTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options, new Mock<IDateTimeService>().Object);
            _orderRepository = new RepositoryAsync<OrderItem>(_dbContext);
            _orderItemRepository = new RepositoryAsync<OrderItem>(_dbContext);
            _productRepository = new RepositoryAsync<Product>(_dbContext);
        }
        [Fact]
        public async Task Add_Should_AddEntityToDatabase()
        {
            // Arrange
            var record = ProductHelper.Create();

            // Act
            await _productRepository.AddAsync(record);
            await _dbContext.SaveChangesAsync();

            // Assert
            var addeddRecord = await _productRepository.GetByIdAsync(record.Id);
            Assert.NotNull(addeddRecord);
            Assert.Equal("Name", addeddRecord.Name);
        }
        [Fact]
        public async Task Update_Should_UpdateEntityInDatabase()
        {
            // Arrange
            var record = ProductHelper.Create();
            await _productRepository.AddAsync(record);
            await _dbContext.SaveChangesAsync();

            // Act
            record.Name = "New Name";
            await _productRepository.UpdateAsync(record);
            await _dbContext.SaveChangesAsync();

            // Assert
            var updatedRecord = await _productRepository.GetByIdAsync(record.Id);
            Assert.NotNull(record);
            Assert.Equal("New Name", record.Name);
        }
        [Fact]
        public async Task Delete_Should_RemoveEntityFromDatabase()
        {
            // Arrange
            var record = ProductHelper.Create();
            await _productRepository.AddAsync(record);
            await _dbContext.SaveChangesAsync();

            // Act
            await _productRepository.DeleteAsync(record);
            await _dbContext.SaveChangesAsync();

            // Assert
            var deletedRecord = await _productRepository.GetByIdAsync(record.Id);
            Assert.Null(deletedRecord);
        }
    }
}