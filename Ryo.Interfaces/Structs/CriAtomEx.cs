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

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CriAtomExCueInfoTag
{
    public int id;
    public int type;
    public nint name;
    public nint user_data;
    public long length;
    public ushort* categories;
    public short num_limits;
    public ushort num_blocks;
    public ushort num_tracks;
    public ushort num_related_waveforms;
    public byte priority;
    public byte header_visibility;
    public byte ignore_player_parameter;
    public byte probability;
    public int pan_type;
    public int pos3d_info;
    public int game_variable_info;
    public float volume;
    public int silent_mode;
}
