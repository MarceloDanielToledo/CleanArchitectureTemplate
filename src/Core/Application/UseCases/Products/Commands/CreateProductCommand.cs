using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Products.Commands
{
    public class CreateProductCommand(CreateProductRequest request) : IRequest<Response<ProductResponse>>
    {
        public CreateProductRequest Request { get; } = request;
    }
    internal class CreateProductCommandHandler(IRepositoryAsync<Product> repositoryAsync, IMapper mapper) : IRequestHandler<CreateProductCommand, Response<ProductResponse>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync = repositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var newProduct = _mapper.Map<Product>(command.Request);
            var newProductCreated = await _repositoryAsync.AddAsync(newProduct, cancellationToken);
            return Response<ProductResponse>.Success(_mapper.Map<ProductResponse>(newProductCreated), ResponseMessages.AddedSuccesfullyMessage);
        }
    }

}
