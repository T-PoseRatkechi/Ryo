using Ryo.Definitions.Structs;
using Ryo.Interfaces;
using Ryo.Reloaded.Audio.Models.Containers;
using Ryo.Reloaded.CRI.CriAtomEx;
using SharedScans.Interfaces;
using System.Runtime.InteropServices;
using static Ryo.Definitions.Functions.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio;

internal unsafe class AudioService
{
    private readonly CriAtomEx criAtomEx;
    private readonly CriAtomRegistry criAtomRegistry;
    private readonly AudioRegistry audioRegistry;

    private bool devMode;
    private readonly bool useSetFile;
    private readonly HookContainer<criAtomExPlayer_SetCueName> setCueName;
    private readonly HookContainer<criAtomExPlayer_SetCueId> setCueId;
    private readonly HookContainer<criAtomExPlayer_SetFile> setFile;
    private readonly HookContainer<criAtomExPlayer_SetWaveId> setWaveId;
    private readonly HookContainer<criAtomExAcb_GetCueInfoById> getCueInfoById;
    private readonly HookContainer<criAtomExAcb_GetCueInfoByName> getCueInfoByName;
    private readonly HookContainer<criAtomExPlayer_SetData> setData;
    private readonly Dictionary<nint, CategoryVolume> modifiedCategories = new();

    public AudioService(
        string game,
        ISharedScans scans,
        CriAtomEx criAtomEx,
        CriAtomRegistry criAtomRegistry,
        AudioRegistry audioRegistry)
    {
        this.criAtomEx = criAtomEx;
        this.criAtomRegistry = criAtomRegistry;
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
        this.setFile = scans.CreateHook<criAtomExPlayer_SetFile>(this.CriAtomExPlayer_SetFile, Mod.NAME);
        this.setData = scans.CreateHook<criAtomExPlayer_SetData>(this.CriAtomExPlayer_SetData, Mod.NAME);
        this.setWaveId = scans.CreateHook<criAtomExPlayer_SetWaveId>(this.CriAtomExPlayer_SetWaveId, Mod.NAME);

        this.getCueInfoById = scans.CreateHook<criAtomExAcb_GetCueInfoById>(this.CriAtomExAcb_GetCueInfoById, Mod.NAME);
        this.getCueInfoByName = scans.CreateHook<criAtomExAcb_GetCueInfoByName>(this.CriAtomExAcb_GetCueInfoByName, Mod.NAME);
    }

    public void SetDevMode(bool devMode)
        => this.devMode = devMode;

    private void SetRyoAudio(Player player, BaseContainer container)
    {
        var currentPlayer = player;
        
        var manualStart = false;
        if (container.PlayerId != -1
            && currentPlayer.Id != container.PlayerId
            && this.criAtomRegistry.GetPlayerById(container.PlayerId) is Player newPlayer)
        {
            currentPlayer = newPlayer;
            manualStart = true;
        }

        var newAudio = container.GetAudio();

        if (this.useSetFile)
        {
            this.criAtomEx.Player_SetFile(currentPlayer.Handle, IntPtr.Zero, (byte*)StringsCache.GetStringPtr(newAudio.FilePath));
        }
        else
        {
            var audioData = AudioCache.GetAudioData(newAudio.FilePath);
            this.criAtomEx.Player_SetData(currentPlayer.Handle, (byte*)audioData.Address, audioData.Size);
        }

        this.criAtomEx.Player_SetFormat(currentPlayer.Handle, newAudio.Format);
        this.criAtomEx.Player_SetSamplingRate(currentPlayer.Handle, newAudio.SampleRate);
        this.criAtomEx.Player_SetNumChannels(currentPlayer.Handle, newAudio.NumChannels);

        // Use first category for setting custom volume.
        int volumeCategory = container.CategoryIds.Length > 0 ? container.CategoryIds[0] : -1;
        if (volumeCategory > -1 && newAudio.Volume >= 0 && !this.modifiedCategories.ContainsKey(currentPlayer.Handle))
        {
            var currentVolume = this.criAtomEx.Category_GetVolumeById((uint)volumeCategory);
            this.modifiedCategories[currentPlayer.Handle] = new CategoryVolume(currentPlayer.Id, volumeCategory, currentVolume);
            this.criAtomEx.Category_SetVolumeById((uint)volumeCategory, newAudio.Volume);
            Log.Debug($"Modified volume. Player ID: {currentPlayer.Id} || Category ID: {volumeCategory} || Volume: {newAudio.Volume}");
        }

        // Apply categories.
        foreach (var id in container.CategoryIds)
        {
            this.criAtomEx.Player_SetCategoryById(currentPlayer.Handle, (uint)id);
        }

        if (manualStart)
        {
            this.criAtomEx.Player_Start(currentPlayer.Handle);
            Log.Debug($"Manually started player with ID: {currentPlayer.Id}");
        }

        Log.Debug($"Redirected {container.Name}\nFile: {newAudio.FilePath}");
    }

