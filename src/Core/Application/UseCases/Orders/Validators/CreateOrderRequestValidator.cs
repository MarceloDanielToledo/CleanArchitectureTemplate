using Application.UseCases.OrderItems.Validators;
using Application.UseCases.Orders.Commands;
using FluentValidation;

namespace Application.UseCases.Orders.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderRequestValidator() 
        {
            RuleFor(e => e.Request.Comment)
                .Length(1, 300);
            RuleForEach(x => x.Request.OrderItems)
                .SetValidator(new CreateOrderItemRequestValidator());
        }

    }
}
