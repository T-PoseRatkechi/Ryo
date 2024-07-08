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

[StructLayout(LayoutKind.Explicit)]
public unsafe struct AcbHn
{
    [FieldOffset(0x10)]
    public AcbObj* acb;

    public readonly string GetAcbName()
    {
        var acbName = Marshal.PtrToStringAnsi(acb->name)!;
        return acbName;
    }
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct AcbObj
{
    [FieldOffset(0x98)]
    public nint name;
}
