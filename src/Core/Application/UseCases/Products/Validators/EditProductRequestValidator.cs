using Application.UseCases.Products.Commands;
using FluentValidation;

namespace Application.UseCases.Products.Validators
{
    public class EditProductRequestValidator : AbstractValidator<EditProductCommand>
    {
        public EditProductRequestValidator() 
        {
            RuleFor(e => e.Request.Id)
                .GreaterThan(0);
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
