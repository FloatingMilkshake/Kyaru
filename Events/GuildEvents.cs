namespace Kyaru.Events;

internal static class GuildEvents
{
    internal static async Task HandleGuildDownloadCompletedEventAsync(DiscordClient _, GuildDownloadCompletedEventArgs e)
    {
        // We need to wait for GuildDownloadCompleted before running this.
        Setup.State.Discord.GifChannel = await Setup.State.Discord.Client.GetChannelAsync(1503779253534068816);
        await Tasks.GifTasks.ExecuteAsync();
    }
}
