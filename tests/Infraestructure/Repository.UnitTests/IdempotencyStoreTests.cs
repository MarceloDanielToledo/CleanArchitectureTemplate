using Application.Interfaces;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Repository.Contexts;
using Repository.Options;
using Repository.Services;

namespace Repository.UnitTests
{
    public class IdempotencyStoreTests
    {
        private const string Key = "key-1";
        private const string Hash = "hash-1";

        private readonly Mock<IDateTimeService> _dateTimeServiceMock = new();
        private readonly ServiceProvider _serviceProvider;
        private readonly IdempotencyStore _store;
        private DateTime _now = new(2026, 1, 1, 12, 0, 0);

        public IdempotencyStoreTests()
        {
            _dateTimeServiceMock.Setup(x => x.Now).Returns(() => _now);

            var databaseName = Guid.NewGuid().ToString();
            var services = new ServiceCollection();
            services.AddSingleton(_dateTimeServiceMock.Object);
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
            _serviceProvider = services.BuildServiceProvider();

            _store = new IdempotencyStore(
                _serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                _dateTimeServiceMock.Object,
                Microsoft.Extensions.Options.Options.Create(new IdempotencyOptions
                {
                    Expiration = TimeSpan.FromHours(1),
                    InProgressTimeout = TimeSpan.FromMinutes(2),
                }));
        }

        [Fact]
        public async Task TryBegin_Should_ReserveKey_When_KeyIsNew()
        {
            // Act
            var existing = await _store.TryBeginAsync(Key, Hash);

            // Assert
            Assert.Null(existing);
            var record = await FindAsync(Key);
            Assert.NotNull(record);
            Assert.False(record.IsCompleted);
        }

        [Fact]
        public async Task TryBegin_Should_ReturnInProgressRecord_When_KeyIsAlreadyReserved()
        {
            // Arrange
            await _store.TryBeginAsync(Key, Hash);

            // Act
            var existing = await _store.TryBeginAsync(Key, Hash);

            // Assert
            Assert.NotNull(existing);
            Assert.False(existing.IsCompleted);
            Assert.Equal(Hash, existing.RequestHash);
        }

        [Fact]
        public async Task TryBegin_Should_ReturnCompletedRecord_When_KeyWasCompleted()
        {
            // Arrange
            await _store.TryBeginAsync(Key, Hash);
            await _store.CompleteAsync(Key, 201, "{\"id\":1}");

            // Act
            var existing = await _store.TryBeginAsync(Key, Hash);

            // Assert
            Assert.NotNull(existing);
            Assert.True(existing.IsCompleted);
            Assert.Equal(201, existing.StatusCode);
            Assert.Equal("{\"id\":1}", existing.ResponseBody);
        }

        [Fact]
        public async Task TryBegin_Should_ReserveAgain_When_KeyWasReleased()
        {
            // Arrange
            await _store.TryBeginAsync(Key, Hash);
            await _store.ReleaseAsync(Key);

            // Act
            var existing = await _store.TryBeginAsync(Key, Hash);

            // Assert
            Assert.Null(existing);
        }

        [Fact]
        public async Task TryBegin_Should_ReserveAgain_When_InProgressReservationIsAbandoned()
        {
            // Arrange
            await _store.TryBeginAsync(Key, Hash);
            _now = _now.AddMinutes(3);

            // Act
            var existing = await _store.TryBeginAsync(Key, "hash-2");

            // Assert
            Assert.Null(existing);
            Assert.Equal("hash-2", (await FindAsync(Key))!.RequestHash);
        }

        [Fact]
        public async Task TryBegin_Should_ReserveAgain_When_CompletedRecordExpired()
        {
            // Arrange
            await _store.TryBeginAsync(Key, Hash);
            await _store.CompleteAsync(Key, 201, "{}");
            _now = _now.AddHours(2);

            // Act
            var existing = await _store.TryBeginAsync(Key, Hash);

            // Assert
            Assert.Null(existing);
            Assert.False((await FindAsync(Key))!.IsCompleted);
        }

        [Fact]
        public async Task Release_Should_NotRemoveCompletedRecord()
        {
            // Arrange
            await _store.TryBeginAsync(Key, Hash);
            await _store.CompleteAsync(Key, 201, "{}");

            // Act
            await _store.ReleaseAsync(Key);

            // Assert
            Assert.NotNull(await FindAsync(Key));
        }

        private async Task<IdempotencyRecord?> FindAsync(string key)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.Set<IdempotencyRecord>().AsNoTracking().FirstOrDefaultAsync(r => r.Key == key);
        }
    }
}
