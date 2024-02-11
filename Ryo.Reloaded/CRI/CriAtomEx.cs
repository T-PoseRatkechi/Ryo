using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using static Ryo.Reloaded.CRI.CriAtomExFunctions;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using CommunityToolkit.Mvvm.ComponentModel;
using Ryo.Interfaces.Types;
using Ryo.Interfaces;

namespace Ryo.Reloaded.CRI;

internal unsafe partial class CriAtomEx : ObservableObject, IGameHook, ICriAtomEx
{
    private readonly string game;
    private readonly CriAtomExPatterns patterns;

    private readonly List<ScanHook> scans = new();
    private readonly Dictionary<int, CriAtomExPlayerConfigTag> playerConfigs = new();
    private readonly List<PlayerConfig> players = new();
    private readonly List<AcbConfig> acbs = new();

    private IFunction<criAtomExPlayer_Create>? create;
    private IFunction<criAtomExPlayer_SetStartTime>? setStartTime;
    private IFunction<criAtomExPlayback_GetTimeSyncedWithAudio>? getTimeSyncedWithAudio;
    private IFunction<criAtomExPlayer_GetNumPlayedSamples>? getNumPlayedSamples;
    private IFunction<criAtomExAcb_LoadAcbFile>? loadAcbFile;
    private IFunction<criAtomExPlayer_Start>? start;
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

    [ObservableProperty]
    private IFunction<criAtomExPlayer_SetCueName>? setCueName;

    [ObservableProperty]
    private IFunction<criAtomExPlayer_SetCueId>? setCueId;

    private IHook<criAtomExPlayer_Create>? createHook;
    private IHook<criAtomExAcb_LoadAcbFile>? loadAcbFileHook;
    private IHook<criAtomExPlayer_SetCueId>? setCueIdHook;
    private IHook<criAtomExPlayer_SetCueName>? setCueNameHook;

    private bool devMode;

