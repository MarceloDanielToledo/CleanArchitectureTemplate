using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Mappings;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.Products.Commands
{
    public class CreateProductCommand(CreateProductRequest request) : ICommand<Response<ProductResponse>>
    {
        public CreateProductRequest Request { get; } = request;
    }
    internal sealed class CreateProductCommandHandler(IRepositoryAsync<Product> repositoryAsync) : ICommandHandler<CreateProductCommand, Response<ProductResponse>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync = repositoryAsync;

        public async Task<Response<ProductResponse>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            var newProduct = command.Request.ToEntity();
            var newProductCreated = await _repositoryAsync.AddAsync(newProduct, cancellationToken);
            return Response<ProductResponse>.Success(newProductCreated.ToResponse(), ResponseMessages.AddedSuccesfullyMessage);
        }
    }

}
