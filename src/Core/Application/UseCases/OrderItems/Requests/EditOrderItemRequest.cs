using Application.UseCases.Products.Responses;

namespace Application.UseCases.OrderItems.Requests
{
    public class EditOrderItemRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
