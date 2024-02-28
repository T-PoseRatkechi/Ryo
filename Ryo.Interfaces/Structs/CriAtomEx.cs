using System.Runtime.InteropServices;

namespace Ryo.Definitions.Structs;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CriAtomExCategoryInfo
{
    public uint groupNo;
    public uint id;
    public byte* name;
    public uint numCueLimits;
    public float volume;
}

[StructLayout(LayoutKind.Sequential)]
public struct CriAtomExPlayerConfigTag
{
    public int voiceAllocationMethod;
    public int maxPathStrings;
    public int maxPath;
    public byte maxAisacs;
    public bool updatesTime;
    public bool enableAudioSyncedTimer;
}