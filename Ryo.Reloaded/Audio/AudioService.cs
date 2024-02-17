using Reloaded.Hooks.Definitions;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Ryo.Interfaces.Types;
using Ryo.Reloaded.CRI;
using Ryo.Reloaded.P3R;
using System.Runtime.InteropServices;
using static Ryo.Reloaded.CRI.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio;

internal unsafe class AudioService : IGameHook
{
    private readonly Dictionary<string, AudioData> cachedAudioData = new(StringComparer.OrdinalIgnoreCase);

    private readonly CriAtomEx criAtomEx;
    private readonly AudioRegistry audioRegistry;
    private IHook<criAtomExPlayer_SetCueName>? setCueNameHook;

    private readonly VolumeGetterAlt volume = new();
    private readonly Dictionary<nint, CategoryVolume> modifiedCategories = new();

    public AudioService(CriAtomEx criAtomEx, AudioRegistry audioRegistry)
    {
        this.criAtomEx = criAtomEx;
        this.audioRegistry = audioRegistry;

        criAtomEx.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(criAtomEx.SetCueName))
            {
                this.setCueNameHook = this.criAtomEx.SetCueName!.Hook(this.CriAtomExPlayer_SetCueName).Activate();
            }
        };
    }

    public void Initialize(IStartupScanner scanner, IReloadedHooks hooks)
    {
        this.volume.Initialize(scanner, hooks);
    }

    private void CriAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueName)
    {
        var player = this.criAtomEx.GetPlayerByHn(playerHn)!;
        var cueNameStr = Marshal.PtrToStringAnsi((nint)cueName);

        //for (int i = 0; i < 25; i++)
        //{
        //    Log.Information($"Category: {i} || Volume: {this.volume.CriAtomExCategory_GetVolumeById((uint)i)}");
        //}

        this.ResetPlayerVolume(player.PlayerHn);
        if (this.audioRegistry.TryGetAudio(player, cueNameStr ?? string.Empty, out var audio))
        {
            var manualStart = false;
            if (player.Id != audio.PlayerId && this.criAtomEx.GetPlayerById(audio.PlayerId) is PlayerConfig newPlayer)
            {
                player = newPlayer;
                manualStart = true;
            }

            var audioData = this.GetAudioData(audio.AudioFile);
            this.criAtomEx.Player_SetData(player.PlayerHn, (byte*)audioData.Buffer, audioData.Size);
            this.criAtomEx.Player_SetFormat(player.PlayerHn, audio.Format);
            this.criAtomEx.Player_SetSamplingRate(player.PlayerHn, audio.SampleRate);
            this.criAtomEx.Player_SetNumChannels(player.PlayerHn, audio.NumChannels);
            //this.criAtomEx.Player_LimitLoopCount(player.PlayerHn, -3);

            // Use first category for setting custom volume.
            int volumeCategory = audio.CategoryIds.Length > 0 ? audio.CategoryIds[0] : -1;
            if (volumeCategory > -1 && audio.Volume >= 0)
            {
                if (this.modifiedCategories.ContainsKey(player.PlayerHn))
                {
                    Log.Warning("Modifiying mulitple category volumes before reseting.");
                }

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

            // I wish this worked :(
            //this.criAtomEx.Player_SetVolume(player.PlayerHn, 0.0f);
            //this.criAtomEx.Player_UpdateAll(player.PlayerHn);

            if (manualStart)
            {
                this.criAtomEx.Player_Start(player.PlayerHn);
                Log.Debug($"Manually started player with ID: {player.Id}");
            }

            Log.Debug($"Redirected Cue Name: {cueNameStr}\nFile: {audio.AudioFile}");
        }
        else
        {
            this.setCueNameHook!.OriginalFunction(playerHn, acbHn, cueName);
        }
    }

    private AudioData GetAudioData(string audioFile)
    {
        if (this.cachedAudioData.TryGetValue(audioFile, out var existingData))
        {
            return existingData;
        }

        var data = File.ReadAllBytes(audioFile);
        var buffer = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, buffer, data.Length);
        this.cachedAudioData[audioFile] = new(buffer, data.Length);
        return this.cachedAudioData[audioFile];
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

    private record AudioData(nint Buffer, int Size);

    private record CategoryVolume(int PlayerId, int CategoryId, float OriginalVolume);
}
