using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Orders.Commands
{
    public class DeleteOrderCommand(int id) : IRequest<Response<string>>
    {
        public int Id { get; } = id;
    }
    internal class DelteOrderCommandHandler(IRepositoryAsync<Order> repositoryAsync) : IRequestHandler<DeleteOrderCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Order> _repositoryAsync = repositoryAsync;

        public async Task<Response<string>> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.FirstOrDefaultAsync(new GetOrderByIdSpecification(command.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            await _repositoryAsync.DeleteAsync(record, cancellationToken);
            return Response<string>.Success(ResponseMessages.DeletedSuccessfullyMessage);
        }
    }
}
