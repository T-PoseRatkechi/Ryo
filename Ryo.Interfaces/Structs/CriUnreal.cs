using System.Runtime.InteropServices;

namespace Ryo.Definitions.Structs;

[StructLayout(LayoutKind.Explicit, Size = 0x118)]
public unsafe struct USoundAtomCueSheet
{
    [FieldOffset(0x00d0)] public nint* _AcbHn;
    [FieldOffset(0x0040)] public FString CueSheetName;
}

[StructLayout(LayoutKind.Explicit, Size = 0x240)]
public unsafe struct USoundAtomCueSheetSMTV
{
    [FieldOffset(0x00B8)] public nint _acbHn;
    [FieldOffset(0x0080)] public FString CueSheetName;
}

[StructLayout(LayoutKind.Explicit, Size = 0xA98)]
public unsafe struct UPlayAdxControl
{
}

[StructLayout(LayoutKind.Explicit, Size = 0xC0)]
public unsafe struct USoundAtomCue
{
    [FieldOffset(0x0030)] public USoundAtomCueSheet* CueSheet;
    [FieldOffset(0x0038)] public FString CueName;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct LoadTaskParameter
{
    [FieldOffset(0x0018)] public nint Field11;
    [FieldOffset(0x0018)] public USoundAtomCueSheet* CueSheet;
};

public enum EPlayerType
{
    BGM = 0,
    VOICE = 1,
    SE = 2,
    EPlayerType_MAX = 3,
};