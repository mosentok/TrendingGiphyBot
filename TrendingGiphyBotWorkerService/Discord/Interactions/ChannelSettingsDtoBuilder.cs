using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

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
            .AsNoTracking()
            .Where(s => s.ChannelId == channelId)
            .Select(s => new ChannelSettingsDto(
                s.ChannelId,
                s.Frequency,
                s.GifPostingBehavior,
                s.GifPostingBehavior.GetDescription(),
                s.GifKeyword,
                s.GiphyRating,
                s.GifSource,
                s.Interval,
                s.PostingHours,
                s.RetentionDays
            ))
            .SingleAsync();
    }
}
