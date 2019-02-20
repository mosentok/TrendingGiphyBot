using Discord;
using Discord.Rest;
using System;
using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class DiscordHelper : IDiscordHelper, IDisposable
    {
        readonly TaskCompletionSource<bool> _LoggedInSource = new TaskCompletionSource<bool>();
        readonly TaskCompletionSource<bool> _LoggedOutSource = new TaskCompletionSource<bool>();
        readonly DiscordRestClient _DiscordClient = new DiscordRestClient();
        readonly string _BotToken;
        public DiscordHelper(string botToken)
        {
            _BotToken = botToken;
        }
        public async Task LogInAsync()
        {
            _DiscordClient.LoggedIn += LoggedIn;
            await _DiscordClient.LoginAsync(TokenType.Bot, _BotToken);
            await _LoggedInSource.Task;
            _DiscordClient.LoggedIn -= LoggedIn;
        }
        Task LoggedIn()
        {
            _LoggedInSource.SetResult(true);
            return Task.CompletedTask;
        }
        public async Task LogOutAsync()
        {
            _DiscordClient.LoggedOut += LoggedOut;
            await _DiscordClient.LogoutAsync();
            await _LoggedOutSource.Task;
            _DiscordClient.LoggedOut -= LoggedOut;
        }
        Task LoggedOut()
        {
            _LoggedOutSource.SetResult(true);
            return Task.CompletedTask;
        }
        public async Task<IMessageChannel> GetChannelAsync(ulong id)
        {
            return await _DiscordClient.GetChannelAsync(id) as IMessageChannel;
        }
        public void Dispose()
        {
            _DiscordClient?.Dispose();
        }
    }
}
