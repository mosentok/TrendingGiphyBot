using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class HowOftenInteractionModule
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : BothHooksToggleInteractionModuleBase(_trendingGiphyBotContext, _dtoBuilder, _settingsMessageComponentFactory)
{
    [ComponentInteraction(InteractionId.HowOftenButtonWildcard)]
    public async Task SelectHowOftenAsync(string howOftenValue)
    {
        var howOftenPieces = howOftenValue.Split('-');

        if (howOftenPieces.Length != 2)
            throw new ThisShouldBeImpossibleException();

        var frequencyString = howOftenPieces[0];
        var intervalDescription = howOftenPieces[1];

        var frequency = int.Parse(frequencyString);
        var interval = (Interval)int.Parse(intervalDescription);

        ChannelSettingsModel.Frequency = frequency;
        ChannelSettingsModel.Interval = interval;

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    protected override MessageComponent BuildToggleModal(ChannelSettingsDto channelSettings, string channelName) =>
        SettingsMessageComponentFactory.BuildHowOftenToggleModal(channelSettings, channelName);
}
