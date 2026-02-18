namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings;

public interface ICurrentChannelSettingsDisplayBuilder
{
    string BuildMainSettingsDisplay(ChannelSettingsDto channelSettings);
    string BuildOptionalSettingsDisplay(ChannelSettingsDto channelSettings);
}
