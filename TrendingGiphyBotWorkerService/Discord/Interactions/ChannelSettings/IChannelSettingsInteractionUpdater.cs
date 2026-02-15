using Discord.WebSocket;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

/// <summary>
/// Updates Discord interactions to reflect channel settings.
/// </summary>
public interface IChannelSettingsInteractionUpdater
{
    /// <summary>
    /// Refreshes the interaction display with current channel settings.
    /// </summary>
    /// <param name="channelSettings">The channel settings to reflect</param>
    /// <param name="channelName">The name of the channel being configured</param>
    /// <param name="interaction">The interaction to update</param>
    Task RefreshInteractionAsync(
        ChannelSettingsModel channelSettings,
        string channelName,
        SocketMessageComponent interaction);
}