    private bool SetCue(nint playerHn, nint acbHn, string cueName)
    {
        var player = this.criAtomRegistry.GetPlayerByHn(playerHn)!;
        var acbName = this.criAtomRegistry.GetAcbByHn(acbHn)?.Name ?? "Unknown";

        if (this.devMode)
        {
            Log.Information($"{nameof(SetCue)} || Player: {player.Id} || ACB: {acbName} || Cue: {cueName}");
        }

        if (this.audioRegistry.TryGetCueContainer(cueName, acbName, out var cue))
        {
            this.SetRyoAudio(player, cue);
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

        if (this.SetCue(playerHn, acbHn, cueNameStr) == false)
        {
            this.setCueName.Hook!.OriginalFunction(playerHn, acbHn, cueName);
        }
    }

    private void CriAtomExPlayer_SetCueId(nint playerHn, nint acbHn, int cueId)
    {
        if (this.SetCue(playerHn, acbHn, cueId.ToString()) == false)
        {
            this.setCueId.Hook!.OriginalFunction(playerHn, acbHn, cueId);
        }
    }

    private void CriAtomExPlayer_SetFile(nint playerHn, nint criBinderHn, byte* path)
    {
        var filePath = Marshal.PtrToStringAnsi((nint)path);
        var player = this.criAtomRegistry.GetPlayerByHn(playerHn)!;

        if (this.devMode)
        {
            Log.Information($"{nameof(criAtomExPlayer_SetFile)} || Player: {player.Id} || {filePath}");
        }

        if (filePath != null && this.audioRegistry.TryGetFileContainer(filePath, out var file))
        {
            this.SetRyoAudio(player, file);
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
            this.setFile.Hook!.OriginalFunction(playerHn, criBinderHn, path);
        }
    }

    private void CriAtomExPlayer_SetData(nint playerHn, byte* buffer, int size)
    {
        var player = this.criAtomRegistry.GetPlayerByHn(playerHn)!;
        var audioData = this.criAtomRegistry.GetAudioDataByAddress((nint)buffer);

        if (this.devMode)
        {
            Log.Information($"{nameof(criAtomExPlayer_SetData)} || Player: {player.Id} || Buffer: {(nint)buffer:X}");
        }

        if (audioData != null && this.audioRegistry.TryGetDataContainer(audioData.Name, out var data))
        {
            this.SetRyoAudio(player, data);
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
            this.setData.Hook!.OriginalFunction(playerHn, buffer, size);
        }
    }

    private void CriAtomExPlayer_SetWaveId(nint playerHn, nint awbHn, int waveId)
    {
        if (this.SetCue(playerHn, awbHn, waveId.ToString()) == false)
        {
            this.setWaveId.Hook!.OriginalFunction(playerHn, awbHn, waveId);
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

    private bool GetCueInfo(string acb, string cue, CriAtomExCueInfoTag* info)
    {
        // Ryo has audio registered for a "new" cue.
        if (this.audioRegistry.TryGetCueContainer(cue, acb, out _))
        {
            // Hopefully null/zero values doesn't crash...
            // Might be better to use GetCueInfoByIndex with index 0 and use that
            // cue info as a shell.
            _ = int.TryParse(cue, out int id);
            var fakeCue = new CriAtomExCueInfoTag
            {
                id = id,
                name = StringsCache.GetStringPtr(cue),
            };

            *info = fakeCue;
            Log.Debug($"{nameof(GetCueInfo)} || Faked Cue: {cue} / {acb}");
            return true;
        }

        return false;
    }

    private bool CriAtomExAcb_GetCueInfoByName(nint acbHn, nint nameStr, CriAtomExCueInfoTag* info)
    {
        // Cue by name exists in original ACB.
        var result = this.getCueInfoByName.Hook!.OriginalFunction(acbHn, nameStr, info);
        if (result)
        {
            return true;
        }

        return this.GetCueInfo(this.criAtomRegistry.GetAcbByHn(acbHn)?.Name ?? string.Empty, Marshal.PtrToStringAnsi(nameStr)!, info);
    }

    private bool CriAtomExAcb_GetCueInfoById(nint acbHn, int id, CriAtomExCueInfoTag* info)
    {
        // Cue by ID exists in original ACB.
        var result = this.getCueInfoById.Hook!.OriginalFunction(acbHn, id, info);
        if (result)
        {
            return true;
        }

        return this.GetCueInfo(this.criAtomRegistry.GetAcbByHn(acbHn)?.Name ?? string.Empty, id.ToString(), info);
    }

    private record CategoryVolume(int PlayerId, int CategoryId, float OriginalVolume);
}
