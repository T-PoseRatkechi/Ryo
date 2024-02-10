using System.Runtime.InteropServices;

namespace Ryo.Reloaded.CRI.Types;

[StructLayout(LayoutKind.Sequential)]
internal struct CriAtomExPlayerConfigTag
{
    public int voiceAllocationMethod;
    public int maxPathStrings;
    public int maxPath;
    public byte maxAisacs;
    public bool updatesTime;
    public bool enableAudioSyncedTimer;
}