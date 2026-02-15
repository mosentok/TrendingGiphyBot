using Discord;
using Injectio;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.ToggleModal;

[RegisterSingleton]
public class ToggleModalComponentBuilder : IToggleModalComponentBuilder
{
    public MessageComponent BuildToggleModal(string title, ButtonBuilder[] optionButtons)
    {
        var componentBuilder = new ComponentBuilderV2();

        componentBuilder = componentBuilder.WithTextDisplay($"# {title}");

        var chunkedButtons = optionButtons.Chunk(5);

        foreach (var chunk in chunkedButtons)
            componentBuilder = componentBuilder.WithActionRow(chunk);

        componentBuilder = componentBuilder.WithSeparator();

        var backButton = new ButtonBuilder()
            .WithCustomId(InteractionId.BackButton)
            .WithLabel("Back")
            .WithStyle(ButtonStyle.Secondary);

        componentBuilder = componentBuilder.WithActionRow([backButton]);

        componentBuilder = componentBuilder.WithMediaGallery([
            "attachment://PoweredBy_200_Horizontal_Light-Backgrounds_With_Logo.gif",
            "attachment://Powered by KLIPY Horizontal - Yellow&White Logo.png"
        ]);

        return componentBuilder.Build();
    }
}
