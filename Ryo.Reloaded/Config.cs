using Ryo.Reloaded.Template.Configuration;
using System.ComponentModel;

namespace Ryo.Reloaded.Configuration;

public class Config : Configurable<Config>
{
    [DisplayName("Log Level")]
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

/// <summary>
/// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
/// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
/// </summary>
public class ConfiguratorMixin : ConfiguratorMixinBase
{
    // 
}