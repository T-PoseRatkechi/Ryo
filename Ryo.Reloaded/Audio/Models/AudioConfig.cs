using Ryo.Interfaces.Types;

namespace Ryo.Reloaded.Audio.Models;

internal class AudioConfig
{
    public string CueName { get; set; } = string.Empty;

    public string AcbName { get; set; } = string.Empty;

    public int SampleRate { get; set; } = 44100;

    public CriAtom_Format Format { get; set; } = CriAtom_Format.HCA;

    public int NumChannels { get; set; } = 2;

    public int PlayerId { get; set; }

    public int[] CategoryIds { get; set; } = Array.Empty<int>();

    public string AudioFile { get; set; } = string.Empty;

    public float Volume { get; set; } = -1f;

    public bool ForceStop { get; set; }
}
