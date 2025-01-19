using Application.UseCases.Products.Responses;
using Application.Wrappers;
using MediatR;

namespace Application.UseCases.Products.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
