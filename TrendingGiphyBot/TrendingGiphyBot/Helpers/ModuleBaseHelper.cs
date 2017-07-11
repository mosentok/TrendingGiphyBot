using Discord.Commands;
using System;
using System.Linq;
using System.Reflection;
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
                var command = method.GetCustomAttribute<CommandAttribute>();
                if (command != null)
                {
                    var methodSummary = method.GetCustomAttribute<SummaryAttribute>();
                    var parameters = method.GetParameters().Select(parameter =>
                    {
                        var parameterSummary = parameter.GetCustomAttribute<SummaryAttribute>().Text;
                        return new ParameterContainer(parameter.Name, parameterSummary);
                    });
                    return new MethodContainer(command.Text, methodSummary.Text, parameters);
                }
                return null;
            }).Where(s => s != null)
            .OrderBy(s => s.Name));
        }
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
