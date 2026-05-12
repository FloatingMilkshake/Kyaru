namespace Kyaru.Tasks;

internal class GifTasks
{
    internal static async Task ExecuteAsync()
    {
        while (true)
        {
            await UpdateCachedGifThreadsListAsync();
            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }

    private static async Task UpdateCachedGifThreadsListAsync()
    {
        Setup.State.Caches.GifThreads.RemoveAll(x => true);
        Setup.State.Caches.GifThreads.AddRange(Setup.State.Discord.GifChannel.Threads);
        Setup.State.Caches.GifThreads.AddRange(await Setup.State.Discord.GifChannel.GetAllThreadsAsync());
    }
}
