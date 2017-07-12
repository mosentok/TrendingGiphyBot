﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrendingGiphyBot.Attributes;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Helpers
{
    static class ModuleBaseHelper
    {
        internal static HelpContainer BuildHelpContainer<T>() where T : ModuleBase
        {
            return new HelpContainer(typeof(T).GetMethods().Select(method =>
            {
                var isCommand = method.GetCustomAttribute<CommandAttribute>() != null;
                if (isCommand)
                {
                    var commandText = GetMethodSignature(method);
                    var fields = new List<FieldContainer>();
                    var parameterInfos = method.GetParameters();
                    //TODO is any really required?
                    fields.AddRange(parameterInfos.Select(s =>
                    {
                        var parameterSummary = s.GetCustomAttribute<SummaryAttribute>().Text;
                        return new FieldContainer(s.Name, parameterSummary);
                    }));
                    var example = method.GetCustomAttribute<ExampleAttribute>();
                    if (example != null)
                        fields.Add(new FieldContainer(example.Name, example.Text));
                    var methodSummary = method.GetCustomAttribute<SummaryAttribute>();
                    return new MethodContainer(commandText, methodSummary.Text, fields);
                }
                return null;
            }).Where(s => s != null)
            .OrderBy(s => s.Name));
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
            var configgedMinutes = DetermineJobIntervalSeconds(interval, time);
            return configgedMinutes >= minimumMinutes;
        }
        internal static double DetermineJobIntervalSeconds(int interval, Time time)
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
