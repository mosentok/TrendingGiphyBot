using Discord;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Extensions
{
    static class EmbedBuilderExtensions
    {
        internal static EmbedBuilder WithHowOften(this EmbedBuilder embedBuilder, JobConfig config)
        {
            var every = $"```less\n{config.Interval} {config.Time.ToLower()}```";
            return embedBuilder.AddInlineField("How Often?", every);
        }
        internal static EmbedBuilder WithRandomConfigFields(this EmbedBuilder embedBuilder, JobConfig config)
        {
            var fieldValue = DetermineRandomFieldValue(config);
            return embedBuilder.AddInlineField("Random Gifs?", $"```yaml\n{fieldValue}```");
        }
        static string DetermineRandomFieldValue(JobConfig config)
        {
            if (config.RandomIsOn)
            {
                if (!string.IsNullOrEmpty(config.RandomSearchString))
                    return $"yes, of \"{config.RandomSearchString}\"";
                return "yes";
            }
            return "no";
        }
        internal static EmbedBuilder WithQuietHourFields(this EmbedBuilder embedBuilder, JobConfig jobConfig)
        {
            var fieldValue = DetermineQuietHoursFieldValue(jobConfig);
            return embedBuilder.AddInlineField("Trend When?", $"```fix\n{fieldValue}```");
        }
        static string DetermineQuietHoursFieldValue(JobConfig jobConfig)
        {
            if (jobConfig.MinQuietHour.HasValue && jobConfig.MaxQuietHour.HasValue)
                return $"between {jobConfig.MaxQuietHour.Value} and {jobConfig.MinQuietHour.Value}";
            return "all the time";
        }
    }
}
