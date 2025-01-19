using Domain.Entities;

namespace Shared.Helpers
{
    public static class ProductHelper
    {
        public static Product Create()
        {
            return new Product()
            {
                StockQuantity = 10,
                Description ="Description",
                IsActive = true,
                Name = "Name",
                Price = 10
            };
        }
    }
}
