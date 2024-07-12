using Ryo.Definitions.Classes;
using Ryo.Reloaded.CRI.CriAtomEx;
using SharedScans.Interfaces;
using System.Runtime.InteropServices;
using static Ryo.Definitions.Functions.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio;

internal unsafe class AudioService
{
    private readonly CriAtomEx criAtomEx;
    private readonly AudioRegistry audioRegistry;

    private bool devMode;
    private readonly bool useSetFile;
    private readonly HookContainer<criAtomExPlayer_SetCueName> setCueName;
    private readonly HookContainer<criAtomExPlayer_SetCueId> setCueId;
    private readonly Dictionary<nint, CategoryVolume> modifiedCategories = new();

    public AudioService(
        string game,
        ISharedScans scans,
        CriAtomEx criAtomEx,
        AudioRegistry audioRegistry)
    {
        this.criAtomEx = criAtomEx;
        this.audioRegistry = audioRegistry;

        GameDefaults.ConfigureCriAtom(game, criAtomEx);
        var patterns = CriAtomExGames.GetGamePatterns(game);
        if (patterns.criAtomExPlayer_SetFile != null)
        {
            Log.Debug($"New audio uses: {nameof(criAtomExPlayer_SetFile)}");
            this.useSetFile = true;
        }
        else
        {
            Log.Debug($"New audio uses: {nameof(criAtomExPlayer_SetData)}");
        }

        this.setCueName = scans.CreateHook<criAtomExPlayer_SetCueName>(this.CriAtomExPlayer_SetCueName, Mod.NAME);
        this.setCueId = scans.CreateHook<criAtomExPlayer_SetCueId>(this.CriAtomExPlayer_SetCueId, Mod.NAME);
    }

    public void SetDevMode(bool devMode)
        => this.devMode = devMode;

    private bool SetRyoAudio(nint playerHn, nint acbHn, string cueName)
    {
        var player = this.criAtomEx.GetPlayerByHn(playerHn)!;
        var acbName = AcbRegistry.GetAcbName(acbHn) ?? "Unknown";

        if (this.devMode)
        {
            Log.Information($"SetCue || Player: {player.Id} || ACB: {acbName} || Cue: {cueName}");
        }

        if (this.audioRegistry.TryGetCue(cueName, acbName, out var cue))
        {
            var manualStart = false;
            if (cue.PlayerId != -1
                && player.Id != cue.PlayerId
                && this.criAtomEx.GetPlayerById(cue.PlayerId) is PlayerConfig newPlayer)
            {
                player = newPlayer;
                manualStart = true;
            }

            var newAudio = cue.GetAudio();

            if (this.useSetFile)
            {
                this.criAtomEx.Player_SetFile(player.PlayerHn, IntPtr.Zero, (byte*)StringsCache.GetStringPtr(newAudio.FilePath));
            }
            else
            {
                var audioData = AudioCache.GetAudioData(newAudio.FilePath);
                this.criAtomEx.Player_SetData(player.PlayerHn, (byte*)audioData.Buffer, audioData.Size);
            }

            this.criAtomEx.Player_SetFormat(player.PlayerHn, newAudio.Format);
            this.criAtomEx.Player_SetSamplingRate(player.PlayerHn, newAudio.SampleRate);
            this.criAtomEx.Player_SetNumChannels(player.PlayerHn, newAudio.NumChannels);

            // Use first category for setting custom volume.
            int volumeCategory = cue.CategoryIds.Length > 0 ? cue.CategoryIds[0] : -1;
            if (volumeCategory > -1 && newAudio.Volume >= 0 && !this.modifiedCategories.ContainsKey(player.PlayerHn))
            {
                var currentVolume = this.criAtomEx.Category_GetVolumeById((uint)volumeCategory);
                this.modifiedCategories[player.PlayerHn] = new CategoryVolume(player.Id, volumeCategory, currentVolume);
                this.criAtomEx.Category_SetVolumeById((uint)volumeCategory, newAudio.Volume);
                Log.Debug($"Modified volume. Player ID: {player.Id} || Category ID: {volumeCategory} || Volume: {newAudio.Volume}");
            }

            // Apply categories.
            foreach (var id in cue.CategoryIds)
            {
                this.criAtomEx.Player_SetCategoryById(player.PlayerHn, (uint)id);
            }

            if (manualStart)
            {
                this.criAtomEx.Player_Start(player.PlayerHn);
                Log.Debug($"Manually started player with ID: {player.Id}");
            }

            Log.Debug($"Redirected Cue: {cueName} / {acbName}\nFile: {newAudio.FilePath}");
            return true;
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
            return false;
        }
    }

    private void CriAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueName)
    {
        var cueNameStr = Marshal.PtrToStringAnsi((nint)cueName)!;

        if (this.SetRyoAudio(playerHn, acbHn, cueNameStr) == false)
        {
            this.setCueName.Hook!.OriginalFunction(playerHn, acbHn, cueName);
        }
    }

    private void CriAtomExPlayer_SetCueId(nint playerHn, nint acbHn, int cueId)
    {
        if (this.SetRyoAudio(playerHn, acbHn, cueId.ToString()) == false)
        {
            this.setCueId.Hook!.OriginalFunction(playerHn, acbHn, cueId);
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
