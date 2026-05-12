namespace Kyaru.Extensions;

internal static class DiscordChannelExtensions
{
    extension(DiscordChannel channel)
    {
        internal async Task<IReadOnlyList<DiscordThreadChannel>> GetAllThreadsAsync()
        {
            List<DiscordThreadChannel> threads = channel.Threads.ToList();

            var hasMore = true;
            List<DiscordThreadChannel> publicArchivedThreads = [];
            while (hasMore)
            {
                var result = await channel.ListPublicArchivedThreadsAsync(before: publicArchivedThreads.Count == 0 ? null : publicArchivedThreads.Last().ThreadMetadata.ArchiveTimestamp);
                publicArchivedThreads.AddRange(result.Threads);
                hasMore = result.HasMore;
            }
            threads.AddRange(publicArchivedThreads);

            return threads;
        }
    }
}
