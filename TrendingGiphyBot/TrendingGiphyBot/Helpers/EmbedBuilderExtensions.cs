using Discord;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Helpers
{
    static class EmbedBuilderExtensions
    {
        internal static EmbedBuilder AddInlineJobConfigField(this EmbedBuilder embedBuilder, JobConfig config)
        {
            if (!string.IsNullOrEmpty(config.RandomSearchString))
                return embedBuilder.AddInlineField(nameof(config.RandomSearchString), config.RandomSearchString);
            return embedBuilder.AddInlineField(nameof(config.RandomSearchString), "null");
        }
    }
}
