using Application.Constants;

namespace Application.Wrappers
{
    public class Response<T> where T : class
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = [];
        public T? Data { get; set; }
        public Response() { }
        public Response(bool succeeded, string message, List<string> errors, T data)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors;
            Data = data;
        }
        public Response(bool succeeded, string message, List<string> errors)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors;
        }

        public static Response<T> NotSuccess(string message)
            => new(succeeded: false, message: message, errors: Enumerable.Empty<string>().ToList());
        public static Response<T> NotSuccess(string message, string[] errors)
            => new(succeeded: false, message: message, errors: [..errors]);
        public static Response<T> Success(T data, string message)
            => new(succeeded: true, message: message, errors: Enumerable.Empty<string>().ToList(), data: data);
        public static Response<T> Success(T data)
            => new(succeeded: true, message: ResponseMessages.SuccessMessage, errors: Enumerable.Empty<string>().ToList(), data: data);
    }
}
