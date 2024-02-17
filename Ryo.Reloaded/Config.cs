using Ryo.Reloaded.Template.Configuration;
using System.ComponentModel;

namespace Ryo.Reloaded.Configuration;

public class Config : Configurable<Config>
{
    [DisplayName("Log Level")]
    [DefaultValue(LogLevel.Information)]
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    [DisplayName("Developer Mode")]
    [Description("Display extra information useful for mod development.")]
    [DefaultValue(false)]
    public bool DevMode { get; set; } = false;

    [DisplayName("Preload Audio Data")]
    [Description("Loads all audio files into memory. DON'T USE UNDER NORMAL CIRCUMSTANCES!")]
    [DefaultValue(false)]
    public bool PreloadAudio { get; set; } = false;
}

/// <summary>
/// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
/// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
/// </summary>
public class ConfiguratorMixin : ConfiguratorMixinBase
{
    // 
}