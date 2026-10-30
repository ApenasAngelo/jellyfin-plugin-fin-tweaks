using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.FinTweaks.Configuration;

/// <summary>One switch per tweak. Every tweak is off until the admin turns it on.</summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>The search button always searches the whole server (not only the library being browsed).</summary>
    public bool GlobalSearch { get; set; }

    /// <summary>A trackpad gesture over a row moves only the row or only the page, as its start decides.</summary>
    public bool WheelAxisLock { get; set; }
}
