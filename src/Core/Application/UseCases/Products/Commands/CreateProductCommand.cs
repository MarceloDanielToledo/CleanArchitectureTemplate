using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Mappings;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Products.Commands
{
    public class CreateProductCommand(CreateProductRequest request) : IRequest<Response<ProductResponse>>
    {
        public CreateProductRequest Request { get; } = request;
    }
    internal class CreateProductCommandHandler(IRepositoryAsync<Product> repositoryAsync) : IRequestHandler<CreateProductCommand, Response<ProductResponse>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync = repositoryAsync;

        public async Task<Response<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var newProduct = command.Request.ToEntity();
            var newProductCreated = await _repositoryAsync.AddAsync(newProduct, cancellationToken);
            return Response<ProductResponse>.Success(newProductCreated.ToResponse(), ResponseMessages.AddedSuccesfullyMessage);
        }
    }

}
