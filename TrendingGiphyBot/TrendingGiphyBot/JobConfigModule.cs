using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;
using System.Reflection;
using System.Linq;

namespace TrendingGiphyBot
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : ModuleBase
    {
        Job _Job => (Context as JobConfigCommandContext).Job;
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
            var serialized = JsonConvert.SerializeObject(_Job.JobConfig, Formatting.Indented);
            await ReplyAsync(serialized);
        }
        [Command(nameof(Set))]
        [Summary("Sets the " + nameof(JobConfig) + ".")]
        public async Task Set(
            [Summary(nameof(JobConfig.Interval) + " to set.")]
            int interval,
            [Summary(nameof(JobConfig.Time) + " to set.")]
            Time time)
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<Config>(contents);
            config.JobConfig.Interval = interval;
            config.JobConfig.Time = time;
            var serialized = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, serialized);
            _Job.Restart(config.JobConfig);
            await Get();
        }
    }
}
