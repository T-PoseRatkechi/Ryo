using System.Runtime.InteropServices;

namespace Ryo.Definitions.Structs;

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public unsafe struct TArray<T> where T : unmanaged
{
    public T* AllocatorInstance;
    public int Num;
    public int Max;
}

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public unsafe struct FString
{
    public TArray<ushort> Data;

    public readonly string? GetString()
    {
        return Marshal.PtrToStringAuto((nint)Data.AllocatorInstance);
    }
}
