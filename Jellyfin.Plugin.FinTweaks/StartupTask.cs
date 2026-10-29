using System.Runtime.Loader;
using Jellyfin.Plugin.FinTweaks.Helpers;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.FinTweaks;

/// <summary>Registers the index.html transformation with File Transformation when the server starts.</summary>
public class StartupTask : IScheduledTask
{
    private readonly ILogger<StartupTask> _logger;

    public StartupTask(IServerConfigurationManager serverConfigurationManager, ILogger<StartupTask> logger)
    {
        TransformationPatches.ServerConfigurationManager = serverConfigurationManager;
        _logger = logger;
    }

    public string Name => "Fin Tweaks Startup";

    public string Key => "Jellyfin.Plugin.FinTweaks.Startup";

    public string Description => "Registers the Fin Tweaks script with File Transformation.";

    public string Category => "Startup Services";

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        var assembly = AssemblyLoadContext.All.SelectMany(x => x.Assemblies)
            .FirstOrDefault(x => x.FullName?.Contains(".FileTransformation", StringComparison.Ordinal) ?? false);
        var register = assembly?.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface")?.GetMethod("RegisterTransformation");
        if (register == null)
        {
            _logger.LogWarning("File Transformation not found: Fin Tweaks needs it to add its script to the web client");
            return Task.CompletedTask;
        }

        var payload = new JObject
        {
            { "id", "5d0c8b1e-7a4f-4c2e-9f61-2b8d4e3a9c70" },
            { "fileNamePattern", "index.html" },
            { "callbackAssembly", GetType().Assembly.FullName },
            { "callbackClass", typeof(TransformationPatches).FullName },
            { "callbackMethod", nameof(TransformationPatches.IndexHtml) },
        };
        register.Invoke(null, new object?[] { payload });
        _logger.LogInformation("Fin Tweaks registered with File Transformation");
        return Task.CompletedTask;
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo { Type = TaskTriggerInfoType.StartupTrigger };
    }
}
