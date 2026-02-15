using Discord;
using Discord.WebSocket;
using Injectio.Attributes;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

[RegisterSingleton]
public class ChannelSettingsInteractionUpdater(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    IOptionsMonitor<AppConfig> _appConfig
) : IChannelSettingsInteractionUpdater
{
    public async Task RefreshInteractionAsync(
        ChannelSettingsModel channelSettings,
        string channelName,
        SocketMessageComponent interaction)
    {
        var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, channelName);

        var attachments = _appConfig.CurrentValue.Attribution.AttachmentFileNames.Select(fileName => new FileAttachment(fileName)).ToArray();

        await interaction.UpdateAsync(messageProperties =>
        {
            messageProperties.Components = settingsMessageComponent;
            messageProperties.Attachments = attachments;
        });
    }
}
