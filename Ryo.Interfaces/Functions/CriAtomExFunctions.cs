using Reloaded.Hooks.Definitions.X64;
using Ryo.Definitions.Enums;
using Ryo.Definitions.Structs;

namespace Ryo.Definitions.Functions;

public unsafe static class CriAtomExFunctions
{
    [Function(CallingConventions.Microsoft)]
    public delegate CriBool criAtomExPlayer_GetNumPlayedSamples(uint playbackId, ulong* numSamples, uint* samplingRate);

    [Function(CallingConventions.Microsoft)]
    public delegate nint criAtomExAcb_LoadAcbFile(nint acbBinder, byte* acbPathStr, nint awbBinder, byte* awbPathStr, void* work, int workSize);

    [Function(CallingConventions.Microsoft)]
    public delegate nint criAtomExAcb_LoadAcbData(nint acbData, int acbDataSize, nint awbBinder, nint awbPath, void* work, int workSize);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCueId(nint playerHn, nint acbHn, int cueId);

    [Function(CallingConventions.Microsoft)]
    public delegate uint criAtomExPlayer_Start(nint playerHn);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetFile(nint playerHn, nint criBinderHn, byte* path);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetWaveId(nint playerHn, nint awbHn, int waveId);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetFormat(nint playerHn, CriAtomFormat format);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetSamplingRate(nint playerHn, int samplingRate);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetNumChannels(nint playerHn, int numChannels);

    [Function(CallingConventions.Microsoft)]
    public delegate float criAtomExCategory_GetVolumeById(uint categoryId);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetVolume(nint playerHn, float volume);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExCategory_SetVolumeById(uint id, float volume);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExCategory_SetVolume(uint index, float volume);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCategoryById(nint playerHn, uint id);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetStartTime(nint playerHn, int startTimeMs);

    [Function(CallingConventions.Microsoft)]
    public delegate int criAtomExPlayback_GetTimeSyncedWithAudio(uint playbackId);

    [Function(CallingConventions.Microsoft)]
    public delegate int criAtomExPlayback_GetTimeSyncedWithAudioMicro(uint playbackId);

    [Function(CallingConventions.Microsoft)]
    public delegate nint criAtomExPlayer_Create(CriAtomExPlayerConfigTag* config, void* work, int workSize);

    [Function(CallingConventions.Microsoft)]
    public delegate uint criAtomExPlayer_GetLastPlaybackId(nint playerHn);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCategoryByName(nint playerHn, byte* name);

    [Function(CallingConventions.Microsoft)]
    public delegate bool criAtomExPlayer_GetCategoryInfo(nint playerHn, ushort index, CriAtomExCategoryInfoTag* info);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetData(nint playerHn, byte* buffer, int size);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueName);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_UpdateAll(nint playerHn);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_LimitLoopCount(nint playerHn, int count);

    [Function(CallingConventions.Microsoft)]
    public delegate CriAtomExPlayerStatusTag criAtomExPlayer_GetStatus(nint playerHn);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_Stop(nint playerHn);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetAisacControlByName(nint playerHn, byte* controlName, float controlValue);

    [Function(CallingConventions.Microsoft)]
    public delegate ushort criAtomConfig_GetCategoryIndexById(uint id);

    [Function(CallingConventions.Microsoft)]
    public delegate float criAtomExCategory_GetVolume(ushort index);

    [Function(CallingConventions.Microsoft)]
    public delegate bool criAtomExAcb_GetCueInfoByName(nint acbHn, nint nameStr, CriAtomExCueInfoTag* info);

    [Function(CallingConventions.Microsoft)]
    public delegate bool criAtomExAcb_GetCueInfoById(nint acbHn, int id, CriAtomExCueInfoTag* info);

    [Function(CallingConventions.Microsoft)]
    public delegate bool criAtomExAcb_ExistsName(nint acbHn, nint nameStr);

    [Function(CallingConventions.Microsoft)]
    public delegate nint criAtomAwb_LoadToc(nint binder, nint path, void* work, int workSize);

    [Function(CallingConventions.Microsoft)]
    public delegate bool criAtomExAcf_GetCategoryInfoByIndex(ushort index, CriAtomExCategoryInfoTag* info);

    [Function(CallingConventions.Microsoft)]
    public delegate void criAtomExPlayer_SetSyncPlaybackId(nint playerHn, uint playbackId);
}
