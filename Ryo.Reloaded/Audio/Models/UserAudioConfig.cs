using Ryo.Interfaces.Types;

namespace Ryo.Reloaded.Audio.Models;

/// <summary>
/// User defined audio settings.
/// </summary>
internal class UserAudioConfig
{
    public string? CueName { get; set; }

    public string? AcbName { get; set; }

    public int? SampleRate { get; set; }

    public CriAtom_Format? Format { get; set; }

    public int? NumChannels { get; set; }

    public int? PlayerId { get; set; }

    public int[]? CategoryIds { get; set; }

    public float? Volume { get; set; }
}
