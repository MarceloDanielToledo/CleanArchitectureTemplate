using Domain.Entities;

namespace Shared.Helpers
{
    public class OrderHelper
    {
        public static Order Create()
        {
            return new Order()
            {
                Comment = "Comment",
                OrderItems = OrderItemHelper.CreateList(5),
            };
        } 
    }
}
