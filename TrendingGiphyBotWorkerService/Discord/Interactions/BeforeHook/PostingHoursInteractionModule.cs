using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BeforeHook;

public class PostingHoursInteractionModule(ITrendingGiphyBotDbContext _trendingGiphyBotContext, IUtcOffsetFormatter _utcOffsetFormatter) : BeforeHookModelInteractionModuleBase(_trendingGiphyBotContext)
{
    [ComponentInteraction(InteractionId.TrendingPostingHoursButton)]
    public async Task OpenPostingHoursModalAsync()
    {
        var (fromString, toString, utcOffsetString) = DestructurePostingHoursStrings();

        var postingHooursModal = new ModalBuilder()
            .WithTitle("Set the hours when the bot should post")
            .WithCustomId(InteractionId.TrendingPostingHoursModal)
            .AddTextInput("From (24 hour time)", InteractionId.TrendingPostingHoursFromTextInput, placeholder: "10", maxLength: 2, required: false, value: fromString)
            .AddTextInput("To (24 hour time)", InteractionId.TrendingPostingHoursToTextInput, placeholder: "22", maxLength: 2, required: false, value: toString)
            .AddTextInput("Time Zone UTC Offset (+/-ab:xy)", InteractionId.TrendingPostingHoursUtcOffsetTextInput, placeholder: "-12:00, -03:30, +06:00", maxLength: 6, required: false, value: utcOffsetString)
            .Build();

        await Context.Interaction.RespondWithModalAsync(postingHooursModal);

        (string fromString, string toString, string utcOffsetString) DestructurePostingHoursStrings()
        {
            if (ChannelSettingsModel is not { PostingHours: { From: { } from, To: { } to, UtcOffset: { } utcOffset } })
                return (string.Empty, string.Empty, string.Empty);

            var utcOffsetString = _utcOffsetFormatter.FormatUtcOffset(utcOffset);

            return (from.ToString(), to.ToString(), utcOffsetString);
        }
    }
}
