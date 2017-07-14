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
    static class ModuleBaseHelper
    {
        internal static List<EmbedFieldBuilder> BuildFields<T>() where T : ModuleBase
        {
            return typeof(T).GetMethods().OrderBy(s => s.Name).Select(method =>
            {
                var isCommand = method.GetCustomAttribute<CommandAttribute>() != null;
                if (isCommand)
                    return BuildFields(method);
                return null;
            }).Where(s => s != null).SelectMany(s => s).ToList();
        }
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
        internal static bool IsValid(int interval, Time time, int minimumMinutes)
        {
            var configgedMinutes = DetermineConfiggedMinutes(interval, time);
            return configgedMinutes >= minimumMinutes;
        }
        internal static double DetermineConfiggedMinutes(int interval, Time time)
        {
            switch (time)
            {
                case Time.Hours:
                case Time.Hour:
                    return TimeSpan.FromHours(interval).TotalMinutes;
                case Time.Minutes:
                case Time.Minute:
                    return TimeSpan.FromMinutes(interval).TotalMinutes;
                case Time.Seconds:
                case Time.Second:
                    return TimeSpan.FromSeconds(interval).TotalMinutes;
                default:
                    throw new InvalidTimeException(time);
            }
        }
    }
}
