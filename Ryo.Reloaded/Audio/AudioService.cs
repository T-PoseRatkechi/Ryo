using Ryo.Definitions.Structs;
using Ryo.Interfaces;
using Ryo.Reloaded.Audio.Services;
using Ryo.Reloaded.CRI.CriAtomEx;
using SharedScans.Interfaces;
using System.Runtime.InteropServices;
using static Ryo.Definitions.Functions.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio;

internal unsafe class AudioService
{
    private readonly ICriAtomEx criAtomEx;
    private readonly CriAtomRegistry criAtomRegistry;
    private readonly AudioRegistry audioRegistry;

    private readonly RyoService ryo;
    private readonly VirtualCueService virtualCue;

    private bool devMode;
    private readonly HookContainer<criAtomExPlayer_SetCueName> setCueName;
    private readonly HookContainer<criAtomExPlayer_SetCueId> setCueId;
    private readonly HookContainer<criAtomExPlayer_SetFile> setFile;
    private readonly HookContainer<criAtomExPlayer_SetWaveId> setWaveId;
    private readonly HookContainer<criAtomExPlayer_SetData> setData;

    public AudioService(
        string game,
        ISharedScans scans,
        ICriAtomEx criAtomEx,
        CriAtomRegistry criAtomRegistry,
        AudioRegistry audioRegistry)
    {
        this.criAtomEx = criAtomEx;
        this.criAtomRegistry = criAtomRegistry;
        this.audioRegistry = audioRegistry;

        this.ryo = new(game, criAtomEx, criAtomRegistry);
        this.virtualCue = new(scans, criAtomRegistry, audioRegistry);

        GameDefaults.ConfigureCriAtom(game, criAtomEx);

        this.setCueName = scans.CreateHook<criAtomExPlayer_SetCueName>(this.CriAtomExPlayer_SetCueName, Mod.NAME);
        this.setCueId = scans.CreateHook<criAtomExPlayer_SetCueId>(this.CriAtomExPlayer_SetCueId, Mod.NAME);
        this.setFile = scans.CreateHook<criAtomExPlayer_SetFile>(this.CriAtomExPlayer_SetFile, Mod.NAME);
        this.setData = scans.CreateHook<criAtomExPlayer_SetData>(this.CriAtomExPlayer_SetData, Mod.NAME);
        this.setWaveId = scans.CreateHook<criAtomExPlayer_SetWaveId>(this.CriAtomExPlayer_SetWaveId, Mod.NAME);
    }

    public void SetDevMode(bool devMode)
        => this.devMode = devMode;

    private bool SetCue(nint playerHn, nint acbHn, Cue cue)
    {
        var player = this.criAtomRegistry.GetPlayerByHn(playerHn)!;
        var acbName = this.criAtomRegistry.GetAcbByHn(acbHn)?.Name ?? "Unknown";

        if (this.devMode)
        {
            var cueIdString = cue.Id == -1 ? "(N/A)" : cue.Id.ToString();
            var cueNameString = cue.Name ?? "(N/A)";

            Log.Information($"{nameof(SetCue)} || Player: {player.Id} || ACB: {acbName} || Cue: {cueIdString} / {cueNameString}");
            if (cue.Categories?.Length > 0)
            {
                Log.Debug($"Categories: {string.Join(", ", cue.Categories.Select(x => x.ToString()))}");
            }
        }

        if (cue.Id != -1 && this.audioRegistry.TryGetCueContainer(cue.Id.ToString(), acbName, out var idContainer))
        {
            this.ryo.SetAudio(player, idContainer, idContainer.CategoryIds ?? cue.Categories);
            return true;
        }
        else if (cue.Name != null && this.audioRegistry.TryGetCueContainer(cue.Name, acbName, out var nameContainer))
        {
            this.ryo.SetAudio(player, nameContainer, nameContainer.CategoryIds ?? cue.Categories);
            return true;
        }
        else
        {
            this.ryo.ResetPlayerVolume(playerHn);
            return false;
        }
    }

    private void CriAtomExPlayer_SetCueName(nint playerHn, nint acbHn, byte* cueNameStr)
    {
        var cueInfo = (CriAtomExCueInfoTag*)Marshal.AllocHGlobal(sizeof(CriAtomExCueInfoTag));

        var cueName = Marshal.PtrToStringAnsi((nint)cueNameStr)!;
        var cueId = -1;
        int[]? categories = null;
        
        if (this.criAtomEx.Acb_GetCueInfoByName(acbHn, (nint)cueNameStr, cueInfo))
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

        if (this.criAtomEx.Acb_GetCueInfoById(acbHn, cueId, cueInfo))
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
            this.ryo.SetAudio(player, file, file.CategoryIds);
        }
        else
        {
            this.ryo.ResetPlayerVolume(playerHn);
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
            this.ryo.SetAudio(player, data, data.CategoryIds);
        }
        else
        {
            this.ryo.ResetPlayerVolume(playerHn);
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
            this.ryo.SetAudio(player, file, file.CategoryIds);
        }
        else
        {
            this.ryo.ResetPlayerVolume(playerHn);
            this.setWaveId.Hook!.OriginalFunction(playerHn, awbHn, waveId);
        }
    }

    private class Cue
    {
        public int Id { get; init; } = -1;

        public string? Name { get; init; }

        public int[]? Categories { get; init; }
    }
}
