using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Mappings;
using Application.UseCases.OrderItems.Requests;
using Application.UseCases.OrderItems.Responses;
using Application.UseCases.OrderItems.Specifications;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.OrderItems.Commands
{
    public class EditOrderItemCommand(int orderId, EditOrderItemRequest request) : ICommand<Response<OrderItemResponse>>
    {
        public int OrderId { get; } = orderId;
        public EditOrderItemRequest Request { get; } = request;
    }
    internal sealed class EditOrderItemCommandHandler(
        IRepositoryAsync<Order> orderRepositoryAsync,
        IRepositoryAsync<OrderItem> orderItemRepositoryAsync) : ICommandHandler<EditOrderItemCommand, Response<OrderItemResponse>>
    {
        private readonly IRepositoryAsync<Order> _orderRepositoryAsync = orderRepositoryAsync;
        private readonly IRepositoryAsync<OrderItem> _orderItemRepositoryAsync = orderItemRepositoryAsync;

        public async Task<Response<OrderItemResponse>> HandleAsync(EditOrderItemCommand command, CancellationToken cancellationToken = default)
        {
            var existOrder = await _orderRepositoryAsync.AnyAsync(new GetOrderByIdSpecification(command.OrderId), cancellationToken);
            if (!existOrder)
            {
                throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            }
            var record = await _orderItemRepositoryAsync.FirstOrDefaultAsync(new GetOrderItemByIdSpecification(command.Request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            record.Quantity = command.Request.Quantity;
            record.ProductId = command.Request.ProductId;
            record.UnitPrice = command.Request.UnitPrice;
            await _orderItemRepositoryAsync.UpdateAsync(record, cancellationToken);
            return Response<OrderItemResponse>.Success(record.ToResponse(), ResponseMessages.UpdatedSuccessfullyMessage);
        }
    }
}
