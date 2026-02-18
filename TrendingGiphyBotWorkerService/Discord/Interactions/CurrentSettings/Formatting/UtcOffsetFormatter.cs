namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

[RegisterSingleton]
public class UtcOffsetFormatter : IUtcOffsetFormatter
{
    public string FormatUtcOffset(string utcOffset) =>
        utcOffset.StartsWith('-')
            ? utcOffset[..6]
            : $"+{utcOffset[..5]}";
}