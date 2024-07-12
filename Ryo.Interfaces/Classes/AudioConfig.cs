using Ryo.Definitions.Enums;

namespace Ryo.Interfaces.Classes;

/// <summary>
/// Audio configuration.
/// </summary>
public class AudioConfig
{
    /// <summary>
    /// Gets or sets the shared container ID.
    /// Setting this allows for multiple audios to be added
    /// to the same cue container.
    /// </summary>
    public string? SharedContainerId { get; set; }

    public string? CueName { get; set; }

    public string? AcbName { get; set; }

    public int? SampleRate { get; set; }

    public CriAtomFormat? Format { get; set; }

    public int? NumChannels { get; set; }

    public int? PlayerId { get; set; }

    public int[]? CategoryIds { get; set; }

    public float? Volume { get; set; }

    public string? Key { get; set; }

    public string[]? Tags { get; set; }

    public AudioConfig Clone() => (AudioConfig)this.MemberwiseClone();
}
