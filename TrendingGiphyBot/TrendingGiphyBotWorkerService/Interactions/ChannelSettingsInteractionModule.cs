using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

namespace TrendingGiphyBotWorkerService.Interactions;

public class ChannelSettingsInteractionModule(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IGifPostingBehaviorHelper _gifPostingBehaviorHelper) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
	ChannelSettingsModel? _channelSettings;
	bool? _shouldUpdateInteraction;

	public override async Task BeforeExecuteAsync(ICommandInfo command) =>
		_channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

    public override async Task AfterExecuteAsync(ICommandInfo command)
	{
		if (_shouldUpdateInteraction is null)
			throw new ThisShouldBeImpossibleException();

		if (!_shouldUpdateInteraction.Value)
		{
			await Context.Interaction.DeferAsync();

			return;
		}

        await _trendingGiphyBotContext.SaveChangesAsync();

        var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(_channelSettings!, Context.Channel.Name);

		await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = settingsMessageComponent);
	}

	[ComponentInteraction("how-often-select-menu")]
	public async Task SetHowOftenAsync(string[] selectedValues)
	{
		ThisShouldBeImpossibleException.ThrowIf(selectedValues.Length != 1);

		var howOftenPieces = selectedValues[0].Split('-');

		if (howOftenPieces.Length != 2)
			throw new ThisShouldBeImpossibleException();

		var frequencyString = howOftenPieces[0];
		var intervalDescription = howOftenPieces[1];

		var frequency = int.Parse(frequencyString);
		var intervalId = int.Parse(intervalDescription);

		_channelSettings!.Frequency = frequency;
		_channelSettings.IntervalId = intervalId;

		_shouldUpdateInteraction = true;
    }

	[ComponentInteraction("trending-gifs-only-button")]
	public async Task SetTrendingGifsOnlyAsync() =>
		_shouldUpdateInteraction = await _gifPostingBehaviorHelper.SetBehaviorAsync(_channelSettings!, GifPostingBehaviorKind.TrendingGifsOnly);

    [ComponentInteraction("trending-gifs-with-random-button")]
    public async Task SetTrendingGifsWithRandomAsync() =>
        _shouldUpdateInteraction = await _gifPostingBehaviorHelper.SetBehaviorAsync(_channelSettings!, GifPostingBehaviorKind.TrendingGifsWithRandomGifs);
}