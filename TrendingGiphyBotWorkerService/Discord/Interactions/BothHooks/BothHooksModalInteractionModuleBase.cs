using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public abstract class BothHooksModalInteractionModuleBase : InteractionModuleBase<SocketInteractionContext<SocketModal>>
{
    protected MessageComponent? Component { get; set; }

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        if (Context.Interaction.HasResponded)
            return;

        ThisShouldBeImpossibleException.ThrowIf(Component is null);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = Component);
    }
}
