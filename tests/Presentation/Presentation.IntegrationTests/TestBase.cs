using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Repository.Contexts;
using Respawn;
using Respawn.Graph;

namespace Presentation.IntegrationTests
{
    public class TestBase
    {
        protected ApiWebApplication Application;

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
            await ResetState();
        }

        [TearDown]
        public void Down()
        {

        }
        protected async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            using var scope = Application.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();

            return entity;
        }


        /// <summary>
        /// Crea un usuario de prueba según los parámetros
        /// </summary>
        /// <returns></returns>
        public async Task<HttpClient> CreateTestUser()
        {
            using var scope = Application.Services.CreateScope();
            var client = Application.CreateClient();
            return (client);
        }


        protected async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            using var scope = Application.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await context.FindAsync<TEntity>(keyValues);
        }
        private static async Task EnsureDatabaseAsync(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
        private async Task ResetState()
        {
            var checkpoint = await Respawner.CreateAsync(ApiWebApplication.TestConnectionString, new RespawnerOptions
            {

                TablesToIgnore = new[] { new Table("__EFMigrationsHistory") }
            });
            await checkpoint.ResetAsync(ApiWebApplication.TestConnectionString);
        }

    }
}
