using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Requests;
using Application.UseCases.OrderItems.Responses;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderItems.Commands
{
    public class CreateOrderItemCommand(int orderId, CreateOrderItemRequest request) : IRequest<Response<OrderItemResponse>>
    {
        public int OrderId { get; } = orderId;
        public CreateOrderItemRequest Request { get; } = request;
    }
    internal class CreateOrderItemCommandHandler(
        IRepositoryAsync<Order> orderRepositoryAsync,
        IRepositoryAsync<OrderItem> orderItemRepositoryAsync,
        IMapper mapper) : IRequestHandler<CreateOrderItemCommand, Response<OrderItemResponse>>
    {
        private readonly IRepositoryAsync<Order> _orderRepositoryAsync = orderRepositoryAsync;
        private readonly IRepositoryAsync<OrderItem> _orderItemRepositoryAsync = orderItemRepositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<OrderItemResponse>> Handle(CreateOrderItemCommand command, CancellationToken cancellationToken)
        {
            var record = await _orderRepositoryAsync.FirstOrDefaultAsync(new GetOrderByIdSpecification(command.OrderId), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            var newOrderItem = _mapper.Map<OrderItem>(command.Request);
            newOrderItem.OrderId = record.Id;
            var newOrderItemCreated = await _orderItemRepositoryAsync.AddAsync(newOrderItem, cancellationToken);
            return Response<OrderItemResponse>.Success(_mapper.Map<OrderItemResponse>(newOrderItemCreated), ResponseMessages.AddedSuccesfullyMessage);
        }
    }
}
