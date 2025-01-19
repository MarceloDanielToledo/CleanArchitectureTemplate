using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Orders.Specifications;
using Application.UseCases.Products.Responses;
using Application.UseCases.Products.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Orders.Queries
{
    public class GetOrderByIdQuery(int id) : IRequest<Response<OrderResponse>>
    {
        public int Id { get; } = id;

    }
    internal class GetOrderByIdQueryHandler(IRepositoryAsync<Order> repositoryAsync, IMapper mapper) : IRequestHandler<GetOrderByIdQuery, Response<OrderResponse>>
    {
        private readonly IRepositoryAsync<Order> _repositoryAsync = repositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.FirstOrDefaultAsync(new GetOrderByIdSpecification(request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            return Response<OrderResponse>.Success(_mapper.Map<OrderResponse>(record));
        }
    }
}
