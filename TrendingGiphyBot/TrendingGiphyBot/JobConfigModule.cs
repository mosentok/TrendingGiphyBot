using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using GiphyDotNet.Manager;
using Discord.WebSocket;
using System;

namespace TrendingGiphyBot
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : ModuleBase
    {
        List<Job> _Jobs => (Context as JobConfigCommandContext).Jobs;
        JobConfigDal _ChannelJobConfigDal => (Context as JobConfigCommandContext).ChannelJobConfigDal;
        Giphy _GiphyClient => (Context as JobConfigCommandContext).GiphyClient;
        [Command]
        [Summary("Help menu for " + nameof(JobConfig) + ".")]
        [Alias(nameof(Help))]
        public async Task Help()
        {
            var methodContainers = BuildMethodContainers<JobConfigModule>();
            var helpMethod = typeof(JobConfigModule).GetMethod(nameof(Help));
            var helpSummary = helpMethod.GetCustomAttribute<SummaryAttribute>();
            var helpAlias = helpMethod.GetCustomAttribute<AliasAttribute>();
            var helpContainer = new HelpContainer(nameof(Help), helpSummary.Text, helpAlias.Aliases, methodContainers);
            var serialized = JsonConvert.SerializeObject(helpContainer, Formatting.Indented);
            await ReplyAsync(serialized);
        }
        static IOrderedEnumerable<MethodContainer> BuildMethodContainers<T>() where T : ModuleBase
        {
            var type = typeof(T);
            var methods = type.GetMethods().Where(s => s.Name != nameof(Help));
            return methods.Select(method =>
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
            .OrderBy(s => s.Name);
        }
        [Command(nameof(Get))]
        [Summary("Gets the " + nameof(JobConfig) + ".")]
        public async Task Get()
        {
            var any = await _ChannelJobConfigDal.Any(Context.Channel.Id);
            if (any)
            {
                var config = await _ChannelJobConfigDal.Get(Context.Channel.Id);
                var serialized = JsonConvert.SerializeObject(config, Formatting.Indented);
                await ReplyAsync(serialized);
            }
            else
                await ReplyAsync($"{Context.Channel.Id} not configured.");
        }
        [Command(nameof(Set))]
        [Summary("Sets the " + nameof(JobConfig) + ".")]
        public async Task Set(
            [Summary(nameof(JobConfig.Interval) + " to set.")]
            int interval,
            [Summary(nameof(JobConfig.Time) + " to set.")]
            Time time)
        {
            var isValid = IsValid(interval, time);
            if (isValid)
            {
                var config = new JobConfig
                {
                    ChannelId = Context.Channel.Id,
                    Interval = interval,
                    Time = time.ToString()
                };
                var any = await _ChannelJobConfigDal.Any(Context.Channel.Id);
                if (any)
                    await _ChannelJobConfigDal.Update(config);
                else
                {
                    await _ChannelJobConfigDal.Insert(config);
                    _Jobs.Add(new Job(_GiphyClient, Context.Client as DiscordSocketClient, config));
                }
                await Get();
            }
            else
                await ReplyAsync($"{nameof(JobConfig.Interval)} and {nameof(JobConfig.Time)} must combine to at least 10 minutes.");
        }
        static bool IsValid(int interval, Time time)
        {
            var configgedMinutes = DetermineJobIntervalSeconds(interval, time);
            return configgedMinutes >= 10;
        }
        static double DetermineJobIntervalSeconds(int interval, Time time)
        {
            switch (time)
            {
                case Time.Hours:
                    return TimeSpan.FromHours(interval).TotalMinutes;
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval).TotalMinutes;
                case Time.Seconds:
                    return TimeSpan.FromSeconds(interval).TotalMinutes;
                default:
                    throw new InvalidTimeException(time);
            }
        }
    }
}
