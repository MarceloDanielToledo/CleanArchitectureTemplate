using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Contexts;
using Repository.Options;
using Repository.Services;

namespace Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var useInMemory = configuration["Database:UseInMemory"] == "true";
            var resilience = configuration.GetSection(DatabaseResilienceOptions.SectionName).Get<DatabaseResilienceOptions>()
                ?? new DatabaseResilienceOptions();

            if (useInMemory)
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("IntegrationTestDb"));
            else
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, sql =>
                {
                    // Retries transient SQL Server failures (deadlocks, failover, throttling, network blips).
                    sql.EnableRetryOnFailure(
                        maxRetryCount: resilience.MaxRetryCount,
                        maxRetryDelay: resilience.MaxRetryDelay,
                        errorNumbersToAdd: null);
                    sql.CommandTimeout(resilience.CommandTimeoutSeconds);
                }));

            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));

            services.Configure<IdempotencyOptions>(configuration.GetSection(IdempotencyOptions.SectionName));
            services.AddSingleton<IIdempotencyStore, IdempotencyStore>();
        }
    }
}
