using Ryo.Reloaded.CRI.Types;

namespace Ryo.Reloaded.Audio;

internal class AudioConfig
{
    public string CueName { get; set; } = string.Empty;

    public int SampleRate { get; set; } = 44100;

    public CRIATOM_FORMAT Format { get; set; } = CRIATOM_FORMAT.ADX;

    public int NumChannels { get; set; } = 2;

    public int PlayerId { get; set; }

    public int[] CategoryIds { get; set; } = Array.Empty<int>();

    public string AudioFile { get; set; } = string.Empty;
}