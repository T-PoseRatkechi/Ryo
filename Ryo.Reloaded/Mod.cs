using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Ryo.Reloaded.Configuration;
using Ryo.Reloaded.CRI;
using Ryo.Reloaded.Template;
using System.Drawing;

namespace Ryo.Reloaded;

public class Mod : ModBase
{
    private readonly IModLoader modLoader;
    private readonly IReloadedHooks hooks;
    private readonly ILogger log;
    private readonly IMod owner;

    private Config config;
    private readonly IModConfig modConfig;

    private readonly CriAtomEx criAtomEx;

    public Mod(ModContext context)
    {
        this.modLoader = context.ModLoader;
        this.hooks = context.Hooks!;
        this.log = context.Logger;
        this.owner = context.Owner;
        this.config = context.Configuration;
        this.modConfig = context.ModConfig;

        Log.Initialize("Ryo", this.log, Color.FromArgb(138, 177, 255));

        this.modLoader.GetController<IStartupScanner>().TryGetTarget(out var scanner);

        this.criAtomEx = new(Path.GetFileNameWithoutExtension(this.modLoader.GetAppConfig().AppId));
        this.criAtomEx.Initialize(scanner!, this.hooks);
        this.ApplyConfig();
    }

    private void ApplyConfig()
    {
        Log.LogLevel = this.config.LogLevel;
        this.criAtomEx.SetDevMode(this.config.DevMode);
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        config = configuration;
        log.WriteLine($"[{modConfig.ModId}] Config Updated: Applying");
        this.ApplyConfig();
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}