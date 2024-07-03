using Reloaded.Hooks.Definitions.X64;
using Ryo.Definitions.Structs;

namespace Ryo.Definitions.Functions;

public static unsafe class CriUnrealFunctions
{
    [Function(CallingConventions.Microsoft)]
    public delegate int PlayAdxControl_RequestLoadAcb(UPlayAdxControl* instance, FString filename);

    [Function(CallingConventions.Microsoft)]
    public delegate void PlayAdxControl_RequestSound(UPlayAdxControl* instance, int playerMajorID, int playerMinorID, int cueId);

    [Function(CallingConventions.Microsoft)]
    public delegate void PlayAdxControl_SetPlayerAcbBank(UPlayAdxControl* instance, int playerMajorID, int playerMinorID, EPlayerType type, int bankId);

    [Function(CallingConventions.Microsoft)]
    public delegate USoundAtomCueSheet* USoundAtomCueSheet_LoadAtomCueSheet(USoundAtomCueSheet* instance, bool bAddToLevel);

    [Function(CallingConventions.Microsoft)]
    public delegate USoundAtomCue* USoundAtomCueSheet_GetAtomCueById(USoundAtomCueSheet* instance, int cueId);

    [Function(CallingConventions.Microsoft)]
    public delegate int PlayAdxControl_CreatePlayerBank(UPlayAdxControl* instance, int playerMajorID, int playerMinorID, bool isMulti);

    [Function(CallingConventions.Microsoft)]
    public delegate void USoundAtomCueSheet_AsyncLoadCueSheetTask(LoadTaskParameter* taskParams);

    [Function(CallingConventions.Microsoft)]
    public delegate void USoundAtomCueSheet_AsyncLoadCueSheetTask_SMTV(USoundAtomCueSheetSMTV* cueSheet);
}
