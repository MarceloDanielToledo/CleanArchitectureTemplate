using Application.UseCases.Products.Commands;
using FluentValidation;

namespace Application.UseCases.Products.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(e => e.Request.Name)
                .NotEmpty()
                .Length(1, 200);
            RuleFor(e => e.Request.Description)
                .NotEmpty()
                .Length(1, 300);
            RuleFor(e => e.Request.Price)
                .GreaterThan(0);
            RuleFor(e => e.Request.StockQuantity)
                .GreaterThan(0);
        }

    }
}
