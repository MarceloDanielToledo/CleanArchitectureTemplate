using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.Wrappers;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using Shared.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Presentation.IntegrationTests.UseCases
{
    public class ProductsModuleTests : TestBase
    {
        private const string ProductsRoute = "/api/v1/products";

        [Test]
        public async Task GetProduct_Should_Return200_WhenProductExists()
        {
            // Arrange
            var product = await AddAsync(ProductHelper.Create());
            var client = Application.CreateClient();

            // Act
            var response = await client.GetAsync($"{ProductsRoute}/{product.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetProduct_Should_Return404_WhenProductDoesNotExist()
        {
            // Arrange
            var client = Application.CreateClient();
            var nonExistentId = 99999;

            // Act
            var response = await client.GetAsync($"{ProductsRoute}/{nonExistentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task CreateProduct_Should_Return201_WhenRequestIsValid()
        {
            // Arrange
            var client = Application.CreateClient();
            var createRequest = new CreateProductRequest
            {
                Name = "Integration Test Product",
                Description = "A product created during integration testing",
                Price = 49.99m,
                StockQuantity = 100
            };

            // Act
            var response = await client.PostAsJsonAsync(ProductsRoute, createRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<Response<ProductResponse>>();
            result.Should().NotBeNull();
            result!.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createRequest.Name);
            result.Data.Description.Should().Be(createRequest.Description);
            result.Data.Price.Should().Be(createRequest.Price);
            result.Data.StockQuantity.Should().Be(createRequest.StockQuantity);
        }
    }
}
