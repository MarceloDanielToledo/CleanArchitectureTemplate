using Azure;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using Shared.Helpers;
using System.Net.Http.Json;

namespace Presentation.IntegrationTests.UseCases
{
    public class ProductsModuleTests : TestBase
    {
        [Test]
        public async Task GetProducts()
        {
            // Arrange
            var testCategory = await AddAsync(ProductHelper.Create());
            var client = Application.CreateClient();

            // Act
            var productResponse = await client.GetFromJsonAsync<Response<Product>>("/api/products/1");

            // Assert
            productResponse.Should().NotBeNull();
            productResponse.Should().Be(2);
        }

    }
}
