using Application.Abstractions.Messaging;
using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using Domain.Entities;

namespace Application.UseCases.Orders.Commands
{
    public class DeleteOrderCommand(int id) : ICommand<Response<string>>
    {
        public int Id { get; } = id;
    }
    internal sealed class DelteOrderCommandHandler(IRepositoryAsync<Order> repositoryAsync) : ICommandHandler<DeleteOrderCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Order> _repositoryAsync = repositoryAsync;

        public async Task<Response<string>> HandleAsync(DeleteOrderCommand command, CancellationToken cancellationToken = default)
        {
            var record = await _repositoryAsync.FirstOrDefaultAsync(new GetOrderByIdSpecification(command.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            await _repositoryAsync.DeleteAsync(record, cancellationToken);
            return Response<string>.Success(ResponseMessages.DeletedSuccessfullyMessage);
        }
    }
}
