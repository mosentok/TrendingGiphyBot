namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

public interface IUtcOffsetFormatter
{
    string FormatUtcOffset(string utcOffset);
}
