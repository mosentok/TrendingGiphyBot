using Discord;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.ToggleModal;

public interface IToggleModalComponentBuilder
{
    MessageComponent BuildToggleModal(string title, ButtonBuilder[] optionButtons);
}
