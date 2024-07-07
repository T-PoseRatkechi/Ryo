using Ryo.Definitions.Classes;
using Ryo.Reloaded.Audio.Models;
using Ryo.Reloaded.CRI.CriAtomEx;
using SharedScans.Interfaces;
using System.Runtime.InteropServices;
using static Ryo.Definitions.Functions.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio;

internal unsafe class AudioService
{
    private readonly CriAtomEx criAtomEx;
    private readonly AudioRegistry audioRegistry;

    private readonly HookContainer<criAtomExPlayer_SetCueName> setCueName;
    private readonly Dictionary<nint, CategoryVolume> modifiedCategories = new();
    private bool devMode;

    public AudioService(
        ISharedScans scans,
        CriAtomEx criAtomEx,
        AudioRegistry audioRegistry)
    {
        this.criAtomEx = criAtomEx;
        this.audioRegistry = audioRegistry;

        this.setCueName = scans.CreateHook<criAtomExPlayer_SetCueName>(this.CriAtomExPlayer_SetCueName, Mod.NAME);
    }

    public void SetDevMode(bool devMode)
        => this.devMode = devMode;

    private void CriAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueName)
    {
        var player = this.criAtomEx.GetPlayerByHn(playerHn)!;
        var cueNameStr = Marshal.PtrToStringAnsi((nint)cueName)!;
        var acbName = AcbRegistry.GetAcbName(acbHn) ?? "Unknown ACB";

        if (this.devMode)
        {
            Log.Information($"{nameof(CriAtomExPlayer_SetCueName)} || Cue: {cueNameStr} || ACB: {acbName}");
        }

        if (this.audioRegistry.TryGetAudio(cueNameStr, acbName, out var audio))
        {
            var manualStart = false;
            if (audio.PlayerId != -1
                && player.Id != audio.PlayerId
                && this.criAtomEx.GetPlayerById(audio.PlayerId) is PlayerConfig newPlayer)
            {
                player = newPlayer;
                manualStart = true;
            }

            var audioFile = audio.GetAudioFile();
            var audioData = AudioCache.GetAudioData(audioFile);
            this.criAtomEx.Player_SetData(player.PlayerHn, (byte*)audioData.Buffer, audioData.Size);
            this.criAtomEx.Player_SetFormat(player.PlayerHn, audio.Format);
            this.criAtomEx.Player_SetSamplingRate(player.PlayerHn, audio.SampleRate);
            this.criAtomEx.Player_SetNumChannels(player.PlayerHn, audio.NumChannels);

            // Use first category for setting custom volume.
            int volumeCategory = audio.CategoryIds.Length > 0 ? audio.CategoryIds[0] : -1;
            if (volumeCategory > -1 && audio.Volume >= 0 && !this.modifiedCategories.ContainsKey(player.PlayerHn))
            {
                var currentVolume = this.criAtomEx.Category_GetVolumeById((uint)volumeCategory);
                this.modifiedCategories[player.PlayerHn] = new CategoryVolume(player.Id, volumeCategory, currentVolume);
                this.criAtomEx.Category_SetVolumeById((uint)volumeCategory, audio.Volume);
                Log.Debug($"Modified volume. Player ID: {player.Id} || Category ID: {volumeCategory} || Volume: {audio.Volume}");
            }

            // Apply categories.
            foreach (var id in audio.CategoryIds)
            {
                this.criAtomEx.Player_SetCategoryById(player.PlayerHn, (uint)id);
            }

            if (manualStart)
            {
                this.criAtomEx.Player_Start(player.PlayerHn);
                Log.Debug($"Manually started player with ID: {player.Id}");
            }

            Log.Debug($"Redirected Cue Name: {cueNameStr}\nFile: {audioFile}");
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
            this.setCueName.Hook!.OriginalFunction(playerHn, acbHn, cueName);
        }
    }

    private void ResetPlayerVolume(nint playerHn)
    {
        // Reset modified category volume.
        // Limited to one modified category per player.
        if (this.modifiedCategories.TryGetValue(playerHn, out var modifiedVolume))
        {
            Log.Debug($"Reseting volume. Player ID: {modifiedVolume.PlayerId} || Category ID: {modifiedVolume.CategoryId}");
            this.criAtomEx.Category_SetVolumeById((uint)modifiedVolume.CategoryId, modifiedVolume.OriginalVolume);
            this.modifiedCategories.Remove(playerHn);
        }
    }

    private record CategoryVolume(int PlayerId, int CategoryId, float OriginalVolume);
}
