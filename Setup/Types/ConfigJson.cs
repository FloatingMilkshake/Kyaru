namespace Kyaru.Setup.Types;

internal sealed class ConfigJson
{
    [JsonProperty("token")] internal string Token { get; private set; }
}
