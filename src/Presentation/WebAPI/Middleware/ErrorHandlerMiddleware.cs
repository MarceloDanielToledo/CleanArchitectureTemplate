using Application.Constants;
using Application.Wrappers;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middleware
{
    public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        private RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message ?? ResponseMessages.InternalServerErrorMessage };
                switch (error)
                {
                    case Application.Exceptions.ApplicationException e:
                        _logger.LogError(e, "ApplicationiException");
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message = ResponseMessages.InternalServerErrorMessage;
                        responseModel.Succeeded = false;
                        break;
                    case Application.Exceptions.ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message = ResponseMessages.ValidationErrorMessage;
                        responseModel.Errors = e.Errors;
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.Message = ResponseMessages.NotFoundMessage;
                        responseModel.Succeeded = false;
                        break;
                    default:
                        _logger.LogCritical(error, "UnhandledError");
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Message = ResponseMessages.InternalServerErrorMessage;
                        responseModel.Succeeded = false;
                        break;
                }

                JsonSerializerOptions options = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true,
                };
                var result = JsonSerializer.Serialize(responseModel, options);
                await response.WriteAsync(result);
            }
        }


    }
}
