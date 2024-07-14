namespace Ryo.Reloaded.CRI.CriUnreal;

internal static class CriUnrealGames
{
    private static readonly CriUnrealPatterns[] patterns = new CriUnrealPatterns[]
    {
        new("p3r")
        {
            //USoundAtomCueSheet_AsyncLoadCueSheetTask = "4C 8B DC 55 53 49 8D AB ?? ?? ?? ?? 48 81 EC B8 04 00 00",
            //USoundAtomCueSheet_GetAtomCueById = "40 55 41 56 48 81 EC 58 02 00 00 48 8B 05 ?? ?? ?? ?? 48 31 E0",
            //PlayAdxControl_RequestLoadAcb = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 20 48 8D B9 ?? ?? ?? ?? 49 89 D6",
            //PlayAdxControl_RequestSound = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 45 31 D2",
            //PlayAdxControl_SetPlayerAcbBank = "48 89 5C 24 ?? 8B 5C 24 ?? 48 8D 41",
            //USoundAtomCueSheet_LoadAtomCueSheet = "40 56 48 83 EC 60 48 89 CE 84 D2",
            //PlayAdxControl_CreatePlayerBank = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 31 FF 4C 8D 51",
        },
        new("SMT5V-Win64-Shipping")
        {
            //USoundAtomCueSheet_AsyncLoadCueSheetTask_SMTV = "40 55 56 48 8D AC 24 ?? ?? ?? ?? 48 81 EC D8 01 00 00",
        }
    };

    public static CriUnrealPatterns GetGamePatterns(string game)
        => patterns.FirstOrDefault(x => x.Games.Contains(game, StringComparer.OrdinalIgnoreCase)) ?? new();
}

internal class CriUnrealPatterns
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public CriUnrealPatterns(params string[] games)
    {
        this.Games = games;
    }

    public string[] Games { get; }
    public string PlayAdxControl_RequestLoadAcb { get; internal set; }
    public string PlayAdxControl_RequestSound { get; internal set; }
    public string PlayAdxControl_SetPlayerAcbBank { get; internal set; }
    public string USoundAtomCueSheet_LoadAtomCueSheet { get; internal set; }
    public string USoundAtomCueSheet_GetAtomCueById { get; internal set; }
    public string PlayAdxControl_CreatePlayerBank { get; internal set; }
    public string USoundAtomCueSheet_AsyncLoadCueSheetTask { get; internal set; }
    public string USoundAtomCueSheet_AsyncLoadCueSheetTask_SMTV { get; internal set; }
}
