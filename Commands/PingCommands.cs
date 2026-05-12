namespace Kyaru.Commands;

internal class PingCommands
{
    [Command("ping")]
    [Description("Check the bot's latency.")]
    [AllowedProcessors(typeof(SlashCommandProcessor))]
    public static async Task PingCommandAsync(SlashCommandContext ctx)
    {
        await ctx.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Ping."));

        var websocketPing = ctx.Client.GetConnectionLatency(0).TotalMilliseconds;
        var msg = await ctx.Interaction.GetOriginalResponseAsync();
        var interactionLatency = Math.Round((DateTime.UtcNow - msg.CreationTimestamp.UtcDateTime).TotalMilliseconds);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent($"Pong. Websocket latency `{websocketPing}ms`, interaction latency `{interactionLatency}ms`."));
    }
}
