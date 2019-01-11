using Discord;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Extensions
{
    static class EmbedBuilderExtensions
    {
        internal static EmbedBuilder WithRandomConfigFields(this EmbedBuilder embedBuilder, JobConfig config)
        {
            var fieldValue = DetermineRandomFieldValue(config.RandomIsOn);
            return embedBuilder.AddInlineField("Random Gifs?", fieldValue)
                .WithRandomSearchString(config);
        }
        static string DetermineRandomFieldValue(bool randomIsOn)
        {
            if (randomIsOn)
                return "yes";
            return "no";
        }
        static EmbedBuilder WithRandomSearchString(this EmbedBuilder embedBuilder, JobConfig config)
        {
            if (string.IsNullOrEmpty(config.RandomSearchString))
                return embedBuilder.AddInlineField("Random Gif Filter", "none");
            return embedBuilder.AddInlineField("Random Gif Filter", $"\"{config.RandomSearchString}\"");
        }
        internal static EmbedBuilder WithQuietHourFields(this EmbedBuilder embedBuilder, JobConfig jobConfig, short hourOffset) =>
            embedBuilder.WithQuietHour("Start Hour", jobConfig.MaxQuietHour, hourOffset)
                .WithQuietHour("End Hour", jobConfig.MinQuietHour, hourOffset);
        static EmbedBuilder WithQuietHour(this EmbedBuilder embedBuilder, string name, short? quietHour, short hourOffset)
        {
            if (quietHour.HasValue)
                return embedBuilder.AddInlineField(name, quietHour.Value);
            return embedBuilder.AddInlineField(name, "none");
        }
    }
}
