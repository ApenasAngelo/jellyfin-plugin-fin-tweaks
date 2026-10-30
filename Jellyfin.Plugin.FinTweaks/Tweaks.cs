using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Jellyfin.Plugin.FinTweaks.Configuration;

namespace Jellyfin.Plugin.FinTweaks;

/// <summary>The list of tweaks: a script in Web/tweaks and the switch that turns it on.</summary>
public static class Tweaks
{
    private static readonly (string Script, Func<PluginConfiguration, bool> IsEnabled)[] s_all =
    {
        ("global-search", c => c.GlobalSearch),
        ("wheel-axis-lock", c => c.WheelAxisLock),
    };

    public static IEnumerable<string> Enabled(PluginConfiguration config)
        => s_all.Where(t => t.IsEnabled(config)).Select(t => t.Script);

    /// <summary>All enabled scripts in one file, each wrapped so an error in one does not stop the others.</summary>
    public static string BuildScript(PluginConfiguration config)
    {
        var sb = new StringBuilder("'use strict';\n");
        foreach (string script in Enabled(config))
        {
            sb.Append("// ").Append(script).Append('\n')
                .Append("try {\n").Append(ReadScript(script)).Append("\n} catch (e) { console.error('Fin Tweaks: ")
                .Append(script).Append("', e); }\n");
        }

        return sb.ToString();
    }

    /// <summary>Changes whenever the set of enabled tweaks or the plugin version changes (cache key).</summary>
    public static string CacheKey(PluginConfiguration config)
    {
        string text = Plugin.Instance.Version + "|" + string.Join(',', Enabled(config));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)))[..12].ToLowerInvariant();
    }

    private static string ReadScript(string name)
    {
        Assembly assembly = typeof(Tweaks).Assembly;
        using Stream stream = assembly.GetManifestResourceStream($"{typeof(Tweaks).Namespace}.Web.tweaks.{name}.js")
            ?? throw new InvalidOperationException($"Missing tweak script {name}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
