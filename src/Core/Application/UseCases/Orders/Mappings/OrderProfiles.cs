using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Orders.Mappings
{
    public class OrderProfiles : Profile
    {
        public OrderProfiles()
        {
            CreateMap<Order, CreateOrderRequest>()
                .ReverseMap();
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.TotalAmount,
                           opt => opt.MapFrom(src =>
                               src.OrderItems != null
                                   ? src.OrderItems.Sum(item => item.UnitPrice * item.Quantity)
                                   : 0))
                .ReverseMap();
        }
    }
}
