using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Specifications;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.OrderItems.Commands
{
    public class DeleteOrderItemCommand(int orderId,int idOrderItem) : ICommand<Response<string>>
    {
        public int OrderId { get; } = orderId;
        public int IdOrderItem { get; } = idOrderItem;
    }
    internal sealed class DeleteOrderItemCommandHandler(IRepositoryAsync<Order> orderRepositoryAsync,IRepositoryAsync<OrderItem> orderItemRepositoryAsync) : ICommandHandler<DeleteOrderItemCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Order> _orderRepositoryAsync = orderRepositoryAsync;
        private readonly IRepositoryAsync<OrderItem> _orderItemRepositoryAsync = orderItemRepositoryAsync;

        public async Task<Response<string>> HandleAsync(DeleteOrderItemCommand command, CancellationToken cancellationToken = default)
        {
            var existOrder = await _orderRepositoryAsync.AnyAsync(new GetOrderByIdSpecification(command.OrderId), cancellationToken);
            if (!existOrder)
            {
                throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            }
            var record = await _orderItemRepositoryAsync.FirstOrDefaultAsync(new GetOrderItemByIdSpecification(command.IdOrderItem), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            await _orderItemRepositoryAsync.DeleteAsync(record, cancellationToken);
            return Response<string>.Success(ResponseMessages.DeletedSuccessfullyMessage);
        }
    }
}
