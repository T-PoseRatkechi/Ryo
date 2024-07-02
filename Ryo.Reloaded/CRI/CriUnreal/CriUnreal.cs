using Reloaded.Hooks.Definitions;
using Ryo.Definitions.Structs;
using Ryo.Reloaded.Audio;
using static Ryo.Definitions.Functions.CriUnrealFunctions;

namespace Ryo.Reloaded.CRI.CriUnreal;

internal unsafe class CriUnreal
{
    private readonly string game;
    private readonly CriUnrealPatterns patterns;
    private IFunction<USoundAtomCueSheet_AsyncLoadCueSheetTask> asyncLoadCueSheetTask;
    private IHook<USoundAtomCueSheet_AsyncLoadCueSheetTask> asyncLoadCueSheetTaskHook;
    private IFunction<PlayAdxControl_CreatePlayerBank> createPlayerBank;
    private IFunction<USoundAtomCueSheet_GetAtomCueById> getAtomCueById;
    private IFunction<USoundAtomCueSheet_LoadAtomCueSheet> loadAtomCueSheet;
    private IHook<USoundAtomCueSheet_LoadAtomCueSheet> loadAtomCueSheetHook;
    private IFunction<PlayAdxControl_SetPlayerAcbBank> setPlayerAcbBank;
    private IFunction<PlayAdxControl_RequestSound> requestSound;
    private IFunction<PlayAdxControl_RequestLoadAcb> requestLoadAcb;
    private bool devMode;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public CriUnreal(string game)
    {
        this.game = game;
        this.patterns = CriUnrealGames.GetGamePatterns(game);

        ScanHooks.Add(
            nameof(USoundAtomCueSheet_AsyncLoadCueSheetTask),
            this.patterns.USoundAtomCueSheet_AsyncLoadCueSheetTask,
            (hooks, result) =>
            {
                this.asyncLoadCueSheetTask = hooks.CreateFunction<USoundAtomCueSheet_AsyncLoadCueSheetTask>(result);
                this.asyncLoadCueSheetTaskHook = this.asyncLoadCueSheetTask.Hook(this.USoundAtomCueSheet_AsyncLoadCueSheetTask).Activate();
            });

        ScanHooks.Add(
            nameof(USoundAtomCueSheet_GetAtomCueById),
            this.patterns.USoundAtomCueSheet_GetAtomCueById,
            (hooks, result) => this.getAtomCueById = hooks.CreateFunction<USoundAtomCueSheet_GetAtomCueById>(result));

        ScanHooks.Add(
            nameof(PlayAdxControl_RequestSound),
            this.patterns.PlayAdxControl_RequestSound,
            (hooks, result) => this.requestSound = hooks.CreateFunction<PlayAdxControl_RequestSound>(result));

        ScanHooks.Add(
            nameof(PlayAdxControl_RequestLoadAcb),
            this.patterns.PlayAdxControl_RequestLoadAcb,
            (hooks, result) => this.requestLoadAcb = hooks.CreateFunction<PlayAdxControl_RequestLoadAcb>(result));

        ScanHooks.Add(
            nameof(PlayAdxControl_SetPlayerAcbBank),
            this.patterns.PlayAdxControl_SetPlayerAcbBank,
            (hooks, result) => this.setPlayerAcbBank = hooks.CreateFunction<PlayAdxControl_SetPlayerAcbBank>(result));

        ScanHooks.Add(
            nameof(USoundAtomCueSheet_LoadAtomCueSheet),
            this.patterns.USoundAtomCueSheet_LoadAtomCueSheet,
            (hooks, result) =>
            {
                this.loadAtomCueSheet = hooks.CreateFunction<USoundAtomCueSheet_LoadAtomCueSheet>(result);
                this.loadAtomCueSheetHook = this.loadAtomCueSheet.Hook(this.USoundAtomCueSheet_LoadAtomCueSheet).Activate();
            });

        ScanHooks.Add(
            nameof(PlayAdxControl_CreatePlayerBank),
            this.patterns.PlayAdxControl_CreatePlayerBank,
            (hooks, result) => this.createPlayerBank = hooks.CreateFunction<PlayAdxControl_CreatePlayerBank>(result));
    }

    private USoundAtomCueSheet* USoundAtomCueSheet_LoadAtomCueSheet(USoundAtomCueSheet* instance, bool bAddToLevel)
    {
        Log.Debug($"Loaded AtomCueSheet: {instance->CueSheetName.GetString()} || {(nint)instance->_AcbHn:X}");
        return this.loadAtomCueSheetHook.OriginalFunction(instance, bAddToLevel);
    }

    public void SetDevMode(bool devMode)
    {
        this.devMode = devMode;
    }

    private unsafe void USoundAtomCueSheet_AsyncLoadCueSheetTask(LoadTaskParameter* taskParams)
    {
        this.asyncLoadCueSheetTaskHook.OriginalFunction(taskParams);

        var acbName = taskParams->CueSheet->CueSheetName.GetString()!;
        var acbHn = *taskParams->CueSheet->_AcbHn;

        AcbRegistry.Register(acbName, acbHn);
    }
}
