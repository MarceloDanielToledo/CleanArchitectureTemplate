using Domain.Entities;

namespace Shared.Helpers
{
    public static class OrderItemHelper
    {
        public static OrderItem Create()
        {
            return new OrderItem()
            {
                OrderId = 1,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 1,
            };
        }
        public static List<OrderItem> CreateList(int elements)
        {
            var records = new List<OrderItem>();
            for (int i = 0; i < elements; i++)
            {
                records.Add(Create());
            }
            return records;
        }
    }
}
