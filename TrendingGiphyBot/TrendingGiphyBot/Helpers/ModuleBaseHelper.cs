using Discord;
using Discord.Commands;
using System;
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
            typeof(T).GetMethods()
            .OrderBy(s => s.Name)
            .Where(s => s.GetCustomAttribute<CommandAttribute>() != null)
            .SelectMany(s => BuildFields(s)).ToList();
        static List<EmbedFieldBuilder> BuildFields(MethodInfo method)
        {
            var methodSignature = GetMethodSignature(method);
            var commandField = new EmbedFieldBuilder()
                .WithName(methodSignature);
            var fields = new List<EmbedFieldBuilder>();
            var methodSummary = method.GetCustomAttribute<SummaryAttribute>();
            var parameterInfos = method.GetParameters();
            if (parameterInfos.Any())
            {
                fields.Add(commandField
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
                fields.Add(commandField
                    .WithValue(methodSummary.Text));
            var exampleAttributes = method.GetCustomAttributes<ExampleAttribute>().ToList();
            if (exampleAttributes.Any())
            {
                var texts = string.Join(Environment.NewLine, exampleAttributes.SelectMany(s => s.Texts));
                fields.Add(new EmbedFieldBuilder()
                    .WithIsInline(false)
                    .WithName(ExampleAttribute.Name)
                    .WithValue(texts));
            }
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
