namespace Application.Constants
{
    public static class ResponseMessages
    {
        public static readonly string SuccessMessage = "Successful response";
        public static readonly string AddedSuccesfullyMessage = "Record added successfully.";
        public static readonly string UpdatedSuccessfullyMessage = "Record updated successfully.";
        public static readonly string DeletedSuccessfullyMessage = "Record deleted successfully.";
        public static readonly string NotFoundsMessage = "Records not found";
        public static readonly string NotFoundMessage = "Record not found";
        public static readonly string ValidationErrorMessage = "The validation process has encountered one or more errors.";
        public static readonly string InternalServerErrorMessage = "Internal server error has occurred.";
        public static readonly string IdempotencyKeyRequiredMessage = "The Idempotency-Key header is required for this operation.";
        public static readonly string IdempotencyKeyInvalidMessage = "The Idempotency-Key header must be between 1 and 100 characters.";
        public static readonly string IdempotencyKeyInProgressMessage = "A request with the same Idempotency-Key is still being processed.";
        public static readonly string IdempotencyKeyReusedMessage = "The Idempotency-Key has already been used with a different request.";

    }
}
