using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService;

public class DiscordSocketClientHandler(ILoggerWrapper<DiscordSocketClientHandler> _loggerWrapper, HttpClient _httpClient) : IDiscordSocketClientHandler
{
    public async Task JoinedGuild(SocketGuild arg)
    {
        //TODO post stats to websites that track the bot's server count
        //await _loggerWrapper.SwallowAsync(_FunctionWrapper.PostStatsAsync(_discordSocketClient.CurrentUser.Id, _discordSocketClient.Guilds.Count));
    }

    public async Task LeftGuild(SocketGuild arg)
    {
        //TODO post stats to websites that track the bot's server count
        //await _loggerWrapper.SwallowAsync(async () =>
        //{
        //	await RemoveThisGuildsJobConfigs(arg);
        //	await _FunctionWrapper.PostStatsAsync(_discordSocketClient.CurrentUser.Id, _discordSocketClient.Guilds.Count);
        //});
    }

    public Task Log(LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                _loggerWrapper.LogDiscordMessage(LogLevel.Critical, logMessage);
                break;

            case LogSeverity.Debug:
                _loggerWrapper.LogDiscordMessage(LogLevel.Debug, logMessage);
                break;

            case LogSeverity.Error:
                _loggerWrapper.LogDiscordMessage(LogLevel.Error, logMessage);
                break;

            case LogSeverity.Info:
                _loggerWrapper.LogDiscordMessage(LogLevel.Information, logMessage);
                break;

            case LogSeverity.Verbose:
                _loggerWrapper.LogDiscordMessage(LogLevel.Trace, logMessage);
                break;

            case LogSeverity.Warning:
                _loggerWrapper.LogDiscordMessage(LogLevel.Warning, logMessage);
                break;

            default:
                throw new ThisShouldBeImpossibleException();
        }

        return Task.CompletedTask;
    }
}