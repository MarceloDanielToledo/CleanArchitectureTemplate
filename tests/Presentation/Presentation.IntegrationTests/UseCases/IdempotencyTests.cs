using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.Wrappers;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Repository.Contexts;
using System.Net;
using System.Net.Http.Json;

namespace Presentation.IntegrationTests.UseCases
{
    public class IdempotencyTests : TestBase
    {
        private const string ProductsRoute = "/api/v1/products";
        private const string HeaderName = "Idempotency-Key";
        private const string ReplayedHeaderName = "Idempotent-Replayed";

        [Test]
        public async Task CreateProduct_Should_ReplayResponse_WhenSameKeyIsRetried()
        {
            // Arrange
            var client = CreateClient();
            var key = Guid.NewGuid().ToString();
            var request = CreateRequest("Idempotent product");

            // Act
            var first = await PostAsync(client, key, request);
            var second = await PostAsync(client, key, request);

            // Assert
            first.StatusCode.Should().Be(HttpStatusCode.Created);
            second.StatusCode.Should().Be(HttpStatusCode.Created);
            first.Headers.Contains(ReplayedHeaderName).Should().BeFalse();
            second.Headers.GetValues(ReplayedHeaderName).Should().ContainSingle().Which.Should().Be("true");

            var firstBody = await first.Content.ReadFromJsonAsync<Response<ProductResponse>>();
            var secondBody = await second.Content.ReadFromJsonAsync<Response<ProductResponse>>();
            secondBody!.Data!.Id.Should().Be(firstBody!.Data!.Id);
            (await CountProductsAsync("Idempotent product")).Should().Be(1);
        }

        [Test]
        public async Task CreateProduct_Should_Return422_WhenKeyIsReusedWithDifferentPayload()
        {
            // Arrange
            var client = CreateClient();
            var key = Guid.NewGuid().ToString();
            await PostAsync(client, key, CreateRequest("First payload"));

            // Act
            var response = await PostAsync(client, key, CreateRequest("Second payload"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            (await CountProductsAsync("Second payload")).Should().Be(0);
        }

        [Test]
        public async Task CreateProduct_Should_ReleaseKey_WhenRequestFails()
        {
            // Arrange
            var client = CreateClient();
            var key = Guid.NewGuid().ToString();
            var invalidRequest = new CreateProductRequest();

            // Act
            var first = await PostAsync(client, key, invalidRequest);
            var second = await PostAsync(client, key, invalidRequest);

            // Assert
            first.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            second.Headers.Contains(ReplayedHeaderName).Should().BeFalse();
        }

        [Test]
        public async Task CreateProduct_Should_ExecuteEveryTime_WhenNoKeyIsSent()
        {
            // Arrange
            var client = CreateClient();
            var request = CreateRequest("No key product");

            // Act
            await client.PostAsJsonAsync(ProductsRoute, request);
            await client.PostAsJsonAsync(ProductsRoute, request);

            // Assert
            (await CountProductsAsync("No key product")).Should().Be(2);
        }

        [Test]
        public async Task CreateProduct_Should_Return400_WhenKeyIsTooLong()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await PostAsync(client, new string('k', 101), CreateRequest("Long key"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static CreateProductRequest CreateRequest(string name) => new()
        {
            Name = name,
            Description = "Created by idempotency tests",
            Price = 10m,
            StockQuantity = 1
        };

        private static Task<HttpResponseMessage> PostAsync(HttpClient client, string key, CreateProductRequest request)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, ProductsRoute)
            {
                Content = JsonContent.Create(request)
            };
            message.Headers.Add(HeaderName, key);
            return client.SendAsync(message);
        }

        private async Task<int> CountProductsAsync(string name)
        {
            using var scope = Application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.Set<Product>().CountAsync(p => p.Name == name);
        }
    }
}
