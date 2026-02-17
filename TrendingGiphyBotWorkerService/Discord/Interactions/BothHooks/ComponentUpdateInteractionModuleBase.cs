using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public abstract class ComponentUpdateInteractionModuleBase : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    protected MessageComponent? Component { get; set; }

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        ThisShouldBeImpossibleException.ThrowIf(Component is null);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = Component);
    }
}
