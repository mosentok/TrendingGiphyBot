using Serilog.Core;
using Serilog.Events;

namespace TrendingGiphyBotWorkerService.Logging;

public class UriRedactionEnricher(UriRedactor _uriRedactor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!logEvent.Properties.TryGetValue("Uri", out var uriValue))
            return;

        if (uriValue is not ScalarValue scalarValue)
            return;

        var redactedValue = RedactValue(scalarValue.Value);

        if (redactedValue is null)
            return;

        var redactedProperty = propertyFactory.CreateProperty("Uri", redactedValue);

        logEvent.AddOrUpdateProperty(redactedProperty);
    }

    public string? RedactValue(object? value)
    {
        if (value is Uri uri)
            return _uriRedactor.Redact(uri);

        if (value is string stringValue)
            return _uriRedactor.Redact(stringValue);

        return null;
    }
}
