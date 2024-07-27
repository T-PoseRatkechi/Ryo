using System.Runtime.InteropServices;

namespace Ryo.Definitions.Structs;

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

// Size is larger than it really is.
[StructLayout(LayoutKind.Sequential, Size = 0xFF)]
public unsafe struct CriAtomExCueInfoTag
{
    private const int MAX_NUM_CATEGORIES = 16;

    public int id;
    public int type;
    public nint name;
    public nint user_data;
    public long length;
    public fixed short categories[MAX_NUM_CATEGORIES];
    //public short num_limits;
    //public ushort num_blocks;
    //public ushort num_tracks;
    //public ushort num_related_waveforms;
    //public byte priority;
    //public byte header_visibility;
    //public byte ignore_player_parameter;
    //public byte probability;
    //public int pan_type;
    //public int pos3d_info;
    //public int game_variable_info;
    //public float volume;
    //public int silent_mode;

    public int[] GetCategories()
    {
        var cats = new List<int>();
        for (int i = 0; i < MAX_NUM_CATEGORIES; i++)
        {
            var cat = this.categories[i];
            if (cat == -1)
            {
                break;
            }

            cats.Add(cat);
        }

        return cats.ToArray();
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CriAtomExCategoryInfoTag
{
    public uint group_no;
    public uint id;
    public nint name;
    public uint num_cue_limits;
    public float volume;
}
