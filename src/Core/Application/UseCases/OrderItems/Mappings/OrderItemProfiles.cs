using Application.UseCases.OrderItems.Requests;
using Application.UseCases.OrderItems.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.OrderItems.Mappings
{
    public class OrderItemProfiles : Profile
    {
        public OrderItemProfiles() 
        {
            CreateMap<OrderItem, CreateOrderItemRequest>().ReverseMap();
            CreateMap<OrderItemResponse, OrderItem>().ReverseMap();
        }

    }
}
