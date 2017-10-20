using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Modules
{
    [Group(_Name)]
    public class SetRandomModule : BotModuleBase
    {
        const string _Name = "SetRandom";
        public SetRandomModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '{GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)}' or '{GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help() => await HelpMessageReplyAsync();
        [Command(nameof(Get))]
        public async Task Get()
        {
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = await GlobalConfig.JobConfigDal.Get(Context.Channel.Id);
                var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
                var author = new EmbedAuthorBuilder()
                    .WithName($"{Context.Channel.Name}'s {_Name}")
                    .WithIconUrl(avatarUrl);
                var embedBuilder = new EmbedBuilder()
                    .WithAuthor(author)
                    .WithRandomConfigFields(config);
                await TryReplyAsync(embedBuilder);
            }
            else
                await TryReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(On))]
        public async Task On(params string[] searchValues)
        {
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var randomSearchString = DetermineRandomSearchString(searchValues);
                if (string.IsNullOrEmpty(randomSearchString) || randomSearchString.Length <= GlobalConfig.Config.RandomSearchStringMaxLength)
                {
                    var config = new JobConfig
                    {
                        ChannelId = Context.Channel.Id,
                        RandomIsOn = true,
                        RandomSearchString = randomSearchString
                    };
                    await GlobalConfig.JobConfigDal.UpdateRandom(config);
                    await Get();
                }
                else
                    await TryReplyAsync($"Random search string must be at most {GlobalConfig.Config.RandomSearchStringMaxLength} characters long.");
            }
            else
                await TryReplyAsync(NotConfiguredMessage);
        }
        static string DetermineRandomSearchString(string[] searchValues)
        {
            if (searchValues.Any())
                return string.Join(" ", searchValues);
            return null;
        }
        [Command(nameof(Off))]
        public async Task Off()
        {
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = new JobConfig { ChannelId = Context.Channel.Id, RandomIsOn = false };
                await GlobalConfig.JobConfigDal.UpdateRandom(config);
                await TryReplyAsync("No more Randies.");
            }
            else
                await TryReplyAsync(NotConfiguredMessage);
        }
    }
}
