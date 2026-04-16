using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Mappings;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.Orders.Commands
{
    public class EditOrderCommand(EditOrderRequest request) : ICommand<Response<OrderResponse>>
    {
        public EditOrderRequest Request { get; } = request;
    }
    internal sealed class EditOrderCommandHandler(IRepositoryAsync<Order> orderRespositoryAsync) : ICommandHandler<EditOrderCommand, Response<OrderResponse>>
    {
        private readonly IRepositoryAsync<Order> _orderRespositoryAsync = orderRespositoryAsync;

        public async Task<Response<OrderResponse>> HandleAsync(EditOrderCommand command, CancellationToken cancellationToken = default)
        {
            var record = await _orderRespositoryAsync.FirstOrDefaultAsync(new GetOrderByIdSpecification(command.Request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            record.Comment = command.Request.Comment;
            await _orderRespositoryAsync.UpdateAsync(record, cancellationToken);
            return Response<OrderResponse>.Success(record.ToResponse(), ResponseMessages.UpdatedSuccessfullyMessage);
        }
    }
}
