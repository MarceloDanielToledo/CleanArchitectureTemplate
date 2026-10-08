using Application.Constants;
using Application.Interfaces;
using Application.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WebAPI.Filters
{
    public sealed class IdempotencyFilter(
        bool required,
        IIdempotencyStore store,
        IOptions<JsonOptions> jsonOptions,
        ILogger<IdempotencyFilter> logger) : IAsyncActionFilter
    {
        private const int MaxKeyLength = 100;

        private readonly bool _required = required;
        private readonly IIdempotencyStore _store = store;
        private readonly JsonSerializerOptions _serializerOptions = jsonOptions.Value.JsonSerializerOptions;
        private readonly ILogger<IdempotencyFilter> _logger = logger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var key = httpContext.Request.Headers[IdempotentAttribute.HeaderName].ToString();

            if (string.IsNullOrWhiteSpace(key))
            {
                if (_required)
                {
                    context.Result = Error(StatusCodes.Status400BadRequest, ResponseMessages.IdempotencyKeyRequiredMessage);
                    return;
                }
                await next();
                return;
            }
            if (key.Length > MaxKeyLength)
            {
                context.Result = Error(StatusCodes.Status400BadRequest, ResponseMessages.IdempotencyKeyInvalidMessage);
                return;
            }

            var requestHash = ComputeRequestHash(context);
            var existing = await _store.TryBeginAsync(key, requestHash, httpContext.RequestAborted);
            if (existing is not null)
            {
                context.Result = existing switch
                {
                    _ when existing.RequestHash != requestHash
                        => Error(StatusCodes.Status422UnprocessableEntity, ResponseMessages.IdempotencyKeyReusedMessage),
                    { IsCompleted: false }
                        => Error(StatusCodes.Status409Conflict, ResponseMessages.IdempotencyKeyInProgressMessage),
                    _ => Replay(httpContext, existing.StatusCode!.Value, existing.ResponseBody!),
                };
                return;
            }

            ActionExecutedContext executed;
            try
            {
                executed = await next();
            }
            catch
            {
                await ReleaseAsync(key);
                throw;
            }

            if (executed.Exception is null
                && executed.Result is ObjectResult result
                && result.StatusCode is null or (>= 200 and < 300))
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status200OK;
                var body = JsonSerializer.Serialize(result.Value, _serializerOptions);
                await _store.CompleteAsync(key, statusCode, body, CancellationToken.None);
            }
            else
            {
                // Failed executions are not cached so the client can safely retry with the same key.
                await ReleaseAsync(key);
            }
        }

        private async Task ReleaseAsync(string key)
        {
            try
            {
                await _store.ReleaseAsync(key, CancellationToken.None);
            }
            catch (Exception ex)
            {
                // The reservation expires on its own (InProgressTimeout); never hide the original error.
                _logger.LogWarning(ex, "Could not release idempotency key {IdempotencyKey}", key);
            }
        }

        private string ComputeRequestHash(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var payload = JsonSerializer.Serialize(new
            {
                request.Method,
                Path = request.Path.Value,
                Arguments = context.ActionArguments
                    .Where(a => a.Value is not CancellationToken)
                    .OrderBy(a => a.Key, StringComparer.Ordinal),
            }, _serializerOptions);
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));
        }

        private static ContentResult Replay(HttpContext httpContext, int statusCode, string body)
        {
            httpContext.Response.Headers[IdempotentAttribute.ReplayedHeaderName] = "true";
            return new ContentResult
            {
                StatusCode = statusCode,
                Content = body,
                ContentType = "application/json",
            };
        }

        private static ObjectResult Error(int statusCode, string message)
            => new(Response<string>.NotSuccess(message)) { StatusCode = statusCode };
    }
}
