using Application.Constants;
using FluentValidation.Results;

namespace Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base(ResponseMessages.ValidationErrorMessage)
        {
            Errors = [];
        }
        public List<string> Errors { get; }
        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.ErrorMessage);
            }
        }
        public ValidationException(string error) : base()
        {
            Errors = [error];
        }
    }
}
