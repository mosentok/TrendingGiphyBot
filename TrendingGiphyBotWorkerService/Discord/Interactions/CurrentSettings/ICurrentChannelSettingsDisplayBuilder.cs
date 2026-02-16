namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings;

public interface ICurrentChannelSettingsDisplayBuilder
{
    string BuildDisplay(ChannelSettingsDto channelSettings);
}
