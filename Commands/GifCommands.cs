namespace Kyaru.Commands;

internal class GifCommands
{
    [Command("gif")]
    [Description("Search for a GIF.")]
    [AllowedProcessors(typeof(SlashCommandProcessor))]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task GifCommandAsync(SlashCommandContext ctx,
        [SlashAutoCompleteProvider(typeof(GifCategoryAutoCompleteProvider))]
        [Parameter("category"), Description("Type to search... Pick a category.")] string category)
    {
        var thread = Setup.State.Caches.GifThreads.FirstOrDefault(t => t.Name.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (thread is null)
        {
            await ctx.RespondAsync("That's not how you use this... Pick one of the categories.", ephemeral: true);
            return;
        }

        var gifUrls = (await thread.GetMessagesAsync().ToListAsync()).Select(m => m.Content);

        List<Page> pages = [];
        foreach (var gifUrl  in gifUrls)
        {
            pages.Add(new Page(content: gifUrl));
        }

        var leftSkipButton = new DiscordButtonComponent(DiscordButtonStyle.Primary, "leftskip", "First");
        var leftButton = new DiscordButtonComponent(DiscordButtonStyle.Primary, "left", "Back");
        var rightButton = new DiscordButtonComponent(DiscordButtonStyle.Primary, "right", "Next");
        var rightSkipButton = new DiscordButtonComponent(DiscordButtonStyle.Primary, "rightskip", "Last");
        var stopButton = new DiscordButtonComponent(DiscordButtonStyle.Danger, "stop", "Hide");

        if (pages.Count > 1)
        {
            await ctx.Interaction.SendPaginatedResponseAsync(true, ctx.User, pages,
                new PaginationButtons { SkipLeft = leftSkipButton, Left = leftButton, Right = rightButton, SkipRight = rightSkipButton, Stop = stopButton },
                deletion: ButtonPaginationBehavior.DeleteMessage);
        }
        else
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(pages.First().Content)
                    .AddActionRowComponent([new DiscordButtonComponent(DiscordButtonStyle.Danger, "button-callback-gif-hide", "Hide")])
                    .AsEphemeral(true));

            var msg = await ctx.GetResponseAsync();

            Setup.State.Caches.GifResponseMessages.Add(msg.Id, ctx.Interaction);
        }
    }

    private class GifCategoryAutoCompleteProvider : IAutoCompleteProvider
    {
        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
        {
            List<DiscordAutoCompleteChoice> choices = [];

            var focusedOption = ctx.Options.FirstOrDefault(x => x.Focused);
            if (focusedOption is not null)
            {
                var matchingThreads = Setup.State.Caches.GifThreads.Where(t => t.Name.Contains(focusedOption.Value.ToString(), StringComparison.OrdinalIgnoreCase));
                choices.AddRange(matchingThreads.Select(t => new DiscordAutoCompleteChoice(t.Name, t.Name)));
            }

            return choices.Take(25);
        }
    }
}
