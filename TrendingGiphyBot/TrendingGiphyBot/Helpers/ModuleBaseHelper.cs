using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrendingGiphyBot.Attributes;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Helpers
{
    //TODO all the job state logic could probably be cleaner
    static class ModuleBaseHelper
    {
        static readonly List<int> _ValidMinutesSeconds = new List<int> { 1, 5, 10, 15, 20, 30 };
        static readonly List<int> _ValidHours = new List<int> { 1, 2, 3, 4, 6, 8, 12, 24 };
        internal static string ValidMinutesSecondsString => string.Join(", ", _ValidMinutesSeconds.Select(s => s.ToString()));
        internal static string ValidHoursString => string.Join(", ", _ValidHours.Select(s => s.ToString()));
        internal static string InvalidConfig(Time time, string validValues) =>
            $"When {nameof(Time)} is {time}, interval must be {validValues}.";
        internal static string InvalidConfigRange(int minimumMinutes, int maximumMinutes) =>
            $"Interval must be between {minimumMinutes} and {maximumMinutes} seconds.";
        internal static List<EmbedFieldBuilder> BuildFields<T>() where T : ModuleBase =>
            typeof(T).GetMethods().OrderBy(s => s.Name).Select(method =>
            {
                var isCommand = method.GetCustomAttribute<CommandAttribute>() != null;
                if (isCommand)
                    return BuildFields(method);
                return null;
            }).Where(s => s != null).SelectMany(s => s).ToList();
        static List<EmbedFieldBuilder> BuildFields(MethodInfo method)
        {
            var commandText = GetMethodSignature(method);
            var embedFieldBuilder = new EmbedFieldBuilder()
                .WithName($"{commandText}");
            var fields = new List<EmbedFieldBuilder>();
            var methodSummary = method.GetCustomAttribute<SummaryAttribute>();
            var parameterInfos = method.GetParameters();
            if (parameterInfos.Any())
            {
                fields.Add(embedFieldBuilder
                    .WithValue($"{methodSummary.Text} *Parameters*:"));
                fields.AddRange(parameterInfos.Select(s =>
                {
                    var parameterSummary = s.GetCustomAttribute<SummaryAttribute>().Text;
                    return new EmbedFieldBuilder()
                        .WithIsInline(true)
                        .WithName(s.Name)
                        .WithValue(parameterSummary);
                }));
            }
            else
                fields.Add(embedFieldBuilder
                    .WithValue(methodSummary.Text));
                var example = method.GetCustomAttribute<ExampleAttribute>();
            if (example != null)
                fields.Add(new EmbedFieldBuilder()
                    .WithIsInline(true)
                    .WithName(example.Name)
                    .WithValue(example.Text));
            return fields;
        }
        static string GetMethodSignature(MethodInfo method)
        {
            var parameter = method.GetParameters().Select(s => $"{s.ParameterType.Name} {s.Name}");
            var parameters = string.Join(", ", parameter);
            return $"{method.Name}({parameters})";
        }
        static string RemoveNamespace(string parameterType) => parameterType.Split(new[] { '.' }).Last();
        internal static JobConfigState DetermineJobConfigState(int interval, Time time, int minimumSeconds, int maximumSeconds)
        {
            var configgedSeconds = DetermineConfiggedSeconds(interval, time);
            if (configgedSeconds >= minimumSeconds)
            {
                if (configgedSeconds <= maximumSeconds)
                    switch (time)
                    {
                        case Time.Hour:
                        case Time.Hours:
                            if (_ValidHours.Contains(interval))
                                return JobConfigState.Valid;
                            return JobConfigState.InvalidHours;
                        case Time.Minute:
                        case Time.Minutes:
                            return IsValid(interval, JobConfigState.InvalidMinutes);
                        case Time.Second:
                        case Time.Seconds:
                            return IsValid(interval, JobConfigState.InvalidSeconds);
                        default:
                            return JobConfigState.InvalidTime;
                    }
                return JobConfigState.IntervallTooBig;
            }
            return JobConfigState.IntervalTooSmall;
        }
        static JobConfigState IsValid(int interval, JobConfigState invalidState)
        {
            var isValidMinuteSecond = _ValidMinutesSeconds.Contains(interval);
            if (isValidMinuteSecond)
                return JobConfigState.Valid;
            return invalidState;
        }
        internal static double DetermineConfiggedSeconds(int interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    return TimeSpan.FromHours(interval).TotalSeconds;                case Time.Minute:
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval).TotalSeconds;
                case Time.Second:
                case Time.Seconds:
                    return TimeSpan.FromSeconds(interval).TotalSeconds;
                default:
                    throw new InvalidTimeException(time);
            }
        }
    }
}
