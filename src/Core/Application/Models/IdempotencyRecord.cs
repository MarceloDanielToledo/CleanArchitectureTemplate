namespace Application.Models
{
    /// <summary>
    /// Stored outcome of a request executed under an Idempotency-Key.
    /// A record without <see cref="CompletedOn"/> means the request is still in progress.
    /// </summary>
    public class IdempotencyRecord
    {
        public string Key { get; set; }
        public string RequestHash { get; set; }
        public int? StatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime ExpiresOn { get; set; }

        public bool IsCompleted => CompletedOn.HasValue;
    }
}
