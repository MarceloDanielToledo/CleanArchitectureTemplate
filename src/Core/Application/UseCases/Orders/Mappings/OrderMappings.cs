using Application.UseCases.OrderItems.Mappings;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Domain.Entities;

namespace Application.UseCases.Orders.Mappings
{
    public static class OrderMappings
    {
        public static Order ToEntity(this CreateOrderRequest request) => new()
        {
            Comment = request.Comment,
            OrderItems = request.OrderItems?.Select(i => i.ToEntity()).ToList()
        };

        public static OrderResponse ToResponse(this Order order) => new()
        {
            Id = order.Id,
            Comment = order.Comment,
            TotalAmount = order.OrderItems != null
                ? order.OrderItems.Sum(item => item.UnitPrice * item.Quantity)
                : 0,
            OrderItems = order.OrderItems?.Select(i => i.ToResponse()).ToList()
        };
    }
}
