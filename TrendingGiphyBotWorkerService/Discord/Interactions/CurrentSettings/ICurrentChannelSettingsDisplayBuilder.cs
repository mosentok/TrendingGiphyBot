namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings;

public interface ICurrentChannelSettingsDisplayBuilder
{
    string BuildDisplay(ChannelSettingsDto channelSettings);
    string BuildMainSettingsDisplay(ChannelSettingsDto channelSettings);
    string BuildOptionalSettingsDisplay(ChannelSettingsDto channelSettings);
}
