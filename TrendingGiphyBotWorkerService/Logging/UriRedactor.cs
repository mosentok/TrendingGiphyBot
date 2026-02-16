namespace TrendingGiphyBotWorkerService.Logging;

public class UriRedactor
{
    public string Redact(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        if (Uri.TryCreate(value, UriKind.Absolute, out var absoluteUri))
            return Redact(absoluteUri);

        if (Uri.TryCreate(value, UriKind.Relative, out var relativeUri))
            return RedactRelative(relativeUri.ToString());

        return value;
    }

    public string Redact(Uri uri)
    {
        var builder = new UriBuilder(uri);
        var redactedQuery = RedactQuery(builder.Query);
        var redactedPath = RedactPath(builder.Path);

        builder.Query = redactedQuery;
        builder.Path = redactedPath;

        return builder.Uri.OriginalString;
    }

    public string RedactQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return string.Empty;

        var trimmedQuery = query.TrimStart('?');
        var pairs = trimmedQuery.Split('&', StringSplitOptions.RemoveEmptyEntries);
        var redactedPairs = new List<string>(pairs.Length);

        foreach (var pair in pairs)
        {
            var pieces = pair.Split('=', 2, StringSplitOptions.None);
            var name = pieces.Length > 0 ? pieces[0] : string.Empty;
            var redactedPair = string.IsNullOrWhiteSpace(name) ? string.Empty : $"{name}=*";

            if (!string.IsNullOrWhiteSpace(redactedPair))
                redactedPairs.Add(redactedPair);
        }

        return string.Join('&', redactedPairs);
    }

    public string RedactPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return path;

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var redactedSegments = new List<string>(segments.Length);

        foreach (var segment in segments)
        {
            var redactedSegment = segment;

            if (segment.Length > 20)
            {
                if (!segment.Contains('.') && !segment.Contains('?') && !segment.Contains('='))
                    redactedSegment = "*";
            }

            redactedSegments.Add(redactedSegment);
        }

        var redactedPath = string.Join('/', redactedSegments);

        return "/" + redactedPath;
    }

    public string RedactRelative(string value)
    {
        var queryIndex = value.IndexOf('?', StringComparison.Ordinal);

        if (queryIndex < 0)
            return RedactPath(value);

        var pathPart = value[..queryIndex];
        var queryPart = value[queryIndex..];
        var redactedPath = RedactPath(pathPart);
        var redactedQuery = RedactQuery(queryPart);
        var redactedValue = redactedPath;

        if (!string.IsNullOrWhiteSpace(redactedQuery))
            redactedValue += "?" + redactedQuery;

        return redactedValue;
    }
}
