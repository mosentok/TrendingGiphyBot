using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class HowOftenInteractionModule
(
    ITrendingGiphyBotDbContext trendingGiphyBotContext,
    IChannelSettingsDtoBuilder dtoBuilder,
    IChannelSettingsInteractionUpdater interactionUpdater
) : BothHooksInteractionModuleBase(trendingGiphyBotContext, dtoBuilder, interactionUpdater)
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
        var intervalId = int.Parse(intervalDescription);

        ChannelSettingsModel.Frequency = frequency;
        ChannelSettingsModel.IntervalId = intervalId;

        await TrendingGiphyBotContext.SaveChangesAsync();
    }
}
