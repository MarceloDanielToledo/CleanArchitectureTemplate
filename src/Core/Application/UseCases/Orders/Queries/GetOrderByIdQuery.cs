using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Mappings;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.Orders.Queries
{
    public class GetOrderByIdQuery(int id) : IQuery<Response<OrderResponse>>
    {
        public int Id { get; } = id;

    }
    internal sealed class GetOrderByIdQueryHandler(IRepositoryAsync<Order> repositoryAsync) : IQueryHandler<GetOrderByIdQuery, Response<OrderResponse>>
    {
        private readonly IRepositoryAsync<Order> _repositoryAsync = repositoryAsync;

        public async Task<Response<OrderResponse>> HandleAsync(GetOrderByIdQuery request, CancellationToken cancellationToken = default)
        {
            var record = await _repositoryAsync.FirstOrDefaultAsync(new GetOrderByIdSpecification(request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            return Response<OrderResponse>.Success(record.ToResponse());
        }
    }
}
