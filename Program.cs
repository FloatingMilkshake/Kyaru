namespace Kyaru;

internal static class Program
{
    internal static async Task Main()
    {
        #region read config.json
        Setup.State.Process.Configuration = JsonConvert.DeserializeObject<Setup.Types.ConfigJson>(await File.ReadAllTextAsync("config.json"));

        if (Setup.State.Process.Configuration is null)
        {
            Console.WriteLine("config.json is malformed. Please be sure it has all of the required values.");
            Environment.Exit(1);
        }
        #endregion read config.json

        #region build Discord client
        var clientBuilder = DiscordClientBuilder.CreateDefault(Setup.State.Process.Configuration.Token, DiscordIntents.AllUnprivileged.AddIntent(DiscordIntents.MessageContents));
#if DEBUG
        clientBuilder.SetLogLevel(LogLevel.Debug);
#else
        clientBuilder.SetLogLevel(LogLevel.Information);
#endif
        clientBuilder.ConfigureExtraFeatures(config =>
        {
            config.LogUnknownEvents = false;
            config.LogUnknownAuditlogs = false;
        });
        clientBuilder.ConfigureEventHandlers(builder =>
        {
            builder.HandleGuildDownloadCompleted(Events.GuildEvents.HandleGuildDownloadCompletedEventAsync);
            builder.HandleComponentInteractionCreated(Events.InteractionEvents.HandleComponentInteractionCreatedEventAsync);
        });
        clientBuilder.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = TimeSpan.FromSeconds(300)
        });
        clientBuilder.UseCommands((_, extension) =>
        {
            var commandTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                t.IsClass && t.Namespace is not null && t.Namespace.Contains("Kyaru.Commands") &&
                !t.IsNested).ToList();
            extension.AddCommands(commandTypes);
        }, new CommandsConfiguration
        {
            UseDefaultCommandErrorHandler = false,
        });
        Setup.State.Discord.Client = clientBuilder.Build();
        #endregion build Discord client

        await Setup.State.Discord.Client.ConnectAsync();

        await Task.Delay(Timeout.InfiniteTimeSpan);
    }
}
