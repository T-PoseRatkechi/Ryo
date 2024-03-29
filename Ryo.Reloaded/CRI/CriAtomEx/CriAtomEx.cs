﻿using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Ryo.Interfaces;
using Ryo.Definitions.Structs;
using Ryo.Definitions.Classes;
using static Ryo.Definitions.Functions.CriAtomExFunctions;
using Ryo.Definitions.Enums;
using SharedScans.Interfaces;
using Ryo.Reloaded.Audio;

namespace Ryo.Reloaded.CRI.CriAtomEx;

internal unsafe class CriAtomEx : ICriAtomEx
{
    private readonly string game;
    private readonly CriAtomExPatterns patterns;

    private readonly Dictionary<int, CriAtomExPlayerConfigTag> playerConfigs = new();
    private readonly List<PlayerConfig> players = new();
    private readonly List<AcbConfig> acbs = new();

    private IFunction<criAtomExPlayer_Create>? create;
    private IFunction<criAtomExPlayer_SetStartTime>? setStartTime;
    private IFunction<criAtomExPlayback_GetTimeSyncedWithAudio>? getTimeSyncedWithAudio;
    private IFunction<criAtomExPlayer_GetNumPlayedSamples>? getNumPlayedSamples;
    private IFunction<criAtomExAcb_LoadAcbFile>? loadAcbFile;
    private IFunction<criAtomExPlayer_SetFile>? setFile;
    private IFunction<criAtomExPlayer_SetFormat>? setFormat;
    private IFunction<criAtomExPlayer_SetSamplingRate>? setSamplingRate;
    private IFunction<criAtomExPlayer_SetNumChannels>? setNumChannels;
    private IFunction<criAtomExCategory_GetVolumeById>? getVolumeById;
    private IFunction<criAtomExPlayer_SetVolume>? setVolume;
    private IFunction<criAtomExPlayer_SetCategoryById>? setCategoryById;
    private IFunction<criAtomExPlayer_GetLastPlaybackId>? getLastPlaybackId;
    private IFunction<criAtomExPlayer_SetCategoryByName>? setCategoryByName;
    private IFunction<criAtomExPlayer_GetCategoryInfo>? getCategoryInfo;
    private IFunction<criAtomExPlayer_SetData>? setData;
    private IFunction<criAtomExCategory_SetVolumeById>? setVolumeById;
    private IFunction<criAtomExPlayer_UpdateAll>? updateAll;
    private IFunction<criAtomExPlayer_LimitLoopCount>? limitLoopCount;
    private IFunction<criAtomExPlayer_GetStatus>? getStatus;
    private IFunction<criAtomExPlayer_Stop>? stop;
    private IFunction<criAtomExPlayer_SetAisacControlByName>? setAisacControlByName;

    private IHook<criAtomExPlayer_Create>? createHook;
    private IHook<criAtomExAcb_LoadAcbFile>? loadAcbFileHook;

    private bool devMode;
    private readonly HookContainer<criAtomExPlayer_SetCueId> setCueId;
    private readonly WrapperContainer<criAtomExPlayer_SetCueName> setCueName;
    private readonly WrapperContainer<criAtomExPlayer_Start> start;

