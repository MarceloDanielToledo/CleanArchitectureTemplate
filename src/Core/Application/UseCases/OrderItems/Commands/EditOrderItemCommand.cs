using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Requests;
using Application.UseCases.OrderItems.Responses;
using Application.UseCases.OrderItems.Specifications;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderItems.Commands
{
    public class EditOrderItemCommand(int orderId, EditOrderItemRequest request) : IRequest<Response<OrderItemResponse>>
    {
        public int OrderId { get; } = orderId;
        public EditOrderItemRequest Request { get; } = request;
    }
    internal class EditOrderItemCommandHandler(
        IRepositoryAsync<Order> orderRepositoryAsync, 
        IRepositoryAsync<OrderItem> orderItemRepositoryAsync, 
        IMapper mapper) : IRequestHandler<EditOrderItemCommand, Response<OrderItemResponse>>
    {
        private readonly IRepositoryAsync<Order> _orderRepositoryAsync = orderRepositoryAsync;
        private readonly IRepositoryAsync<OrderItem> _orderItemRepositoryAsync = orderItemRepositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<OrderItemResponse>> Handle(EditOrderItemCommand command, CancellationToken cancellationToken)
        {
            var existOrder = await _orderRepositoryAsync.AnyAsync(new GetOrderByIdSpecification(command.OrderId),cancellationToken);
            if (!existOrder)
            {
                throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            }
            var record = await _orderItemRepositoryAsync.FirstOrDefaultAsync(new GetOrderItemByIdSpecification(command.Request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            record.Quantity = command.Request.Quantity;
            record.ProductId = command.Request.ProductId;
            record.UnitPrice = command.Request.UnitPrice;
            await _orderItemRepositoryAsync.UpdateAsync(record, cancellationToken);
            return Response<OrderItemResponse>.Success(_mapper.Map<OrderItemResponse>(record), ResponseMessages.UpdatedSuccessfullyMessage);
        }
    }
}
