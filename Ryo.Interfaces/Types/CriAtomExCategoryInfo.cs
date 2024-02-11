using System.Runtime.InteropServices;

namespace Ryo.Interfaces.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CriAtomExCategoryInfo
{
    public uint groupNo;
    public uint id;
    public byte* name;
    public uint numCueLimits;
    public float volume;
}
