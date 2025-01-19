namespace Repository.Seeders
{
    public class SeedData
    {
        public static async Task InitializeDataAsync(IServiceProvider serviceProvider)
        {
            await SeedProducts.Seed(serviceProvider);
        }
    }

}
