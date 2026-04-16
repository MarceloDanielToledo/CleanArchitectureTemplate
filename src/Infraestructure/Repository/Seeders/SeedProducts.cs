using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Repository.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Seeders
{
    public static class SeedProducts
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (!context.Products.Any())
            {
                context.Products.AddRange(GetProducts());

                await context.SaveChangesAsync();
            }
        }
        private static List<Product> GetProducts()
        {
            Random random = new();
            List<Product> records = [];
            for (int i = 0; i < 10; i++)
            {
                records.Add(new()
                {

                    Name = $"Product #{i}",
                    StockQuantity = random.Next(10, 100),
                    Description = "Description",
                    IsActive = true,
                    Price = random.Next(1000, 10000)
                });
            }
            return records;

        }
    }
}
