using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Mappings;
using Application.UseCases.OrderItems.Responses;
using Application.UseCases.OrderItems.Specifications;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderItems.Queries
{
    public class GetOrderItemByIdQuery(int orderId, int id) : IRequest<Response<OrderItemResponse>>
    {
        public int OrderId { get; } = orderId;
        public int Id { get; } = id;
    }
    internal class GetOrderItemByIdQueryHandler(IRepositoryAsync<OrderItem> orderItemRepository) : IRequestHandler<GetOrderItemByIdQuery, Response<OrderItemResponse>>
    {
        private readonly IRepositoryAsync<OrderItem> _orderItemRepository = orderItemRepository;

        public async Task<Response<OrderItemResponse>> Handle(GetOrderItemByIdQuery request, CancellationToken cancellationToken)
        {
            var record = await _orderItemRepository.FirstOrDefaultAsync(new GetOrderItemByIdSpecification(request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            if (record.OrderId != request.OrderId)
            {
                throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            }
            return Response<OrderItemResponse>.Success(record.ToResponse());
        }
    }
}
