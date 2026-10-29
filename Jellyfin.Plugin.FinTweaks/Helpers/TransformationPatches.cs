using Jellyfin.Plugin.FinTweaks.Model;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;

namespace Jellyfin.Plugin.FinTweaks.Helpers;

/// <summary>File Transformation callbacks.</summary>
public static class TransformationPatches
{
    internal static IServerConfigurationManager? ServerConfigurationManager { get; set; }

    /// <summary>Adds one script tag to index.html when at least one tweak is on.</summary>
    public static string IndexHtml(PatchRequestPayload payload)
    {
        string contents = payload.Contents ?? string.Empty;
        var config = Plugin.Instance.Configuration;
        if (!Tweaks.Enabled(config).Any())
        {
            return contents;
        }

        string baseUrl = ServerConfigurationManager?.GetNetworkConfiguration().BaseUrl.Trim().Trim('/') ?? string.Empty;
        string root = baseUrl.Length > 0 ? "/" + baseUrl : string.Empty;
        string tag = $"<script plugin=\"FinTweaks\" src=\"{root}/FinTweaks/tweaks.js?v={Tweaks.CacheKey(config)}\" defer></script>";
        return contents.Replace("</body>", tag + "</body>", StringComparison.Ordinal);
    }
}
