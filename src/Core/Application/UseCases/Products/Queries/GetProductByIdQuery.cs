using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Responses;
using Application.UseCases.Products.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Products.Queries
{
    public class GetProductByIdQuery(int id) : IRequest<Response<ProductResponse>>
    {
        public int Id { get; } = id;
    }
    internal class GetProductByIdQueryHandler(IRepositoryAsync<Product> repositoryAsync, IMapper mapper) : IRequestHandler<GetProductByIdQuery, Response<ProductResponse>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync = repositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.FirstOrDefaultAsync(new GetProductByIdSpecification(request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            return Response<ProductResponse>.Success(_mapper.Map<ProductResponse>(record));
        }
    }
}
