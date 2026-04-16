using Ardalis.Specification;
using Domain.Entities;

namespace Application.UseCases.Orders.Specifications
{
    public class GetOrderByIdSpecification : Specification<Order>
    {
        public GetOrderByIdSpecification(int id)
        {
            Query.Where(x => x.Id == id)
                .Include(x => x.OrderItems).AsSplitQuery()
                .AsNoTracking();
        }

    }
}
