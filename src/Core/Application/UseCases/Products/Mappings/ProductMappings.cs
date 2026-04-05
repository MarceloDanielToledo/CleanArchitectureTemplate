using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Domain.Entities;

namespace Application.UseCases.Products.Mappings
{
    public static class ProductMappings
    {
        public static Product ToEntity(this CreateProductRequest request) => new()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        public static Product ToEntity(this EditProductRequest request) => new()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = request.IsActive
        };

        public static ProductResponse ToResponse(this Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive
        };
    }
}
