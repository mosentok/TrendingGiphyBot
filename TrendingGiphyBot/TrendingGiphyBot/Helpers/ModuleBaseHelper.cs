using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrendingGiphyBot.Attributes;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Jobs;

namespace TrendingGiphyBot.Helpers
{
    static class ModuleBaseHelper
    {
        internal static string ValidMinutesSecondsString => string.Join(", ", Job.ValidMinutesSeconds.Select(s => s.ToString()));
        internal static string ValidHoursString => string.Join(", ", Job.ValidHours.Select(s => s.ToString()));
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
    }
}
