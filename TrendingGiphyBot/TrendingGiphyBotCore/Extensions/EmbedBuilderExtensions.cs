using Discord;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Extensions
{
    public static class EmbedBuilderExtensions
    {
        static EmbedBuilder AddInlineField(this EmbedBuilder embedBuilder, string name, object value) => embedBuilder.AddField(name, value, true);
        public static EmbedBuilder WithHowOften(this EmbedBuilder embedBuilder, JobConfigContainer config)
        {
            if (config.Interval.HasValue && !string.IsNullOrEmpty(config.Time))
            {
                var every = $"```less\n{config.Interval} {config.Time.ToLower()}```";
                return embedBuilder.AddInlineField("How Often?", every);
            }
            return embedBuilder.AddInlineField("How Often?", "```less\nnever```");
        }
        public static EmbedBuilder WithRandomConfigFields(this EmbedBuilder embedBuilder, JobConfigContainer config)
        {
            var fieldValue = DetermineRandomFieldValue(config);
            return embedBuilder.AddInlineField("Random Gifs?", $"```yaml\n{fieldValue}```");
        }
        static string DetermineRandomFieldValue(JobConfigContainer config)
        {
            if (!string.IsNullOrEmpty(config.RandomSearchString))
                return $"yes, of \"{config.RandomSearchString}\"";
            return "no";
        }
        public static EmbedBuilder WithQuietHourFields(this EmbedBuilder embedBuilder, JobConfigContainer jobConfig)
        {
            var fieldValue = DetermineQuietHoursFieldValue(jobConfig);
            return embedBuilder.AddInlineField("Trend When?", $"```fix\n{fieldValue}```");
        }
        static string DetermineQuietHoursFieldValue(JobConfigContainer jobConfig)
        {
            if (jobConfig.MinQuietHour.HasValue && jobConfig.MaxQuietHour.HasValue)
                return $"between {jobConfig.MaxQuietHour.Value} and {jobConfig.MinQuietHour.Value} o'clock";
            return "all the time";
        }
    }
}
