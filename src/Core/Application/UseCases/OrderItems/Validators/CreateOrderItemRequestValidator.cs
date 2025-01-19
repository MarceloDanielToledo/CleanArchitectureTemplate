using Application.UseCases.OrderItems.Requests;
using FluentValidation;

namespace Application.UseCases.OrderItems.Validators
{
    public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
    {
        public CreateOrderItemRequestValidator() 
        {
            RuleFor(e => e.Quantity)
                .GreaterThan(0);
            RuleFor(e => e.UnitPrice)
                .GreaterThan(0);
            RuleFor(e => e.ProductId)
                .GreaterThan(0);
        }

    }
}
