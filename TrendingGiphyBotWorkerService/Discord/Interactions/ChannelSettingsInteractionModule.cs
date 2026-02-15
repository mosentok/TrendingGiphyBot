using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public class ChannelSettingsInteractionModule
(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IGifPostingBehaviorHelper _gifPostingBehaviorHelper,
    IChannelSettingsInteractionUpdater _interactionUpdater
) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private enum PendingClearAction
    {
        None,
        HowOften,
        PostingBehavior,
        GifSources,
        GifRetention,
        GiphyRating,
        Keyword,
        PostingHours
    }

    private static readonly Dictionary<ulong, (PendingClearAction Action, string Message)> PendingConfirmations = new();

    ChannelSettingsModel? _channelSettings;

    public override async Task BeforeExecuteAsync(ICommandInfo command) =>
        _channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

    [ComponentInteraction(InteractionId.BackButton)]
    public async Task BackAsync() =>
        await _interactionUpdater.RefreshInteractionAsync(_channelSettings!, Context.Channel.Name, Context.Interaction);

    [ComponentInteraction(InteractionId.HowOftenOpenButton)]
    public async Task OpenHowOftenAsync()
    {
        var component = _settingsMessageComponentFactory.BuildHowOftenToggleModal(_channelSettings!);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

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

        _channelSettings!.Frequency = frequency;
        _channelSettings.IntervalId = intervalId;

        await _trendingGiphyBotContext.SaveChangesAsync();

        var component = _settingsMessageComponentFactory.BuildHowOftenToggleModal(_channelSettings, howOftenValue);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.ResetHowOftenButton)]
    public async Task ResetHowOftenAsync()
    {
        PendingConfirmations[Context.Channel.Id] = (PendingClearAction.HowOften, "Are you sure you want to reset how often the bot posts gifs to 30 minutes?");

        var component = _settingsMessageComponentFactory.BuildConfirmationModal(PendingConfirmations[Context.Channel.Id].Message);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.PostingBehaviorOpenButton)]
    public async Task OpenPostingBehaviorAsync()
    {
        var component = _settingsMessageComponentFactory.BuildPostingBehaviorToggleModal(_channelSettings!);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.PostingBehaviorTrendingOnlyButton)]
    public async Task SelectPostingBehaviorTrendingOnlyAsync()
    {
        await _gifPostingBehaviorHelper.SetBehaviorAsync(_channelSettings!, GifPostingBehaviorKind.TrendingGifsOnly);

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings!, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.PostingBehaviorTrendingWithRandomButton)]
    public async Task SelectPostingBehaviorTrendingWithRandomAsync()
    {
        await _gifPostingBehaviorHelper.SetBehaviorAsync(_channelSettings!, GifPostingBehaviorKind.TrendingGifsWithRandomGifs);

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings!, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.ResetPostingBehaviorButton)]
    public async Task ResetPostingBehaviorAsync()
    {
        PendingConfirmations[Context.Channel.Id] = (PendingClearAction.PostingBehavior, "Are you sure you want to reset the posting behavior to Trending Gifs Only?");

        var component = _settingsMessageComponentFactory.BuildConfirmationModal(PendingConfirmations[Context.Channel.Id].Message);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.GifSourcesOpenButton)]
    public async Task OpenGifSourcesAsync()
    {
        var component = _settingsMessageComponentFactory.BuildGifSourcesToggleModal(_channelSettings!);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.GifSourceGiphyButton)]
    public async Task ToggleGifSourceGiphyAsync()
    {
        var sources = _channelSettings!.GifSource ?? (GifSourceKind.Giphy | GifSourceKind.Klipy);

        if (sources.HasFlag(GifSourceKind.Giphy))
            sources &= ~GifSourceKind.Giphy;
        else
            sources |= GifSourceKind.Giphy;

        _channelSettings.GifSource = sources;

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.GifSourceKlipyButton)]
    public async Task ToggleGifSourceKlipyAsync()
    {
        var sources = _channelSettings!.GifSource ?? (GifSourceKind.Giphy | GifSourceKind.Klipy);

        if (sources.HasFlag(GifSourceKind.Klipy))
            sources &= ~GifSourceKind.Klipy;
        else
            sources |= GifSourceKind.Klipy;

        _channelSettings.GifSource = sources;

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.ClearGifSourcesButton)]
    public async Task ClearGifSourcesAsync()
    {
        PendingConfirmations[Context.Channel.Id] = (PendingClearAction.GifSources, "Are you sure you want to clear gif sources?");

        var component = _settingsMessageComponentFactory.BuildConfirmationModal(PendingConfirmations[Context.Channel.Id].Message);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.GifRetentionOpenButton)]
    public async Task OpenGifRetentionAsync()
    {
        var component = _settingsMessageComponentFactory.BuildGifRetentionToggleModal(_channelSettings!);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.GifRetentionButtonWildcard)]
    public async Task SelectGifRetentionAsync(string gifRetention)
    {
        _channelSettings!.RetentionDays = int.Parse(gifRetention);

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.ResetRetentionPeriodButton)]
    public async Task ResetGifRetentionAsync()
    {
        PendingConfirmations[Context.Channel.Id] = (PendingClearAction.GifRetention, "Are you sure you want to reset the retention period to 14 days?");

        var component = _settingsMessageComponentFactory.BuildConfirmationModal(PendingConfirmations[Context.Channel.Id].Message);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.GiphyRatingOpenButton)]
    public async Task OpenGiphyRatingAsync()
    {
        var component = _settingsMessageComponentFactory.BuildGiphyRatingToggleModal(_channelSettings!);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.GiphyRatingGButton)]
    public async Task SelectGiphyRatingGAsync()
    {
        _channelSettings!.GiphyRating = "g";

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.GiphyRatingPgButton)]
    public async Task SelectGiphyRatingPgAsync()
    {
        _channelSettings!.GiphyRating = "pg";

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.GiphyRatingPg13Button)]
    public async Task SelectGiphyRatingPg13Async()
    {
        _channelSettings!.GiphyRating = "pg-13";

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.GiphyRatingRButton)]
    public async Task SelectGiphyRatingRAsync()
    {
        _channelSettings!.GiphyRating = "r";

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.GiphyRatingAllButton)]
    public async Task SelectGiphyRatingAllAsync()
    {
        _channelSettings!.GiphyRating = "all";

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.ResetGiphyRatingButton)]
    public async Task ResetGiphyRatingAsync()
    {
        PendingConfirmations[Context.Channel.Id] = (PendingClearAction.GiphyRating, "Are you sure you want to reset the giphy rating to PG?");

        var component = _settingsMessageComponentFactory.BuildConfirmationModal(PendingConfirmations[Context.Channel.Id].Message);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.ClearKeywordButton)]
    public async Task ClearKeywordAsync()
    {
        PendingConfirmations[Context.Channel.Id] = (PendingClearAction.Keyword, "Are you sure you want to clear random gif keywords?");

        var component = _settingsMessageComponentFactory.BuildConfirmationModal(PendingConfirmations[Context.Channel.Id].Message);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.TrendingGifsWithKeywordButton)]
    public async Task OpenKeywordModalAsync()
    {
        var gifKeyword = _channelSettings!.GifKeyword;

        var gifKeywordModal = new ModalBuilder()
            .WithTitle("Set keywords to post gifs of when up-to-date")
            .WithCustomId(InteractionId.TrendingGifsWithKeywordModal)
            .AddTextInput("Keywords", InteractionId.TrendingGifsWithKeywordTextInput, placeholder: "cats", required: true, value: gifKeyword)
            .Build();

        await Context.Interaction.RespondWithModalAsync(gifKeywordModal);
    }

    [ComponentInteraction(InteractionId.ClearPostingHoursButton)]
    public async Task ClearPostingHoursAsync()
    {
        PendingConfirmations[Context.Channel.Id] = (PendingClearAction.PostingHours, "Are you sure you want to clear posting hours?");

        var component = _settingsMessageComponentFactory.BuildConfirmationModal(PendingConfirmations[Context.Channel.Id].Message);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    [ComponentInteraction(InteractionId.ConfirmClearButton)]
    public async Task ConfirmClearAsync()
    {
        if (!PendingConfirmations.TryGetValue(Context.Channel.Id, out var confirmation))
        {
            await _interactionUpdater.RefreshInteractionAsync(_channelSettings!, Context.Channel.Name, Context.Interaction);
            return;
        }

        var action = confirmation.Action;

        switch (action)
        {
            case PendingClearAction.HowOften:
                _channelSettings!.Frequency = 30;
                _channelSettings.IntervalId = (int)IntervalDescription.Minutes;
                break;

            case PendingClearAction.PostingBehavior:
                _channelSettings!.GifPostingBehaviorId = (int)GifPostingBehaviorKind.TrendingGifsOnly;
                break;

            case PendingClearAction.GifSources:
                _channelSettings!.GifSource = GifSourceKind.None;
                break;

            case PendingClearAction.GifRetention:
                _channelSettings!.RetentionDays = 14;
                break;

            case PendingClearAction.GiphyRating:
                _channelSettings!.GiphyRating = "pg";
                break;

            case PendingClearAction.Keyword:
                _channelSettings!.GifKeyword = null;
                break;

            case PendingClearAction.PostingHours:
                _channelSettings!.PostingHoursFrom = null;
                _channelSettings.PostingHoursTo = null;
                _channelSettings.UtcOffset = null;
                break;
        }

        PendingConfirmations.Remove(Context.Channel.Id);

        await _trendingGiphyBotContext.SaveChangesAsync();

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings!, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.CancelClearButton)]
    public async Task CancelClearAsync()
    {
        PendingConfirmations.Remove(Context.Channel.Id);

        await _interactionUpdater.RefreshInteractionAsync(_channelSettings!, Context.Channel.Name, Context.Interaction);
    }

    [ComponentInteraction(InteractionId.TrendingPostingHoursButton)]
    public async Task OpenPostingHoursModalAsync()
    {
        var channelSettings = _channelSettings!;
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
