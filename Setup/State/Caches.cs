namespace Kyaru.Setup.State;

internal static class Caches
{
    internal static readonly List<DiscordThreadChannel> GifThreads = [];

    // <message ID, interaction>
    internal static readonly Dictionary<ulong, DiscordInteraction> GifResponseMessages = [];
}
