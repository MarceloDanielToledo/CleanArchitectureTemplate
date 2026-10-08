using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Filters
{
    /// <summary>
    /// Makes an action idempotent through the <c>Idempotency-Key</c> request header.
    /// Retries with the same key and payload replay the first successful response instead of executing again.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IdempotentAttribute : TypeFilterAttribute
    {
        public const string HeaderName = "Idempotency-Key";
        public const string ReplayedHeaderName = "Idempotent-Replayed";

        public IdempotentAttribute(bool required = false) : base(typeof(IdempotencyFilter))
        {
            Required = required;
            Arguments = [required];
        }

        /// <summary>
        /// When true, requests without an <c>Idempotency-Key</c> header are rejected with 400.
        /// </summary>
        public bool Required { get; }
    }
}
