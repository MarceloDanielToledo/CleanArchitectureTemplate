using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Products.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductRequest, Product>();
            CreateMap<EditProductRequest, Product>();
            CreateMap<ProductResponse, Product>().ReverseMap();
        }
    }
}
