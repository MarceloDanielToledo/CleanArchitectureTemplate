using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Mappings;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Orders.Commands
{
    public class CreateOrderCommand(CreateOrderRequest request) : IRequest<Response<OrderResponse>>
    {
        public CreateOrderRequest Request { get; } = request;
    }
    internal class CreateOrderCommandHandler(
        IRepositoryAsync<Order> repositoryAsync,
        IRepositoryAsync<Product> productRepositoryAsync) : IRequestHandler<CreateOrderCommand, Response<OrderResponse>>
    {
        private readonly IRepositoryAsync<Order> _repositoryAsync = repositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync = productRepositoryAsync;

        public async Task<Response<OrderResponse>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var newRecord = command.Request.ToEntity();
            var newRecordCreated = await _repositoryAsync.AddAsync(newRecord, cancellationToken);
            return Response<OrderResponse>.Success(newRecordCreated.ToResponse(), ResponseMessages.AddedSuccesfullyMessage);
        }
    }
}
