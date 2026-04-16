using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Mappings;
using Application.UseCases.OrderItems.Responses;
using Application.UseCases.OrderItems.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.OrderItems.Queries
{
    public class GetOrderItemByIdQuery(int orderId, int id) : IQuery<Response<OrderItemResponse>>
    {
        public int OrderId { get; } = orderId;
        public int Id { get; } = id;
    }
    internal sealed class GetOrderItemByIdQueryHandler(IRepositoryAsync<OrderItem> orderItemRepository) : IQueryHandler<GetOrderItemByIdQuery, Response<OrderItemResponse>>
    {
        private readonly IRepositoryAsync<OrderItem> _orderItemRepository = orderItemRepository;

        public async Task<Response<OrderItemResponse>> HandleAsync(GetOrderItemByIdQuery request, CancellationToken cancellationToken = default)
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
