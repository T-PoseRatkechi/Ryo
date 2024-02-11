using Reloaded.Hooks.Definitions.X64;
using Ryo.Interfaces.Types;

namespace Ryo.Reloaded.CRI;

internal unsafe static class CriAtomExFunctions
{
    [Function(CallingConventions.Microsoft)]
    public delegate CriBool criAtomExPlayer_GetNumPlayedSamples(uint playbackId, ulong* numSamples, uint* samplingRate);

    [Function(CallingConventions.Microsoft)]
    public delegate nint criAtomExAcb_LoadAcbFile(nint acbBinder, byte* acbPathStr, nint awbBinder, byte* awbPathStr, void* work, int workSize);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCueId(nint playerHn, nint acbHn, int cueId);

    [Function(CallingConventions.Microsoft)]
    public delegate uint criAtomExPlayer_Start(nint playerHn);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetFile(nint playerHn, nint criBinderHn, byte* path);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetFormat(nint playerHn, CriAtom_Format format);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetSamplingRate(nint playerHn, int samplingRate);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetNumChannels(nint playerHn, int numChannels);

    [Function(CallingConventions.Microsoft)]
    public delegate float criAtomExCategory_GetVolumeById(uint categoryId);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetVolume(nint playerHn, float volume);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCategoryById(nint playerHn, uint id);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetStartTime(nint playerHn, int startTimeMs);

    [Function(CallingConventions.Microsoft)]
    public delegate int criAtomExPlayback_GetTimeSyncedWithAudio(uint playbackId);

    [Function(CallingConventions.Microsoft)]
    public delegate nint criAtomExPlayer_Create(CriAtomExPlayerConfigTag* config, void* work, int workSize);

    [Function(CallingConventions.Microsoft)]
    public delegate uint criAtomExPlayer_GetLastPlaybackId(nint playerHn);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCategoryByName(nint playerHn, byte* name);

    [Function(CallingConventions.Microsoft)]
    public delegate bool criAtomExPlayer_GetCategoryInfo(nint playerHn, ushort index, CriAtomExCategoryInfo* info);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetData(nint playerHn, byte* buffer, int size);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueName);
}
