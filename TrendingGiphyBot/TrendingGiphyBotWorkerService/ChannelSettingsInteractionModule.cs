using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService
{
    public class ChannelSettingsInteractionModule(IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory, ITrendingGiphyBotContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        ChannelSettings? _channelSettings;
        bool _theresAnythingToUpdate = true;

        public override async Task BeforeExecuteAsync(ICommandInfo command)
        {
            _channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);
        }

        public override async Task AfterExecuteAsync(ICommandInfo command)
        {
            if (!_theresAnythingToUpdate)
            {
                await Context.Interaction.DeferAsync();

                return;
            }

            var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(_channelSettings!);

            await Context.Interaction.UpdateAsync(async messageProperties => messageProperties.Components = settingsMessageComponent);
        }

        [ComponentInteraction("how-often-select-menu")]
        public async Task SetHowOftenAsync(string[] selectedValues)
        {
            if (selectedValues.Length != 1)
                throw new ThisShouldBeImpossibleException();

            _channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

            _channelSettings.HowOften = selectedValues[0];

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("trending-gifs-only-button")]
        public async Task SetTrendingGifsOnlyAsync()
        {
            if (_channelSettings!.GifPostingBehavior == "trending-gifs-only-button")
            {
                _theresAnythingToUpdate = false;

                return;
            }

			_channelSettings.GifPostingBehavior = "trending-gifs-only-button";

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("trending-gifs-with-random-button")]
        public async Task SetTrendingGifsWithRandomAsync()
		{
			if (_channelSettings!.GifPostingBehavior == "trending-gifs-with-random-button")
			{
				_theresAnythingToUpdate = false;

				return;
			}

			_channelSettings.GifPostingBehavior = "trending-gifs-with-random-button";

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("trending-gifs-with-keyword-button")]
        public async Task SetTrendingGifsWithKeywordAsync()
        {
            _channelSettings!.GifPostingBehavior = "trending-gifs-with-keyword-button";

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("posting-hours-from")]
        public async Task SetPostingHoursFromAsync(string postingHoursFrom)
        {
            //TODO validation of input

            _channelSettings!.PostingHoursFrom = postingHoursFrom;

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("posting-hours-to")]
        public async Task SetPostingHoursToAsync(string postingHoursTo)
        {
            //TODO validation of input

            _channelSettings!.PostingHoursTo = postingHoursTo;

            await _trendingGiphyBotContext.SaveChangesAsync();
        }

        [ComponentInteraction("time-zone")]
        public async Task SetTimeZoneAsync(string timeZone)
        {
            //TODO validation of input

            _channelSettings!.TimeZone = timeZone;

            await _trendingGiphyBotContext.SaveChangesAsync();
        }
    }
}