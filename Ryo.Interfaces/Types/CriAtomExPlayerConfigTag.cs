using System.Runtime.InteropServices;

namespace Ryo.Interfaces.Types;

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