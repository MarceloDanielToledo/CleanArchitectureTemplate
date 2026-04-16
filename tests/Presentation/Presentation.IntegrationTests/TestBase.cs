using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Repository.Contexts;

namespace Presentation.IntegrationTests
{
    public class TestBase
    {
        protected ApiWebApplication Application = null!;

        [OneTimeSetUp]
        public async Task RunBeforeAnyTestsAsync()
        {
            Application = new ApiWebApplication();

            using var scope = Application.Services.CreateScope();
            await EnsureDatabaseAsync(scope);
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            Application.Dispose();
        }

        [SetUp]
        public async Task Setup()
        {
            await ResetStateAsync();
        }

        protected async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            using var scope = Application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        protected async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            using var scope = Application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.FindAsync<TEntity>(keyValues);
        }

        protected HttpClient CreateClient() => Application.CreateClient();

        private static async Task EnsureDatabaseAsync(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        private async Task ResetStateAsync()
        {
            using var scope = Application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
