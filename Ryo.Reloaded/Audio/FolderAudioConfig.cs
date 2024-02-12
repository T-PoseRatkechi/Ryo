using Ryo.Interfaces.Types;

namespace Ryo.Reloaded.Audio;

/// <summary>
/// Base audio settings for folder audio files.
/// </summary>
internal class FolderAudioConfig
{
    public int? SampleRate { get; set; }

    public CriAtom_Format? Format { get; set; }

    public int? NumChannels { get; set; }

    public int? PlayerId { get; set; }

    public int[]? CategoryIds { get; set; }

    public float? Volume { get; set; }
}
