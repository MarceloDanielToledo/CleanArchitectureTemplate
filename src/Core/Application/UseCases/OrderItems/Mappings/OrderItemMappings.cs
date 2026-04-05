using Application.UseCases.OrderItems.Requests;
using Application.UseCases.OrderItems.Responses;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Products.Responses;
using Domain.Entities;

namespace Application.UseCases.OrderItems.Mappings
{
    public static class OrderItemMappings
    {
        public static OrderItem ToEntity(this CreateOrderItemRequest request) => new()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };

        public static OrderItemResponse ToResponse(this OrderItem orderItem) => new()
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            ProductId = orderItem.ProductId,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            Product = orderItem.Product != null ? new ProductResponse
            {
                Id = orderItem.Product.Id,
                Name = orderItem.Product.Name,
                Description = orderItem.Product.Description,
                Price = orderItem.Product.Price,
                StockQuantity = orderItem.Product.StockQuantity,
                IsActive = orderItem.Product.IsActive
            } : null
        };
    }
}
