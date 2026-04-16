using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Mappings;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.UseCases.Products.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.Products.Commands
{
    public class EditProductCommand(EditProductRequest request) : ICommand<Response<ProductResponse>>
    {
        public EditProductRequest Request { get; } = request;
    }

    internal sealed class EditProductCommandHandler(IRepositoryAsync<Product> repositoryAsync) : ICommandHandler<EditProductCommand, Response<ProductResponse>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync = repositoryAsync;

        public async Task<Response<ProductResponse>> HandleAsync(EditProductCommand command, CancellationToken cancellationToken = default)
        {
            var product = await _repositoryAsync.FirstOrDefaultAsync(new GetProductByIdSpecification(command.Request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            product.Name = command.Request.Name;
            product.Description = command.Request.Description;
            product.Price = command.Request.Price;
            product.StockQuantity = command.Request.StockQuantity;
            product.IsActive = command.Request.IsActive;
            await _repositoryAsync.UpdateAsync(product, cancellationToken);
            return Response<ProductResponse>.Success(product.ToResponse(), ResponseMessages.UpdatedSuccessfullyMessage);
        }
    }
}
