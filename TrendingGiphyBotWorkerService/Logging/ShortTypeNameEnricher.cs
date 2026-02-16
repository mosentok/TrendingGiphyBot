using Serilog.Core;
using Serilog.Events;

namespace TrendingGiphyBotWorkerService.Logging;

public class ShortTypeNameEnricher : ILogEventEnricher
{
    public void Enrich(
        LogEvent logEvent,
        ILogEventPropertyFactory propertyFactory
    )
    {
        if (!logEvent.Properties.TryGetValue("SourceContext", out var sourceContextValue))
            return;

        if (sourceContextValue is not ScalarValue scalarValue || scalarValue.Value is not string sourceContext)
            return;

        var firstDotIndex = sourceContext.IndexOf('.');
        var lastDotIndex = sourceContext.LastIndexOf('.');
        var shortTypeName = firstDotIndex >= 0 && lastDotIndex >= 0
            ? $"{sourceContext[..firstDotIndex]}..{sourceContext[(lastDotIndex + 1)..]}"
            : sourceContext;

        var shortTypeNameProperty = propertyFactory.CreateProperty(
            "ShortTypeName",
            shortTypeName
        );

        logEvent.AddPropertyIfAbsent(shortTypeNameProperty);
    }
}
