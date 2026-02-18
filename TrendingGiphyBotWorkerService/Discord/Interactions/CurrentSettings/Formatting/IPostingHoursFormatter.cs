using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

public interface IPostingHoursFormatter
{
    string Format(PostingHours? postingHours);
}
