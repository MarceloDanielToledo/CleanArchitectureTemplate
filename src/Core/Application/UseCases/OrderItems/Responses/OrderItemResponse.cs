using Application.UseCases.Orders.Responses;
using Application.UseCases.Products.Responses;

namespace Application.UseCases.OrderItems.Responses
{
    public class OrderItemResponse
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderResponse? Order { get; set; }
        public int ProductId { get; set; }
        public ProductResponse? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
