namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public interface IChannelSettingsDtoBuilder
{
    Task<ChannelSettingsDto> BuildFromChannelIdAsync(ulong channelId);
}
