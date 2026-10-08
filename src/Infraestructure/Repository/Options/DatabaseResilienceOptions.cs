namespace Repository.Options
{
    public class DatabaseResilienceOptions
    {
        public const string SectionName = "Resilience:Database";

        public int MaxRetryCount { get; set; } = 5;
        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(10);
        public int CommandTimeoutSeconds { get; set; } = 30;
    }
}
