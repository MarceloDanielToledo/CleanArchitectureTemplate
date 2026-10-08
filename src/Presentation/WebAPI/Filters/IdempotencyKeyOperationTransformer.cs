using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WebAPI.Filters
{
    /// <summary>
    /// Documents the <c>Idempotency-Key</c> header on actions decorated with <see cref="IdempotentAttribute"/>.
    /// </summary>
    public sealed class IdempotencyKeyOperationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var attribute = context.Description.ActionDescriptor.EndpointMetadata.OfType<IdempotentAttribute>().FirstOrDefault();
            if (attribute is null)
                return Task.CompletedTask;

            operation.Parameters ??= [];
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = IdempotentAttribute.HeaderName,
                In = ParameterLocation.Header,
                Required = attribute.Required,
                Description = "Unique client-generated key (e.g. a UUID). Retrying with the same key replays the original response.",
                Schema = new OpenApiSchema { Type = JsonSchemaType.String, MaxLength = 100 },
            });
            return Task.CompletedTask;
        }
    }
}
