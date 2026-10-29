using Jellyfin.Plugin.FinTweaks.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.FinTweaks;

/// <summary>Small web client tweaks, each one with its own switch in the admin page.</summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public static Plugin Instance { get; private set; } = null!;

    public override string Name => "Fin Tweaks";

    public override Guid Id => Guid.Parse("0a8f6f8e-5c3b-4d52-9b8e-3f1f2d6c7e41");

    public override string Description => "Small web client tweaks, each one with its own switch.";

    public IEnumerable<PluginPageInfo> GetPages()
    {
        yield return new PluginPageInfo
        {
            Name = Name,
            EmbeddedResourcePath = $"{GetType().Namespace}.Configuration.configPage.html"
        };
    }
}
