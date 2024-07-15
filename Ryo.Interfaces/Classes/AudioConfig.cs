using Ryo.Definitions.Enums;
using Ryo.Interfaces.Enums;

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

    public string? AudioDataName { get; set; }

    public string? AudioFilePath { get; set; }

    public int? SampleRate { get; set; }

    public CriAtomFormat? Format { get; set; }

    public int? NumChannels { get; set; }

    public int? PlayerId { get; set; }

    public int[]? CategoryIds { get; set; }

    public float? Volume { get; set; }

    public string? Key { get; set; }

    public string[]? Tags { get; set; }

    public PlaybackMode? PlaybackMode { get; set; }

    public AudioConfig Clone() => (AudioConfig)this.MemberwiseClone();

    /// <summary>
    /// Applies defined settings from <paramref name="newConfig"/>.
    /// </summary>
    /// <param name="newConfig">Config containing the settings to apply.</param>
    public void Apply(AudioConfig newConfig)
    {
        this.CueName = newConfig.CueName ?? this.CueName;
        this.AcbName = newConfig.AcbName ?? this.AcbName;
        this.PlayerId = newConfig.PlayerId ?? this.PlayerId;
        this.CategoryIds = newConfig.CategoryIds ?? this.CategoryIds;
        this.NumChannels = newConfig.NumChannels ?? this.NumChannels;
        this.SampleRate = newConfig.SampleRate ?? this.SampleRate;
        this.Volume = newConfig.Volume ?? this.Volume;
        this.Tags = newConfig.Tags ?? this.Tags;
        this.Key = newConfig.Key ?? this.Key;
        this.SharedContainerId = newConfig.SharedContainerId ?? this.SharedContainerId;
        this.AudioDataName = newConfig.AudioDataName ?? this.AudioDataName;
        this.AudioFilePath = newConfig.AudioFilePath ?? this.AudioFilePath;
        this.PlaybackMode = newConfig.PlaybackMode ?? this.PlaybackMode;
    }
}
