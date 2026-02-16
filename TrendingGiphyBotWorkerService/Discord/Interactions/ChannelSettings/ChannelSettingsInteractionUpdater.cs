using Discord.WebSocket;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

[RegisterSingleton]
public class ChannelSettingsInteractionUpdater(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : IChannelSettingsInteractionUpdater
{
    public async Task RefreshInteractionAsync(
        ChannelSettingsDto channelSettings,
        string channelName,
        SocketMessageComponent interaction)
    {
        var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, channelName);

        await interaction.UpdateAsync(messageProperties => messageProperties.Components = settingsMessageComponent);
    }
}
