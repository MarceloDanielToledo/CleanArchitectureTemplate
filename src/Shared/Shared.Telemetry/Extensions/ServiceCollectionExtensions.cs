using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Telemetry.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTracingExtension(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            Environment.SetEnvironmentVariable("OTEL_DOTNET_AUTO_ENTITYFRAMEWORKCORE_SET_DBSTATEMENT_FOR_TEXT", "true");

            serviceCollection.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(configuration["AppName"]))
                        .AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(configuration["OpenTelemetryCollector"]);
                        });
                });
        }

        public static void AddMetricsExtension(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOpenTelemetry().WithMetrics(meterProviderBuilder =>
            {
                meterProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(configuration["AppName"]))
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(configuration["OpenTelemetryCollector"]);
                    });
            });
        }

        public static void AddLogginExtension(this IServiceCollection serviceCollection, IConfiguration configuration, WebApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(configuration["AppName"]));
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(configuration["OpenTelemetryCollector"]);
                });
                options.AddConsoleExporter();
            });

        }

    }
}
