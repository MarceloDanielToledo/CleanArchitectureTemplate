using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace WebAPI.Extensions
{
    public static class LogExtensions
    {

        public static void AddCustomLogConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(loggingBulder =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Information)
                .WriteTo.MSSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
                var logger = loggerConfiguration.CreateLogger();
                loggingBulder.Services.AddSingleton<ILoggerFactory>(
                    provider => new SerilogLoggerFactory(logger, dispose: false));

            });

        }
    }
}
