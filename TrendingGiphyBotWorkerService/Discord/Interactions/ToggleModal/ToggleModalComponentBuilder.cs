using Discord;
using Microsoft.Extensions.Options;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.ToggleModal;

[RegisterSingleton]
public class ToggleModalComponentBuilder(IOptionsMonitor<AppConfig> _appConfig) : IToggleModalComponentBuilder
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

        var attributionUrls = _appConfig.CurrentValue.Attribution.AttachmentFileNames.Select(fileName => $"attachment://{fileName}").ToArray();

        componentBuilder = componentBuilder
            .WithSeparator()
            .WithMediaGallery(attributionUrls);

        return componentBuilder.Build();
    }
}
