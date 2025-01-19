using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.UseCases.Products.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Products.Commands
{
    public class EditProductCommand(EditProductRequest request) : IRequest<Response<ProductResponse>>
    {
        public EditProductRequest Request { get; } = request;
    }

    internal class EditProductCommandHandler(IRepositoryAsync<Product> repositoryAsync, IMapper mapper) : IRequestHandler<EditProductCommand, Response<ProductResponse>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync = repositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<ProductResponse>> Handle(EditProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.FirstOrDefaultAsync(new GetProductByIdSpecification(command.Request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            product.Name = command.Request.Name;
            product.Description = command.Request.Description;
            product.Price = command.Request.Price;
            product.StockQuantity = command.Request.StockQuantity;
            product.IsActive = command.Request.IsActive;
            await _repositoryAsync.UpdateAsync(product, cancellationToken);
            return Response<ProductResponse>.Success(_mapper.Map<ProductResponse>(product), ResponseMessages.UpdatedSuccessfullyMessage);
        }
    }
}