    public CriAtomEx(string game)
    {
        this.game = game;
        this.patterns = CriAtomExGames.GetGamePatterns(game);

        this.AddHookScan(
            nameof(criAtomExPlayer_Create),
            this.patterns.CriAtomExPlayer_Create,
            (hooks, result) =>
            {
                this.create = hooks.CreateFunction<criAtomExPlayer_Create>(result);
                this.createHook = this.create.Hook(this.Player_Create).Activate();
            });

        this.AddHookScan(
            nameof(criAtomExAcb_LoadAcbFile),
            this.patterns.CriAtomExAcb_LoadAcbFile,
            (hooks, result) =>
            {
                this.loadAcbFile = hooks.CreateFunction<criAtomExAcb_LoadAcbFile>(result);
                this.loadAcbFileHook = this.loadAcbFile.Hook(this.Acb_LoadAcbFile).Activate();
            });

        this.AddHookScan(
            nameof(criAtomExPlayer_SetCueName),
            this.patterns.CriAtomExPlayer_SetCueName,
            (hooks, result) =>
            {
                this.SetCueName = hooks.CreateFunction<criAtomExPlayer_SetCueName>(result);
                this.setCueNameHook = this.SetCueName.Hook(this.Player_SetCueName).Activate();
            });

        this.AddHookScan(
            nameof(criAtomExPlayer_SetCueId),
            this.patterns.CriAtomExPlayer_SetCueId,
            (hooks, result) =>
            {
                this.SetCueId = hooks.CreateFunction<criAtomExPlayer_SetCueId>(result);
                this.setCueIdHook = this.SetCueId.Hook(this.Player_SetCueId).Activate();
            });

        this.AddHookScan(
            nameof(criAtomExPlayer_SetStartTime),
            this.patterns.CriAtomExPlayer_SetStartTime,
            (hooks, result) => this.setStartTime = hooks.CreateFunction<criAtomExPlayer_SetStartTime>(result));

        this.AddHookScan(
            nameof(criAtomExPlayback_GetTimeSyncedWithAudio),
            this.patterns.CriAtomExPlayback_GetTimeSyncedWithAudio,
            (hooks, result) => this.getTimeSyncedWithAudio = hooks.CreateFunction<criAtomExPlayback_GetTimeSyncedWithAudio>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_GetNumPlayedSamples),
            this.patterns.CriAtomExPlayer_GetNumPlayedSamples,
            (hooks, result) => this.getNumPlayedSamples = hooks.CreateFunction<criAtomExPlayer_GetNumPlayedSamples>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_Start),
            this.patterns.CriAtomExPlayer_Start,
            (hooks, result) => this.start = hooks.CreateFunction<criAtomExPlayer_Start>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetFile),
            this.patterns.CriAtomExPlayer_SetFile,
            (hooks, result) => this.setFile = hooks.CreateFunction<criAtomExPlayer_SetFile>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetFormat),
            this.patterns.CriAtomExPlayer_SetFormat,
            (hooks, result) => this.setFormat = hooks.CreateFunction<criAtomExPlayer_SetFormat>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetSamplingRate),
            this.patterns.CriAtomExPlayer_SetSamplingRate,
            (hooks, result) => this.setSamplingRate = hooks.CreateFunction<criAtomExPlayer_SetSamplingRate>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetNumChannels),
            this.patterns.CriAtomExPlayer_SetNumChannels,
            (hooks, result) => this.setNumChannels = hooks.CreateFunction<criAtomExPlayer_SetNumChannels>(result));

        this.AddHookScan(
            nameof(criAtomExCategory_GetVolumeById),
            this.patterns.CriAtomExCategory_GetVolumeById,
            (hooks, result) => this.getVolumeById = hooks.CreateFunction<criAtomExCategory_GetVolumeById>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetVolume),
            this.patterns.CriAtomExPlayer_SetVolume,
            (hooks, result) => this.setVolume = hooks.CreateFunction<criAtomExPlayer_SetVolume>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetCategoryById),
            this.patterns.CriAtomExPlayer_SetCategoryById,
            (hooks, result) => this.setCategoryById = hooks.CreateFunction<criAtomExPlayer_SetCategoryById>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_GetLastPlaybackId),
            this.patterns.CriAtomExPlayer_GetLastPlaybackId,
            (hooks, result) => this.getLastPlaybackId = hooks.CreateFunction<criAtomExPlayer_GetLastPlaybackId>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetCategoryByName),
            this.patterns.CriAtomExPlayer_SetCategoryByName,
            (hooks, result) => this.setCategoryByName = hooks.CreateFunction<criAtomExPlayer_SetCategoryByName>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_GetCategoryInfo),
            this.patterns.CriAtomExPlayer_GetCategoryInfo,
            (hooks, result) => this.getCategoryInfo = hooks.CreateFunction<criAtomExPlayer_GetCategoryInfo>(result));

        this.AddHookScan(
            nameof(criAtomExPlayer_SetData),
            this.patterns.CriAtomExPlayer_SetData,
            (hooks, result) => this.setData = hooks.CreateFunction<criAtomExPlayer_SetData>(result));
    }

    public void Initialize(IStartupScanner scanner, IReloadedHooks hooks)
    {
        foreach (var scan in this.scans)
        {
            if (string.IsNullOrEmpty(scan.Pattern))
            {
                Log.Verbose($"{scan.Name}: No pattern given.");
                continue;
            }

            scanner.Scan(scan.Name, scan.Pattern, result => scan.Success(hooks, result));
        }
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

        this.setCueIdHook!.OriginalFunction(playerHn, acbHn, cueId);
    }

    public void Player_SetCueName(nint playerHn, nint acbHn, byte* cueName)
    {
        if (this.devMode)
        {
            var player = this.players.First(x => x.PlayerHn == playerHn);
            var cueNameStr = Marshal.PtrToStringAnsi((nint)cueName);
            Log.Information($"{nameof(criAtomExPlayer_SetCueName)} || Player ID: {player.Id} || {cueNameStr}");
        }

        this.setCueNameHook!.OriginalFunction(playerHn, acbHn, cueName);
    }

    public void SetPlayerConfigById(int id, CriAtomExPlayerConfigTag config)
        => this.playerConfigs[id] = config;

    public int Playback_GetTimeSyncedWithAudio(uint playbackId)
        => this.getTimeSyncedWithAudio!.GetWrapper()(playbackId);

    public uint Player_Start(nint playerHn)
        => this.start!.GetWrapper()(playerHn);

    public void Player_SetStartTime(nint playerHn, int currentBgmTime)
        => this.setStartTime!.GetWrapper()(playerHn, currentBgmTime);

    public void Player_SetFile(nint playerHn, nint criBinderHn, byte* path)
        => this.setFile!.GetWrapper()(playerHn, criBinderHn, path);

    public void Player_SetFormat(nint playerHn, CriAtom_Format format)
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

    public void Player_SetData(nint playerHn, byte* buffer, int size)
        => this.setData!.GetWrapper()(playerHn, buffer, size);

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

    private void AddHookScan(string name, string? pattern, Action<IReloadedHooks, nint> success)
        => this.scans.Add(new(name, pattern, success));
}

internal class ScanHook
{
    public ScanHook(string name, string? pattern, Action<IReloadedHooks, nint> success)
    {
        this.Name = name;
        this.Pattern = pattern;
        this.Success = success;
    }

    public string Name { get; }

    public string? Pattern { get; }

    public Action<IReloadedHooks, nint> Success { get; }
};