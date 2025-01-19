using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Products.Queries;
using Application.Wrappers;
using AutoMapper;
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
        IRepositoryAsync<Product> productRepositoryAsync, 
        IMapper mapper) : IRequestHandler<CreateOrderCommand, Response<OrderResponse>>
    {
        private readonly IRepositoryAsync<Order> _repositoryAsync = repositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync = productRepositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<OrderResponse>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var newRecord = _mapper.Map<Order>(command.Request);
            var newRecordCreated = await _repositoryAsync.AddAsync(newRecord, cancellationToken);
            var mappedRecord = _mapper.Map<OrderResponse>(newRecordCreated);
            return Response<OrderResponse>.Success(mappedRecord, ResponseMessages.AddedSuccesfullyMessage);
        }
    }
}
