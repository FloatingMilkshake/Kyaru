namespace Kyaru.Events;

internal static class InteractionEvents
{
    internal static async Task HandleComponentInteractionCreatedEventAsync(DiscordClient _, ComponentInteractionCreatedEventArgs e)
    {
        if (e.Id == "button-callback-gif-hide")
        {
            await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);

            var cached = Setup.State.Caches.GifResponseMessages.TryGetValue(e.Message.Id, out var interaction);
            if (cached)
            {
                await interaction.DeleteOriginalResponseAsync();
                Setup.State.Caches.GifResponseMessages.Remove(e.Message.Id);
            }
        }
    }
}
