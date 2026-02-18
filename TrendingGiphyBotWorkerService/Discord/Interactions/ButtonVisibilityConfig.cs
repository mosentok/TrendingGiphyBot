namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public record ButtonVisibilityConfig(
    bool EnableClearGifSourcesButton,
    bool EnableClearKeywordButton,
    bool EnableClearPostingHoursButton,
    bool EnableResetGifRetentionButton,
    bool EnableResetGiphyRatingButton,
    bool EnableResetHowOftenButton,
    bool EnableResetPostingBehaviorButton
);
