using Ryo.Definitions.Enums;

namespace Ryo.Interfaces.Structs;

public struct AudioInfo
{
    public CriAtomFormat Format;
    public string AudioFile;
    public int SampleRate;
    public int NumChannels;
    public float Volume;
    public string[] Tags;
    public string? Key;
}
