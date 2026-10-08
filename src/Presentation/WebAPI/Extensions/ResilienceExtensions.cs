namespace WebAPI.Extensions
{
    public static class ResilienceExtensions
    {
        public const string RequestTimeoutSection = "Resilience:RequestTimeout";

        public static void AddResilienceExtension(this IServiceCollection services, IConfiguration configuration)
        {
            // Every HttpClient created through IHttpClientFactory gets retries, circuit breaker and timeouts.
            services.ConfigureHttpClientDefaults(http => http.AddStandardResilienceHandler());

            // Cancels the request (and the CancellationToken flowing to handlers/EF) when it runs too long.
            var timeout = configuration.GetValue(RequestTimeoutSection, TimeSpan.FromSeconds(30));
            services.AddRequestTimeouts(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
                {
                    Timeout = timeout,
                    TimeoutStatusCode = StatusCodes.Status504GatewayTimeout,
                };
            });
        }
    }
}
