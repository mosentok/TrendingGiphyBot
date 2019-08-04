using System.Threading.Tasks;
using Discord;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public interface IDiscordWrapper
    {
        Task<IMessageChannel> GetChannelAsync(decimal id);
        Task LogInAsync(string botToken);
        Task LogOutAsync();
    }
}