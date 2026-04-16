using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Mappings;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.Orders.Commands
{
    public class CreateOrderCommand(CreateOrderRequest request) : ICommand<Response<OrderResponse>>
    {
        public CreateOrderRequest Request { get; } = request;
    }
    internal sealed class CreateOrderCommandHandler(
        IRepositoryAsync<Order> repositoryAsync,
        IRepositoryAsync<Product> productRepositoryAsync) : ICommandHandler<CreateOrderCommand, Response<OrderResponse>>
    {
        private readonly IRepositoryAsync<Order> _repositoryAsync = repositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync = productRepositoryAsync;

        public async Task<Response<OrderResponse>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
        {
            var newRecord = command.Request.ToEntity();
            var newRecordCreated = await _repositoryAsync.AddAsync(newRecord, cancellationToken);
            return Response<OrderResponse>.Success(newRecordCreated.ToResponse(), ResponseMessages.AddedSuccesfullyMessage);
        }
    }
}
