using Ardalis.Specification;
using Domain.Entities;

namespace Application.UseCases.OrderItems.Specifications
{
    public class GetOrderItemByIdSpecification : Specification<OrderItem>
    {
        public GetOrderItemByIdSpecification(int id) 
        {
            Query.Where(x => x.Id == id);
        }
    }
}
