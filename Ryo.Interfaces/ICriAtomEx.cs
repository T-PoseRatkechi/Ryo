using Ryo.Definitions.Classes;
using Ryo.Definitions.Enums;
using Ryo.Definitions.Structs;

namespace Ryo.Interfaces;

public interface ICriAtomEx
{
    float Category_GetVolumeById(uint id);

    int Playback_GetTimeSyncedWithAudio(uint playbackId);

    unsafe bool Player_GetCategoryInfo(nint playerHn, ushort index, CriAtomExCategoryInfo* info);

    uint Player_GetLastPlaybackId(nint playerHn);

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

    unsafe nint Player_Create(CriAtomExPlayerConfigTag* config, void* work, int workSize);

    void SetPlayerConfigById(int id, CriAtomExPlayerConfigTag config);

    [Obsolete("No alternative until needed.")]
    PlayerConfig? GetPlayerByAcbPath(string acbPath);

    [Obsolete("Use ICriAtomRegistry.GetPlayerByHn")]
    PlayerConfig? GetPlayerByHn(nint playerHn);

    [Obsolete("Use ICriAtomRegistry.GetPlayerById")]
    PlayerConfig? GetPlayerById(int playerId);
}
