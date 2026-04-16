using Application.Abstractions.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Behaviours
{
    public sealed class ValidatingCommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IServiceProvider serviceProvider) : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var validators = serviceProvider.GetServices<IValidator<TCommand>>().ToList();
            if (validators.Count > 0)
            {
                var context = new ValidationContext<TCommand>(command);
                var results = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = results
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();
                if (failures.Count != 0)
                    throw new Exceptions.ValidationException(failures);
            }
            return await inner.HandleAsync(command, cancellationToken);
        }
    }
}
