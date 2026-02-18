namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings;

public interface ICurrentChannelSettingsDisplayBuilder
{
    string BuildUnifiedSettingsDisplay(ChannelSettingsDto channelSettings);
}
