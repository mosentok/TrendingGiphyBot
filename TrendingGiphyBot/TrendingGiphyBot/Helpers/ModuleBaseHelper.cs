using Discord.Commands;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TrendingGiphyBot.Containers;

namespace TrendingGiphyBot.Helpers
{
    static class ModuleBaseHelper
    {
        internal static HelpContainer BuildHelpContainer<T>([CallerMemberName] string name = null) where T : ModuleBase
        {
            var methodContainers = BuildMethodContainers<T>(name);
            var helpMethod = typeof(T).GetMethod(name);
            var helpSummary = helpMethod.GetCustomAttribute<SummaryAttribute>();
            var helpAlias = helpMethod.GetCustomAttribute<AliasAttribute>();
            var helpContainer = new HelpContainer(name, helpSummary.Text, helpAlias.Aliases, methodContainers);
            return helpContainer;
        }
        static IOrderedEnumerable<MethodContainer> BuildMethodContainers<T>(string name) where T : ModuleBase
        {
            var type = typeof(T);
            var methods = type.GetMethods().Where(s => s.Name != name);
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
    }
}
