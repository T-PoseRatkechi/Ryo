using Reloaded.Hooks.Definitions;
using Ryo.Interfaces.Types;
using Ryo.Reloaded.CRI;
using System.Runtime.InteropServices;
using static Ryo.Reloaded.CRI.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio;

internal unsafe class AudioService
{
    private readonly Dictionary<string, AudioData> cachedAudioData = new(StringComparer.OrdinalIgnoreCase);

    private readonly CriAtomEx criAtomEx;
    private readonly AudioRegistry audioRegistry;
    private IHook<criAtomExPlayer_SetCueName>? setCueNameHook;

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

    private void CriAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueName)
    {
        var player = this.criAtomEx.GetPlayerByHn(playerHn)!;
        var cueNameStr = Marshal.PtrToStringAnsi((nint)cueName);
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

    private record AudioData(nint Buffer, int Size);
}
