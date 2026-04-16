using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Contexts;
using Repository.Services;

namespace Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var useInMemory = configuration["Database:UseInMemory"] == "true";

            if (useInMemory)
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("IntegrationTestDb"));
            else
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
        }
    }
}
