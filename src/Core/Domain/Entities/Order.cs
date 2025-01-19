using Domain.Contracts;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public string Comment { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
