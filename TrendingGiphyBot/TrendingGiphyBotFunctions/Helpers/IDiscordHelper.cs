using System.Threading.Tasks;
using Discord;

namespace TrendingGiphyBotFunctions.Helpers
{
    //TODO rename this to Wrapper
    public interface IDiscordHelper
    {
        Task<IMessageChannel> GetChannelAsync(decimal id);
        Task LogInAsync();
        Task LogOutAsync();
    }
}