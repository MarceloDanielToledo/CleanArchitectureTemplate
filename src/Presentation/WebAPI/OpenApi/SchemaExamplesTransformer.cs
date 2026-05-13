using Application.UseCases.OrderItems.Requests;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Products.Requests;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace WebAPI.OpenApi;

internal sealed class SchemaExamplesTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        schema.Example = context.JsonTypeInfo.Type switch
        {
            var t when t == typeof(CreateProductRequest) => new JsonObject
            {
                ["name"] = "Wireless Keyboard",
                ["description"] = "Ergonomic wireless keyboard with RGB backlight",
                ["price"] = 89.99,
                ["stockQuantity"] = 50
            },
            var t when t == typeof(EditProductRequest) => new JsonObject
            {
                ["id"] = 1,
                ["name"] = "Wireless Keyboard Pro",
                ["description"] = "Ergonomic wireless keyboard with RGB backlight",
                ["price"] = 99.99,
                ["stockQuantity"] = 45,
                ["isActive"] = true
            },
            var t when t == typeof(CreateOrderRequest) => new JsonObject
            {
                ["comment"] = "Please deliver between 9am and 6pm",
                ["orderItems"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["productId"] = 1,
                        ["quantity"] = 2,
                        ["unitPrice"] = 89.99
                    },
                    new JsonObject
                    {
                        ["productId"] = 3,
                        ["quantity"] = 1,
                        ["unitPrice"] = 199.99
                    }
                }
            },
            var t when t == typeof(EditOrderRequest) => new JsonObject
            {
                ["id"] = 1,
                ["comment"] = "Updated delivery instructions: ring doorbell twice"
            },
            var t when t == typeof(CreateOrderItemRequest) => new JsonObject
            {
                ["productId"] = 1,
                ["quantity"] = 2,
                ["unitPrice"] = 89.99
            },
            var t when t == typeof(EditOrderItemRequest) => new JsonObject
            {
                ["id"] = 1,
                ["productId"] = 2,
                ["quantity"] = 3,
                ["unitPrice"] = 74.50
            },
            _ => null
        };

        return Task.CompletedTask;
    }
}
