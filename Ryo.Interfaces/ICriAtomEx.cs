using Ryo.Definitions.Classes;
using Ryo.Definitions.Enums;
using Ryo.Definitions.Structs;

namespace Ryo.Interfaces;

public interface ICriAtomEx
{
    unsafe bool Acb_GetCueInfoById(nint acbHn, int id, CriAtomExCueInfoTag* info);
    unsafe bool Acb_GetCueInfoByName(nint acbHn, nint name, CriAtomExCueInfoTag* info);
    unsafe nint Acb_LoadAcbData(nint acbData, int acbDataSize, nint awbBinder, nint awbPath, void* work, int workSize);
    unsafe bool Acf_GetCategoryInfoById(ushort id, CriAtomExCategoryInfoTag* info);
    float Category_GetVolume(ushort index);
    float Category_GetVolumeById(uint id);
    void Category_SetVolumeById(uint id, float volume);
    ushort Config_GetCategoryIndexById(uint id);
    int Playback_GetTimeSyncedWithAudio(uint playbackId);
    unsafe nint Player_Create(CriAtomExPlayerConfigTag* config, void* work, int workSize);
    unsafe bool Player_GetCategoryInfo(nint playerHn, ushort index, CriAtomExCategoryInfoTag* info);
    uint Player_GetLastPlaybackId(nint playerHn);
    CriAtomExPlayerStatusTag Player_GetStatus(nint playerHn);
    void Player_LimitLoopCount(nint playerHn, int count);
    unsafe void Player_SetAisacControlByName(nint playerHn, byte* controlName, float controlValue);
    void Player_SetCategoryById(nint playerHn, uint id);
    unsafe void Player_SetCategoryByName(nint playerHn, byte* name);
    void Player_SetCueId(nint playerHn, nint acbHn, int cueId);
    unsafe void Player_SetCueName(nint playerHn, nint acbHn, byte* cueName);
    unsafe void Player_SetData(nint playerHn, byte* buffer, int size);
    unsafe void Player_SetFile(nint playerHn, nint criBinderHn, byte* path);
    void Player_SetFormat(nint playerHn, CriAtomFormat format);
    void Player_SetNumChannels(nint playerHn, int numChannels);
    void Player_SetSamplingRate(nint playerHn, int samplingRate);
    void Player_SetStartTime(nint playerHn, int currentBgmTime);
    void Player_SetVolume(nint playerHn, float volume);
    uint Player_Start(nint playerHn);
    void Player_Stop(nint playerHn);
    void Player_UpdateAll(nint playerHn);
    void SetPlayerConfigById(int id, CriAtomExPlayerConfigTag config);
    int Playback_GetTimeSyncedWithAudioMicro(uint playbackId);

    [Obsolete("No alternative until needed.")]
    PlayerConfig? GetPlayerByAcbPath(string acbPath);

    [Obsolete("Use ICriAtomRegistry.GetPlayerByHn")]
    PlayerConfig? GetPlayerByHn(nint playerHn);

    [Obsolete("Use ICriAtomRegistry.GetPlayerById")]
    PlayerConfig? GetPlayerById(int playerId);
}
