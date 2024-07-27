using Ryo.Definitions.Structs;
using Ryo.Interfaces;
using Ryo.Reloaded.Audio.Models.Containers;
using Ryo.Reloaded.Audio.Services;
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
    private readonly VirtualAcfService virtualAcf;

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
        this.virtualAcf = new(scans);

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

    private void SetRyoAudio(Player player, BaseContainer container, int[]? categories)
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
        if (categories != null)
        {
            Log.Debug($"Categories: {string.Join(", ", categories.Select(x => $"{x}"))}");
            int volumeCategory = categories.Length > 0 ? categories[0] : -1;
            if (volumeCategory > -1 && newAudio.Volume >= 0 && !this.modifiedCategories.ContainsKey(currentPlayer.Handle))
            {
                var currentVolume = this.criAtomEx.Category_GetVolumeById((uint)volumeCategory);
                this.modifiedCategories[currentPlayer.Handle] = new CategoryVolume(currentPlayer.Id, volumeCategory, currentVolume);
                this.criAtomEx.Category_SetVolumeById((uint)volumeCategory, newAudio.Volume);
                Log.Debug($"Modified volume. Player ID: {currentPlayer.Id} || Category ID: {volumeCategory} || Volume: {newAudio.Volume}");
            }

            // Apply categories.
            foreach (var id in categories)
            {
                this.criAtomEx.Player_SetCategoryById(currentPlayer.Handle, (uint)id);
            }
        }

        if (manualStart)
        {
            this.criAtomEx.Player_Start(currentPlayer.Handle);
            Log.Debug($"Manually started player with ID: {currentPlayer.Id}");
        }

        Log.Debug($"Redirected {container.Name}\nFile: {newAudio.FilePath}");
    }

    private bool SetCue(nint playerHn, nint acbHn, Cue cue)
    {
        var player = this.criAtomRegistry.GetPlayerByHn(playerHn)!;
        var acbName = this.criAtomRegistry.GetAcbByHn(acbHn)?.Name ?? "Unknown";

        if (this.devMode)
        {
            var cueIdString = cue.Id == -1 ? "(N/A)" : cue.Id.ToString();
            var cueNameString = cue.Name ?? "(N/A)";

            Log.Information($"{nameof(SetCue)} || Player: {player.Id} || ACB: {acbName} || Cue: {cueIdString} / {cueNameString}");
        }

        if (cue.Id != -1 && this.audioRegistry.TryGetCueContainer(cue.Id.ToString(), acbName, out var idContainer))
        {
            this.SetRyoAudio(player, idContainer, idContainer.CategoryIds ?? cue.Categories);
            return true;
        }
        else if (cue.Name != null && this.audioRegistry.TryGetCueContainer(cue.Name, acbName, out var nameContainer))
        {
            this.SetRyoAudio(player, nameContainer, nameContainer.CategoryIds ?? cue.Categories);
            return true;
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
            return false;
        }
    }

    private void CriAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueNameStr)
    {
        var cueInfo = (CriAtomExCueInfoTag*)Marshal.AllocHGlobal(sizeof(CriAtomExCueInfoTag));

        var cueName = Marshal.PtrToStringAnsi((nint)cueNameStr)!;
        var cueId = -1;
        int[]? categories = null;
        
        if (this.CriAtomExAcb_GetCueInfoByName(acbHn, (nint)cueNameStr, cueInfo))
        {
            cueId = cueInfo->id;
            categories = cueInfo->GetCategories();
        }

        var cue = new Cue() { Name = cueName, Id = cueId, Categories = categories };
        if (this.SetCue(playerHn, acbHn, cue) == false)
        {
            this.setCueName.Hook!.OriginalFunction(playerHn, acbHn, cueNameStr);
        }

        Marshal.FreeHGlobal((nint)cueInfo);
    }

    private void CriAtomExPlayer_SetCueId(nint playerHn, nint acbHn, int cueId)
    {
        var cueInfo = (CriAtomExCueInfoTag*)Marshal.AllocHGlobal(sizeof(CriAtomExCueInfoTag));
        string? cueName = null;
        int[]? categories = null;

        if (this.CriAtomExAcb_GetCueInfoById(acbHn, cueId, cueInfo))
        {
            cueName = Marshal.PtrToStringAnsi(cueInfo->name)!;
            categories = cueInfo->GetCategories();
        }

        var cue = new Cue() { Name = cueName, Id = cueId, Categories = categories };
        if (this.SetCue(playerHn, acbHn, cue) == false)
        {
            this.setCueId.Hook!.OriginalFunction(playerHn, acbHn, cueId);
        }

        Marshal.FreeHGlobal((nint)cueInfo);
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
            this.SetRyoAudio(player, file, file.CategoryIds);
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
            this.SetRyoAudio(player, data, data.CategoryIds);
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
            this.setData.Hook!.OriginalFunction(playerHn, buffer, size);
        }
    }

    private void CriAtomExPlayer_SetWaveId(nint playerHn, nint awbHn, int waveId)
    {
        var awbPath = this.criAtomRegistry.GetAwbByHn(awbHn)?.Path ?? "Unknown";
        var player = this.criAtomRegistry.GetPlayerByHn(playerHn)!;

        if (this.devMode)
        {
            Log.Information($"{nameof(criAtomExPlayer_SetWaveId)} || Player: {player.Id} || AWB: {awbPath} || Wave ID: {waveId}");
        }

        if (awbPath != null && this.audioRegistry.TryGetFileContainer($"{awbPath.Trim('/')}/{waveId}.wave", out var file))
        {
            this.SetRyoAudio(player, file, file.CategoryIds);
        }
        else
        {
            this.ResetPlayerVolume(playerHn);
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

    private bool GetCueInfoByRyo(string acb, string cue, CriAtomExCueInfoTag* info)
    {
        // Ryo has audio registered for a "new" cue.
        if (this.audioRegistry.TryGetCueContainer(cue, acb, out _))
        {
            // Might be better to use GetCueInfoByIndex with index 0 and use that
            // cue info as a shell.
            _ = int.TryParse(cue, out int id);
            info->id = id;
            info->name = StringsCache.GetStringPtr(cue);
            Log.Debug($"{nameof(GetCueInfoByRyo)} || Faked Cue: {cue} / {acb}");
            return true;
        }

        return false;
    }

    private bool CriAtomExAcb_GetCueInfoByName(nint acbHn, nint nameStr, CriAtomExCueInfoTag* info)
    {
        // Cue by name exists in original ACB.
        if (this.getCueInfoByName.Hook?.OriginalFunction(acbHn, nameStr, info) == true)
        {
            PrintCategories(info);
            return true;
        }

        var acb = this.criAtomRegistry.GetAcbByHn(acbHn);
        if (acb != null)
        {
            return this.GetCueInfoByRyo(acb.Name, Marshal.PtrToStringAnsi(nameStr)!, info);
        }

        return false;
    }

    private bool CriAtomExAcb_GetCueInfoById(nint acbHn, int id, CriAtomExCueInfoTag* info)
    {
        // Cue by ID exists in original ACB.
        if (this.getCueInfoById.Hook?.OriginalFunction(acbHn, id, info) == true)
        {
            PrintCategories(info);
            return true;
        }

        var acb = this.criAtomRegistry.GetAcbByHn(acbHn);
        if (acb != null)
        {
            return this.GetCueInfoByRyo(acb.Name, id.ToString(), info);
        }

        return false;
    }

    private static void PrintCategories(CriAtomExCueInfoTag* info)
        => Log.Debug($"Categories: {string.Join(", ", info->GetCategories().Select(x => x.ToString()))}");

    private class Cue
    {
        public int Id { get; init; } = -1;

        public string? Name { get; init; }

        public int[]? Categories { get; init; }
    }

    private record CategoryVolume(int PlayerId, int CategoryId, float OriginalVolume);
}
