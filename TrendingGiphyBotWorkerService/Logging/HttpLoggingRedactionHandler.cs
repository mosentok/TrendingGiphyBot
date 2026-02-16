using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TrendingGiphyBotWorkerService.Logging;

/// <summary>
/// HTTP message handler that logs HTTP requests and responses with sensitive data redacted.
/// Intercepts at the logging framework level to redact API keys and tokens from logged URIs
/// without modifying the actual HTTP requests sent to the server.
/// </summary>
internal class HttpLoggingRedactionHandler(ILogger<HttpLoggingRedactionHandler> _logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var redactedUri = RedactUriForLogging(request.RequestUri?.OriginalString ?? "unknown");

        _logger.LogDebug("HTTP {Method} {Uri}", request.Method, redactedUri);

        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }

    private static string RedactUriForLogging(string uri)
    {
        if (string.IsNullOrEmpty(uri))
            return uri;

        var redacted = uri;

        redacted = RedactQueryParameter(redacted, "api_key");
        redacted = RedactQueryParameter(redacted, "customer_id");

        redacted = RedactPathSegment(redacted, 20);

        return redacted;
    }

    private static string RedactQueryParameter(string uri, string paramName)
    {
        var pattern = $"{paramName}=";
        var index = uri.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);

        if (index < 0)
            return uri;

        var startIndex = index + pattern.Length;
        var endIndex = uri.IndexOfAny(['&', '#'], startIndex);

        if (endIndex < 0)
            endIndex = uri.Length;

        var paramValue = uri[startIndex..endIndex];

        return uri.Replace($"{pattern}{paramValue}", $"{pattern}*", StringComparison.Ordinal);
    }

    private static string RedactPathSegment(string uri, int minLength)
    {
        var uri_obj = new Uri(uri, UriKind.RelativeOrAbsolute);

        if (!uri_obj.IsAbsoluteUri)
            return uri;

        var segments = uri_obj.Segments;
        var redactedSegments = new List<string>(segments);

        for (var i = 0; i < redactedSegments.Count; i++)
        {
            var segment = redactedSegments[i].TrimEnd('/');

            if (segment.Length > minLength && !segment.Contains(".") && !segment.Contains("?") && !segment.Contains("=") && segment != "/")
            {
                redactedSegments[i] = "*";
            }
        }

        var redactedPath = string.Concat(redactedSegments);
        var uriBuilder = new UriBuilder(uri_obj)
        {
            Path = redactedPath
        };

        return uriBuilder.Uri.OriginalString;
    }
}
