using Discord;
using Discord.WebSocket;
using Injectio.Attributes;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

[RegisterSingleton]
public class ChannelSettingsInteractionUpdater(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : IChannelSettingsInteractionUpdater
{
    public async Task RefreshInteractionAsync(
        ChannelSettingsModel channelSettings,
        string channelName,
        SocketMessageComponent interaction)
    {
        var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, channelName);

        await interaction.UpdateAsync(messageProperties =>
        {
            messageProperties.Components = settingsMessageComponent;
            messageProperties.Attachments = new[] { new FileAttachment("PoweredBy_200_Horizontal_Light-Backgrounds_With_Logo.gif"), new FileAttachment("Powered by KLIPY Horizontal - Yellow&White Logo.png") };
        });
    }
}
