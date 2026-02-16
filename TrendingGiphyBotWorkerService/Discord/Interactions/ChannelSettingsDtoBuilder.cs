using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[RegisterSingleton]
public class ChannelSettingsDtoBuilder(
    IServiceScopeFactory _serviceScopeFactory
) : IChannelSettingsDtoBuilder
{
    public async Task<ChannelSettingsDto> BuildFromChannelIdAsync(ulong channelId)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        return await trendingGiphyBotDbContext.ChannelSettings
            .Where(s => s.ChannelId == channelId)
            .Select(s => new ChannelSettingsDto(
                s.ChannelId,
                s.Frequency,
                s.GifPostingBehaviorId,
                s.GifPostingBehavior.Description,
                s.GifKeyword,
                s.GiphyRating,
                s.GifSource,
                s.IntervalId,
                s.PostingHoursFrom,
                s.PostingHoursTo,
                s.RetentionDays,
                s.UtcOffset
            ))
            .SingleAsync();
    }
}