    public CriAtomEx(string game, ISharedScans scans)
    {
        this.game = game;
        this.patterns = CriAtomExGames.GetGamePatterns(game);

        scans.AddScan<criAtomExPlayer_SetCueId>(this.patterns.CriAtomExPlayer_SetCueId);
        this.setCueId = scans.CreateHook<criAtomExPlayer_SetCueId>(this.Player_SetCueId, Mod.NAME);

        scans.AddScan<criAtomExPlayer_SetCueName>(this.patterns.criAtomExPlayer_SetCueName);
        this.setCueName = scans.CreateWrapper<criAtomExPlayer_SetCueName>(Mod.NAME);

        scans.AddScan<criAtomExPlayer_Start>(this.patterns.criAtomExPlayer_Start);
        this.start = scans.CreateWrapper<criAtomExPlayer_Start>(Mod.NAME);

        ScanHooks.Add(
            nameof(criAtomExPlayer_Create),
            this.patterns.criAtomExPlayer_Create,
            (hooks, result) =>
            {
                this.create = hooks.CreateFunction<criAtomExPlayer_Create>(result);
                this.createHook = this.create.Hook(this.Player_Create).Activate();
            });

        ScanHooks.Add(
            nameof(criAtomExAcb_LoadAcbFile),
            this.patterns.CriAtomExAcb_LoadAcbFile,
            (hooks, result) =>
            {
                this.loadAcbFile = hooks.CreateFunction<criAtomExAcb_LoadAcbFile>(result);
                this.loadAcbFileHook = this.loadAcbFile.Hook(this.Acb_LoadAcbFile).Activate();
            });

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetStartTime),
            this.patterns.CriAtomExPlayer_SetStartTime,
            (hooks, result) => this.setStartTime = hooks.CreateFunction<criAtomExPlayer_SetStartTime>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayback_GetTimeSyncedWithAudio),
            this.patterns.CriAtomExPlayback_GetTimeSyncedWithAudio,
            (hooks, result) => this.getTimeSyncedWithAudio = hooks.CreateFunction<criAtomExPlayback_GetTimeSyncedWithAudio>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_GetNumPlayedSamples),
            this.patterns.CriAtomExPlayer_GetNumPlayedSamples,
            (hooks, result) => this.getNumPlayedSamples = hooks.CreateFunction<criAtomExPlayer_GetNumPlayedSamples>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetFile),
            this.patterns.CriAtomExPlayer_SetFile,
            (hooks, result) => this.setFile = hooks.CreateFunction<criAtomExPlayer_SetFile>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetFormat),
            this.patterns.criAtomExPlayer_SetFormat,
            (hooks, result) => this.setFormat = hooks.CreateFunction<criAtomExPlayer_SetFormat>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetSamplingRate),
            this.patterns.criAtomExPlayer_SetSamplingRate,
            (hooks, result) => this.setSamplingRate = hooks.CreateFunction<criAtomExPlayer_SetSamplingRate>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetNumChannels),
            this.patterns.criAtomExPlayer_SetNumChannels,
            (hooks, result) => this.setNumChannels = hooks.CreateFunction<criAtomExPlayer_SetNumChannels>(result));

        ScanHooks.Add(
            nameof(criAtomExCategory_GetVolumeById),
            this.patterns.CriAtomExCategory_GetVolumeById,
            (hooks, result) => this.getVolumeById = hooks.CreateFunction<criAtomExCategory_GetVolumeById>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetVolume),
            this.patterns.criAtomExPlayer_SetVolume,
            (hooks, result) => this.setVolume = hooks.CreateFunction<criAtomExPlayer_SetVolume>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetCategoryById),
            this.patterns.criAtomExPlayer_SetCategoryById,
            (hooks, result) => this.setCategoryById = hooks.CreateFunction<criAtomExPlayer_SetCategoryById>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_GetLastPlaybackId),
            this.patterns.CriAtomExPlayer_GetLastPlaybackId,
            (hooks, result) => this.getLastPlaybackId = hooks.CreateFunction<criAtomExPlayer_GetLastPlaybackId>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetCategoryByName),
            this.patterns.CriAtomExPlayer_SetCategoryByName,
            (hooks, result) => this.setCategoryByName = hooks.CreateFunction<criAtomExPlayer_SetCategoryByName>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_GetCategoryInfo),
            this.patterns.CriAtomExPlayer_GetCategoryInfo,
            (hooks, result) => this.getCategoryInfo = hooks.CreateFunction<criAtomExPlayer_GetCategoryInfo>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetData),
            this.patterns.criAtomExPlayer_SetData,
            (hooks, result) => this.setData = hooks.CreateFunction<criAtomExPlayer_SetData>(result));

        ScanHooks.Add(
            nameof(criAtomExCategory_SetVolumeById),
            this.patterns.criAtomExCategory_SetVolumeById,
            (hooks, result) => this.setVolumeById = hooks.CreateFunction<criAtomExCategory_SetVolumeById>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_UpdateAll),
            this.patterns.criAtomExPlayer_UpdateAll,
            (hooks, result) => this.updateAll = hooks.CreateFunction<criAtomExPlayer_UpdateAll>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_LimitLoopCount),
            this.patterns.criAtomExPlayer_LimitLoopCount,
            (hooks, result) => this.limitLoopCount = hooks.CreateFunction<criAtomExPlayer_LimitLoopCount>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_GetStatus),
            this.patterns.criAtomExPlayer_GetStatus,
            (hooks, result) => this.getStatus = hooks.CreateFunction<criAtomExPlayer_GetStatus>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_Stop),
            this.patterns.criAtomExPlayer_Stop,
            (hooks, result) => this.stop = hooks.CreateFunction<criAtomExPlayer_Stop>(result));

        ScanHooks.Add(
            nameof(criAtomExPlayer_SetAisacControlByName),
            this.patterns.criAtomExPlayer_SetAisacControlByName,
            (hooks, result) => this.setAisacControlByName = hooks.CreateFunction<criAtomExPlayer_SetAisacControlByName>(result));
    }

    public void SetDevMode(bool devMode)
        => this.devMode = devMode;

    public PlayerConfig? GetPlayerByAcbPath(string acbPath)
    {
        var player = this.players.FirstOrDefault(x => x.Acb.AcbPath == acbPath);
        if (player != null)
        {
            Log.Debug($"PlayerHn: {player.PlayerHn} || ACB Path: {player.Acb.AcbPath} || ID: {this.players.IndexOf(player)}");
        }

        return player;
    }

    public PlayerConfig? GetPlayerByHn(nint playerHn)
        => this.players.FirstOrDefault(x => x.PlayerHn == playerHn);

    public PlayerConfig? GetPlayerById(int playerId)
        => this.players.FirstOrDefault(x => x.Id == playerId);

    public CriAtomExPlayerStatusTag Player_GetStatus(nint playerHn)
        => this.getStatus!.GetWrapper()(playerHn);

    public void Player_LimitLoopCount(nint playerHn, int count)
        => this.limitLoopCount!.GetWrapper()(playerHn, count);

    public void Player_Stop(nint playerHn)
        => this.stop!.GetWrapper()(playerHn);

    public void Player_SetAisacControlByName(nint playerHn, byte* controlName, float controlValue)
        => this.setAisacControlByName!.GetWrapper()(playerHn, controlName, controlValue);

    public void Player_SetCueId(nint playerHn, nint acbHn, int cueId)
    {
        // Update player ACB.
        var player = this.players.First(x => x.PlayerHn == playerHn);
        var acb = this.acbs.FirstOrDefault(x => x.AcbHn == acbHn);
        if (acb != null)
        {
            player.Acb = acb;
        }
        else
        {
            Log.Debug($"Unknown ACB Hn: {acbHn}");
        }

        this.setCueId.Hook!.OriginalFunction(playerHn, acbHn, cueId);
    }

    public void Player_SetCueName(nint playerHn, nint acbHn, byte* cueName)
        => this.setCueName.Wrapper(playerHn, acbHn, cueName);

    public void SetPlayerConfigById(int id, CriAtomExPlayerConfigTag config)
        => this.playerConfigs[id] = config;

    public int Playback_GetTimeSyncedWithAudio(uint playbackId)
        => this.getTimeSyncedWithAudio!.GetWrapper()(playbackId);

    public uint Player_Start(nint playerHn)
        => this.start.Wrapper(playerHn);

    public void Player_SetStartTime(nint playerHn, int currentBgmTime)
        => this.setStartTime!.GetWrapper()(playerHn, currentBgmTime);

    public void Player_SetFile(nint playerHn, nint criBinderHn, byte* path)
        => this.setFile!.GetWrapper()(playerHn, criBinderHn, path);

    public void Player_SetFormat(nint playerHn, CriAtomFormat format)
        => this.setFormat!.GetWrapper()(playerHn, format);

    public void Player_SetNumChannels(nint playerHn, int numChannels)
        => this.setNumChannels!.GetWrapper()(playerHn, numChannels);

    public void Player_SetCategoryById(nint playerHn, uint id)
        => this.setCategoryById!.GetWrapper()(playerHn, id);

    public void Player_SetSamplingRate(nint playerHn, int samplingRate)
        => this.setSamplingRate!.GetWrapper()(playerHn, samplingRate);
    public uint Player_GetLastPlaybackId(nint playerHn)
        => this.getLastPlaybackId!.GetWrapper()(playerHn);

    public void Player_SetCategoryByName(nint playerHn, byte* name)
        => this.setCategoryByName!.GetWrapper()(playerHn, name);

    public bool Player_GetCategoryInfo(nint playerHn, ushort index, CriAtomExCategoryInfo* info)
        => this.getCategoryInfo!.GetWrapper()(playerHn, index, info);

    public float Category_GetVolumeById(uint id)
        => this.getVolumeById!.GetWrapper()(id);

    public void Player_SetVolume(nint playerHn, float volume)
        => this.setVolume!.GetWrapper()(playerHn, volume);

    public void Category_SetVolumeById(uint id, float volume)
        => this.setVolumeById!.GetWrapper()(id, volume);

    public void Player_SetData(nint playerHn, byte* buffer, int size)
        => this.setData!.GetWrapper()(playerHn, buffer, size);

    public void Player_UpdateAll(nint playerHn)
        => this.updateAll!.GetWrapper()(playerHn);

    private nint Player_Create(CriAtomExPlayerConfigTag* config, void* work, int workSize)
    {
        var playerId = this.players.Count;
        Log.Verbose($"{nameof(criAtomExPlayer_Create)} || Config: {(nint)config:X} || Work: {(nint)work:X} || WorkSize: {workSize}");

        CriAtomExPlayerConfigTag* currentConfigPtr;
        if (this.playerConfigs.TryGetValue(playerId, out var newConfig))
        {
            currentConfigPtr = (CriAtomExPlayerConfigTag*)Marshal.AllocHGlobal(sizeof(CriAtomExPlayerConfigTag));
            Marshal.StructureToPtr(newConfig, (nint)currentConfigPtr, false);
            Log.Information($"Using custom player config for: {playerId}");
        }
        else
        {
            currentConfigPtr = config;
        }

        var playerHn = this.createHook!.OriginalFunction(currentConfigPtr, work, workSize);
        this.players.Add(new(playerId, playerHn));
        PlayerRegistry.RegisterPlayer(playerHn);

        Log.Debug($"Player: {playerHn:X} || ID: {playerId}");
        return playerHn;
    }

    private nint Acb_LoadAcbFile(nint acbBinder, byte* acbPathStr, nint awbBinder, byte* awbPathStr, void* work, int workSize)
    {
        var acbHn = this.loadAcbFileHook!.OriginalFunction(acbBinder, acbPathStr, awbBinder, awbPathStr, work, workSize);
        var acbPath = Marshal.PtrToStringAnsi((nint)acbPathStr)!;

        this.acbs.Add(new()
        {
            AcbHn = acbHn,
            AcbPath = acbPath,
        });

        Log.Debug($"{nameof(criAtomExAcb_LoadAcbFile)}: {acbPath} || {acbHn:X}");
        return acbHn;
    }
}
