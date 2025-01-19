using Application.UseCases.OrderItems.Requests;

namespace Application.UseCases.Orders.Requests
{

    public class CreateOrderRequest
    {
        public string Comment { get; set; }
        public List<CreateOrderItemRequest> OrderItems { get; set; }
    }
}
