using System.Threading.Tasks;
using Discord;

namespace TrendingGiphyBotFunctions.Helpers
{
    public interface IDiscordHelper
    {
        Task<IMessageChannel> GetChannelAsync(ulong id);
        Task LogInAsync();
        Task LogOutAsync();
    }
}