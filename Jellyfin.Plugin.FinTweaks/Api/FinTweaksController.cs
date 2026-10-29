using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.FinTweaks.Api;

/// <summary>Serves the enabled tweaks as one script. Anonymous because index.html loads it before login.</summary>
[ApiController]
[Route("FinTweaks")]
public class FinTweaksController : ControllerBase
{
    [HttpGet("tweaks.js")]
    [AllowAnonymous]
    [Produces("application/javascript")]
    public ActionResult GetScript()
    {
        // Small file: always revalidate, so switching a tweak takes effect on the next page load.
        Response.Headers.CacheControl = "no-cache";
        return Content(Tweaks.BuildScript(Plugin.Instance.Configuration), "application/javascript; charset=utf-8");
    }
}
