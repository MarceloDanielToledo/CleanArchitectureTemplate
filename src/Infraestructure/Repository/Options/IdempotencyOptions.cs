namespace Repository.Options
{
    public class IdempotencyOptions
    {
        public const string SectionName = "Idempotency";

        /// <summary>
        /// How long a completed response is kept for replay.
        /// </summary>
        public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// After this time an in-progress reservation is considered abandoned
        /// (e.g. the process crashed) and the key can be reserved again.
        /// </summary>
        public TimeSpan InProgressTimeout { get; set; } = TimeSpan.FromMinutes(2);
    }
}
