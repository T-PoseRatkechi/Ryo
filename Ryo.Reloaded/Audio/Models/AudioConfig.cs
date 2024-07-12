using Ryo.Definitions.Enums;

namespace Ryo.Reloaded.Audio.Models;

/// <summary>
/// Audio configuration.
/// </summary>
internal class AudioConfig
{
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
