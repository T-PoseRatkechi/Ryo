using Reloaded.Hooks.Definitions;
using Ryo.Interfaces.Types;
using Ryo.Reloaded.Audio.Models;
using Ryo.Reloaded.CRI.CriAtomEx;
using Ryo.Reloaded.P3R;
using System.Runtime.InteropServices;
using static Ryo.Reloaded.CRI.CriAtomEx.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio;

internal unsafe class AudioService
{
    private readonly CriAtomEx criAtomEx;
    private readonly AudioRegistry audioRegistry;
    private readonly AudioConfig defaultAudio;
    private readonly VolumeGetterAlt volume;
    private IHook<criAtomExPlayer_SetCueName>? setCueNameHook;

    private bool devMode;
    private readonly Dictionary<nint, CategoryVolume> modifiedCategories = new();

    public AudioService(CriAtomEx criAtomEx, AudioRegistry audioRegistry, AudioConfig defaultAudio)
    {
        this.criAtomEx = criAtomEx;
        this.audioRegistry = audioRegistry;
        this.defaultAudio = defaultAudio;
        this.volume = new();

        criAtomEx.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(criAtomEx.SetCueName))
            {
                this.setCueNameHook = this.criAtomEx.SetCueName!.Hook(this.CriAtomExPlayer_SetCueName).Activate();
            }
        };
    }

    public void SetDevMode(bool devMode)
        => this.devMode = devMode;

    private void CriAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueName)
    {
        var player = this.criAtomEx.GetPlayerByHn(playerHn)!;
        var cueNameStr = Marshal.PtrToStringAnsi((nint)cueName)!;
        var acbName = AcbRegistry.GetAcbName(acbHn) ?? this.defaultAudio.AcbName;

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

            var audioData = this.audioRegistry.GetAudioData(audio.AudioFile);
            this.criAtomEx.Player_SetData(player.PlayerHn, (byte*)audioData.Buffer, audioData.Size);
            this.criAtomEx.Player_SetFormat(player.PlayerHn, audio.Format);
            this.criAtomEx.Player_SetSamplingRate(player.PlayerHn, audio.SampleRate);
            this.criAtomEx.Player_SetNumChannels(player.PlayerHn, audio.NumChannels);

            // Use first category for setting custom volume.
            int volumeCategory = audio.CategoryIds.Length > 0 ? audio.CategoryIds[0] : -1;
            if (volumeCategory > -1 && audio.Volume >= 0 && !this.modifiedCategories.ContainsKey(player.PlayerHn))
            {
                var currentVolume = this.volume.CriAtomExCategory_GetVolumeById((uint)volumeCategory);
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

            Log.Debug($"Redirected Cue Name: {cueNameStr}\nFile: {audio.AudioFile}");
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
            this.setCueNameHook!.OriginalFunction(playerHn, acbHn, cueName);
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
