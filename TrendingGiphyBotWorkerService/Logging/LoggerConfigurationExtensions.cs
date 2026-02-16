using Serilog;
using Serilog.Configuration;

namespace TrendingGiphyBotWorkerService.Logging;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration WithShortTypeName(
        this LoggerEnrichmentConfiguration enrichmentConfiguration
    )
    {
        var enricher = new ShortTypeNameEnricher();

        return enrichmentConfiguration.With(enricher);
    }
}
