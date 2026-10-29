using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.FinTweaks.Model;

/// <summary>What File Transformation passes to the callback: the file as it is about to be served.</summary>
public class PatchRequestPayload
{
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }
}
