﻿using Discord;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Helpers
{
    static class EmbedBuilderExtensions
    {
        internal static EmbedBuilder WithRandomConfigFields(this EmbedBuilder embedBuilder, JobConfig config) =>
            embedBuilder.AddInlineField(nameof(config.RandomIsOn), config.RandomIsOn)
                .WithRandomSearchString(config);
        internal static EmbedBuilder WithQuietHourFields(this EmbedBuilder embedBuilder, JobConfig jobConfig, short hourOffset) =>
            embedBuilder.WithQuietHour(nameof(jobConfig.MinQuietHour), jobConfig.MinQuietHour, hourOffset)
                .WithQuietHour(nameof(jobConfig.MaxQuietHour), jobConfig.MaxQuietHour, hourOffset);
        static EmbedBuilder WithRandomSearchString(this EmbedBuilder embedBuilder, JobConfig config)
        {
            if (!string.IsNullOrEmpty(config.RandomSearchString))
                return embedBuilder.AddInlineField(nameof(config.RandomSearchString), config.RandomSearchString);
            return embedBuilder.AddInlineField(nameof(config.RandomSearchString), "null");
        }
        static EmbedBuilder WithQuietHour(this EmbedBuilder embedBuilder, string name, short? quietHour, short hourOffset)
        {
            var local = UndoHourOffset(quietHour, hourOffset);
            if (local.HasValue)
                return embedBuilder.AddInlineField(name, local.Value);
            return embedBuilder.AddInlineField(name, "null");
        }
        static short? UndoHourOffset(short? hour, short hourOffset)
        {
            if (hour.HasValue)
                return (short)((hour - hourOffset) % 24);
            return null;
        }
    }
}