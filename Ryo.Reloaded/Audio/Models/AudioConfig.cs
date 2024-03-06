using Ryo.Definitions.Enums;

namespace Ryo.Reloaded.Audio.Models;

internal class AudioConfig
{
    public string CueName { get; set; } = string.Empty;

    public string AcbName { get; set; } = string.Empty;

    public int SampleRate { get; set; } = 44100;

    public CriAtomFormat Format { get; set; } = CriAtomFormat.HCA;

    public int NumChannels { get; set; } = 2;

    public int PlayerId { get; set; } = -1;

    public int[] CategoryIds { get; set; } = Array.Empty<int>();

    public string AudioFile { get; set; } = string.Empty;

    public float Volume { get; set; } = -1f;

    public string? Key { get; set; }

    public string[] Tags { get; set; } = Array.Empty<string>();
}
