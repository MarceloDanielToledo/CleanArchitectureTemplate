using Application.Models;

namespace Application.Interfaces
{
    public interface IIdempotencyStore
    {
        /// <summary>
        /// Tries to reserve <paramref name="key"/> for a new execution.
        /// Returns <c>null</c> when the key was reserved by this call; otherwise returns the
        /// existing record (in progress or completed) owned by a previous request.
        /// </summary>
        Task<IdempotencyRecord?> TryBeginAsync(string key, string requestHash, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores the response of a reserved key so later retries can replay it.
        /// </summary>
        Task CompleteAsync(string key, int statusCode, string responseBody, CancellationToken cancellationToken = default);

        /// <summary>
        /// Releases a reserved key without storing a response, allowing the client to retry.
        /// </summary>
        Task ReleaseAsync(string key, CancellationToken cancellationToken = default);
    }
}
