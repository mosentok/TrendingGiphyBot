using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TrendingGiphyBot.Helpers
{
    public class MessageHelper
    {
        readonly string _FailedReplyDisclaimer;
        public MessageHelper(string failedReplyDisclaimer)
        {
            _FailedReplyDisclaimer = failedReplyDisclaimer;
        }
        public async Task SendMessageToUser(ICommandContext context, string message, EmbedBuilder embedBuilder)
        {
            if (!string.IsNullOrEmpty(_FailedReplyDisclaimer))
            {
                var failedReplyDisclaimer = string.Format(_FailedReplyDisclaimer, context.Channel.Name);
                if (embedBuilder != null)
                    await SendMessageToUserWithDisclaimerFooter(context, message, failedReplyDisclaimer, embedBuilder);
                else
                    await SendMessageToUserWithDisclaimerText(context, message, failedReplyDisclaimer);
            }
            else
                await context.User.SendMessageAsync(message, embed: embedBuilder);
        }
        async Task SendMessageToUserWithDisclaimerFooter(ICommandContext context, string message, string failedReplyDisclaimer, EmbedBuilder embedBuilder)
        {
            embedBuilder.Footer = new EmbedFooterBuilder()
                .WithText(failedReplyDisclaimer);
            await context.User.SendMessageAsync(message, embed: embedBuilder);
        }
        async Task SendMessageToUserWithDisclaimerText(ICommandContext context, string message, string failedReplyDisclaimer)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(message);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"*{failedReplyDisclaimer}*");
            await context.User.SendMessageAsync(stringBuilder.ToString().TrimEnd());
        }
    }
}
