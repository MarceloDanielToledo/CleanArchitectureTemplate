using Ardalis.Specification;
using Domain.Entities;

namespace Application.UseCases.Products.Specifications
{
    public class GetProductByIdSpecification : Specification<Product>
    {
        public GetProductByIdSpecification(int id) 
        {
            Query.Where(x => x.Id == id);
        }
    }
}
