using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Mappings;
using Application.UseCases.Products.Responses;
using Application.UseCases.Products.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.Products.Queries
{
    public class GetProductByIdQuery(int id) : IQuery<Response<ProductResponse>>
    {
        public int Id { get; } = id;
    }
    internal sealed class GetProductByIdQueryHandler(IRepositoryAsync<Product> repositoryAsync) : IQueryHandler<GetProductByIdQuery, Response<ProductResponse>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync = repositoryAsync;

        public async Task<Response<ProductResponse>> HandleAsync(GetProductByIdQuery request, CancellationToken cancellationToken = default)
        {
            var record = await _repositoryAsync.FirstOrDefaultAsync(new GetProductByIdSpecification(request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            return Response<ProductResponse>.Success(record.ToResponse());
        }
    }
}
