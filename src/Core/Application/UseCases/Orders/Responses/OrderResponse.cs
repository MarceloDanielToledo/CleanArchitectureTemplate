using Application.UseCases.OrderItems.Responses;

namespace Application.UseCases.Orders.Responses
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Comment { get; set; }
        public List<OrderItemResponse>? OrderItems { get; set; }

    }
}
