﻿using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using Ryo.Reloaded.Audio;
using Ryo.Reloaded.Configuration;
using Ryo.Reloaded.CRI;
using Ryo.Reloaded.Template;
using System.Diagnostics;
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

    private readonly string game;
    private readonly CriAtomEx criAtomEx;
    private readonly AudioRegistry audioRegistry;
    private readonly AudioService audioService;

    public Mod(ModContext context)
    {
        this.modLoader = context.ModLoader;
        this.hooks = context.Hooks!;
        this.log = context.Logger;
        this.owner = context.Owner;
        this.config = context.Configuration;
        this.modConfig = context.ModConfig;

#if DEBUG
        Debugger.Launch();
#endif

        this.game = Path.GetFileNameWithoutExtension(this.modLoader.GetAppConfig().AppId);

        Log.Initialize($"Ryo ({this.game.ToUpper()})", this.log, Color.FromArgb(138, 177, 255));
        this.modLoader.GetController<IStartupScanner>().TryGetTarget(out var scanner);

        this.criAtomEx = new(this.game);
        this.criAtomEx.Initialize(scanner!, this.hooks);

        this.audioRegistry = new(this.game);
        this.audioService = new(this.criAtomEx, this.audioRegistry);

        this.modLoader.ModLoading += this.OnModLoading;

        this.ApplyConfig();
    }

    private void OnModLoading(IModV1 mod, IModConfigV1 config)
    {
        if (!config.ModDependencies.Contains(this.modConfig.ModId))
        {
            return;
        }

        var modDir = this.modLoader.GetDirectoryForModId(config.ModId);
        var modAudioDir = Path.Join(modDir, "ryo", this.game);
        if (Directory.Exists(modAudioDir))
        {
            this.audioRegistry.AddAudioFolder(modAudioDir);
        }
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
        this.config = configuration;
        this.log.WriteLine($"[{this.modConfig.ModId}] Config Updated: Applying");
        this.ApplyConfig();
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}