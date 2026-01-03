using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService
{
    [Group("tgb", "Trending Giphy Bot commands for your server")]
    public class TgbSlashInteractionModule(IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory, ITrendingGiphyBotContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("settings", "View and change your Trending Giphy Bot's settings for your server")]
        public async Task GetSettingsAsync()
        {
            //var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

            var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(new ChannelSettings());

            await RespondAsync(components: settingsMessageComponent);
        }
    }

	public class TgbComponentInteractionModule(IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory, ITrendingGiphyBotContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
		[ComponentInteraction("how-often-select-menu")]
        public async Task SetHowOftenAsync(string[] selectedValues)
        {
            if (selectedValues.Length != 1)
                throw new ThisShouldBeImpossibleException();

            var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

            channelSettings.HowOften = selectedValues[0];

            await _trendingGiphyBotContext.SaveChangesAsync();

            var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings);

            await Context.Interaction.UpdateAsync(async messageProperties => messageProperties.Components = settingsMessageComponent);
        }

        [ComponentInteraction("trending-gifs-only-button")]
        public async Task SetTrendingGifsOnlyAsync()
        {
            var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

            channelSettings.GifPostingBehavior = "trending-gifs-only-button";

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("trending-gifs-with-random-button")]
        public async Task SetTrendingGifsWithRandomAsync()
        {
			var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

			channelSettings.GifPostingBehavior = "trending-gifs-with-random-button";

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("trending-gifs-with-keyword-button")]
        public async Task SetTrendingGifsWithKeywordAsync()
        {
			var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

			channelSettings.GifPostingBehavior = "trending-gifs-with-keyword-button";

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("trending-gifs-with-keyword-text-input")]
        public async Task SetKeywordAsync(string keyword)
        {
			//TODO validation of input

			var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

			channelSettings.GifKeyword = keyword;

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("posting-hours-from")]
        public async Task SetPostingHoursFromAsync(string postingHoursFrom)
        {
			//TODO validation of input

			var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

			channelSettings.PostingHoursFrom = postingHoursFrom;

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("posting-hours-to")]
        public async Task SetPostingHoursToAsync(string postingHoursTo)
        {
			//TODO validation of input

			var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

			channelSettings.PostingHoursTo = postingHoursTo;

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("time-zone")]
        public async Task SetTimeZoneAsync(string timeZone)
        {
			//TODO validation of input

			var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

			channelSettings.TimeZone = timeZone;

            await _trendingGiphyBotContext.SaveChangesAsync();
        }
    }
}