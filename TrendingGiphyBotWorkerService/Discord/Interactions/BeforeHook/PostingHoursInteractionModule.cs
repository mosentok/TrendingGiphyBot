using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BeforeHook;

public class PostingHoursInteractionModule(ITrendingGiphyBotDbContext _trendingGiphyBotContext) : BeforeHookModelInteractionModuleBase(_trendingGiphyBotContext)
{
    [ComponentInteraction(InteractionId.TrendingPostingHoursButton)]
    public async Task OpenPostingHoursModalAsync()
    {
        var channelSettings = ChannelSettingsModel;

        var postingHooursModal = new ModalBuilder()
            .WithTitle("Set the hours when the bot should post")
            .WithCustomId(InteractionId.TrendingPostingHoursModal)
            .AddTextInput("From (24 hour time)", InteractionId.TrendingPostingHoursFromTextInput, placeholder: "10", maxLength: 2, required: false, value: channelSettings.PostingHoursFrom?.ToString() ?? string.Empty)
            .AddTextInput("To (24 hour time)", InteractionId.TrendingPostingHoursToTextInput, placeholder: "22", maxLength: 2, required: false, value: channelSettings.PostingHoursTo?.ToString() ?? string.Empty)
            .AddTextInput("Time Zone UTC Offset (+/-ab:xy)", InteractionId.TrendingPostingHoursUtcOffsetTextInput, placeholder: "-12:00, -3:30, +6:00", maxLength: 6, required: false, value: channelSettings.UtcOffset ?? string.Empty)
            .Build();

        await Context.Interaction.RespondWithModalAsync(postingHooursModal);
    }
}
