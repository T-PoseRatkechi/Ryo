namespace Ryo.Reloaded.CRI.CriAtomEx;

internal static class CriAtomExGames
{
    private static readonly CriAtomExPatterns[] patterns = new CriAtomExPatterns[]
    {
        new("p3r")
        {
            criAtomExPlayer_Create = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8B EC 48 83 EC 40 45 33 ED 41 8B F8",
            criAtomExPlayer_Start = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C8 FF",
            criAtomExPlayer_SetData = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 44 89 C6",
            criAtomExPlayer_SetFormat = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 48 8D 15",
            criAtomExPlayer_SetNumChannels = "48 89 5C 24 ?? 57 48 83 EC 20 89 D7 48 89 CB 48 85 C9 74 ?? 8D 42",
            criAtomExPlayer_SetSamplingRate = "48 89 5C 24 ?? 57 48 83 EC 20 89 D7 48 89 CB 48 85 C9 74 ?? 85 D2",
            criAtomExPlayer_SetCategoryById = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 50 48 8B F9 8B F2",
            criAtomExPlayer_SetCueName = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 49 8B F8 48 8B EA 48 8B D9",
            criAtomExPlayer_SetVolume = "48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8B 89 ?? ?? ?? ?? 33 D2",
            criAtomExCategory_SetVolumeById = "40 53 48 83 EC 30 8B D9 0F 29 74 24",
            criAtomExPlayer_UpdateAll = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? EB",
            criAtomExPlayer_LimitLoopCount = "44 8B C2 48 85 C9 74 ?? 83 FA FD",
            criAtomExPlayer_GetStatus = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 8D 47",
            criAtomExPlayer_Stop = "40 53 48 83 EC 20 48 8B D9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? 48 83 C4 20 5B E9 ?? ?? ?? ?? E8 ?? ?? ?? ?? 85 C0 74 ?? 83 F8 03 75 ?? 48 8B 8B ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 63 ?? 00 83 A3 ?? ?? ?? ?? 00 C6 83 ?? ?? ?? ?? 00 48 83 C4 20 5B C3 E8 ?? ?? ?? ?? 48 8B CB",
            criAtomExPlayer_SetAisacControlByName = "48 89 5C 24 ?? 57 48 83 EC 30 48 8B F9 0F 29 74 24 ?? 33 C9",
            criAtomExCategory_GetVolume = "40 53 48 83 EC 20 83 64 24 ?? 00",
            criAtomConfig_GetCategoryIndexById = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B F9 BE FF FF 00 00",
        },
        new("SMT5V-Win64-Shipping")
        {
            criAtomExPlayer_Create = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8B EC 48 83 EC 40 45 33 ED 41 8B F8",
            criAtomExPlayer_Start = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C8 FF",
            criAtomExPlayer_SetData = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 44 89 C6",
            criAtomExPlayer_SetFormat = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 48 8D 15",
            criAtomExPlayer_SetNumChannels = "48 89 5C 24 ?? 57 48 83 EC 20 89 D7 48 89 CB 48 85 C9 74 ?? 8D 42",
            criAtomExPlayer_SetSamplingRate = "48 89 5C 24 ?? 57 48 83 EC 20 89 D7 48 89 CB 48 85 C9 74 ?? 85 D2",
            criAtomExPlayer_SetCueName = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 49 8B F8 48 8B EA 48 8B D9 48 85 C9",
            criAtomExPlayer_SetCategoryById = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 50 48 8B F9 8B F2",

            criAtomExCategory_GetVolumeById = "40 53 48 83 EC 20 8B D9 33 C9 E8 ?? ?? ?? ?? 85 C0 75 ?? 48 8D 15 ?? ?? ?? ?? 33 C9 E8 ?? ?? ?? ?? F3 0F 10 05",
            criAtomExCategory_GetVolume = "40 53 48 83 EC 20 83 64 24 ?? 00",
            criAtomConfig_GetCategoryIndexById = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B F9 BE FF FF 00 00",
            criAtomExCategory_SetVolume = "40 53 48 83 EC 30 0F B7 D9",
        },
        new("p5r")
        {
            criAtomExPlayer_Create = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8B EC 48 83 EC 40 45 33 ED",
            criAtomExPlayer_Start = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C8 FF EB ?? E8 ?? ?? ?? ?? 33 D2",
            criAtomExPlayer_SetFormat = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 48 8D 15",
            criAtomExPlayer_SetNumChannels = "48 89 5C 24 ?? 57 48 83 EC 20 8B FA 48 8B D9 48 85 C9 74 ?? 8D 42",
            criAtomExPlayer_SetSamplingRate = "48 89 5C 24 ?? 57 48 83 EC 20 8B FA 48 8B D9 48 85 C9 74 ?? 85 D2",
            criAtomExPlayer_SetFile = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 49 8B F0 48 8B EA 48 8B F9",
            criAtomExPlayer_SetCueId = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 49 63 F8 48 8B F2 48 8B D9",
            criAtomExPlayer_SetCategoryById = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 50 48 8B F9 8B F2",
            criAtomExAcb_LoadAcbFile = "48 8B C4 48 89 58 ?? 48 89 68 ?? 48 89 70 ?? 48 89 78 ?? 41 54 41 56 41 57 48 83 EC 40 49 8B E9",

            criAtomExCategory_GetVolumeById = "40 53 48 83 EC 20 8B D9 33 C9 E8 ?? ?? ?? ?? 85 C0 75 ?? 48 8D 15 ?? ?? ?? ?? 33 C9 E8 ?? ?? ?? ?? F3 0F 10 05",
            criAtomExCategory_SetVolumeById = "40 53 48 83 EC 30 8B D9 0F 29 74 24 ?? 33 C9",
        },
    };

    public static CriAtomExPatterns GetGamePatterns(string game)
        => patterns.FirstOrDefault(x => x.Games.Contains(game, StringComparer.OrdinalIgnoreCase)) ?? new();
}

internal class CriAtomExPatterns
{
#pragma warning disable IDE1006 // Naming Styles
    public CriAtomExPatterns(params string[] games)
    {
        this.Games = games;
    }

    public string[] Games { get; }

    public string? criAtomExPlayer_GetNumPlayedSamples { get; init; }
    public string? criAtomExAcb_LoadAcbFile { get; init; }
    public string? criAtomExPlayer_SetCueId { get; init; }
    public string? criAtomExPlayer_Start { get; init; }
    public string? criAtomExPlayer_SetFile { get; init; }
    public string? criAtomExPlayer_SetFormat { get; init; }
    public string? criAtomExPlayer_SetSamplingRate { get; init; }
    public string? criAtomExPlayer_SetNumChannels { get; init; }
    public string? criAtomExCategory_GetVolumeById { get; init; }
    public string? criAtomExPlayer_SetVolume { get; init; }
    public string? criAtomExCategory_SetVolumeById { get; init; }
    public string? criAtomExPlayer_SetCategoryById { get; init; }
    public string? criAtomExPlayer_SetStartTime { get; init; }
    public string? CriAtomExPlayback_GetTimeSyncedWithAudio { get; init; }
    public string? criAtomExPlayer_Create { get; init; }
    public string? criAtomExPlayer_GetLastPlaybackId { get; init; }
    public string? criAtomExPlayer_SetCategoryByName { get; init; }
    public string? criAtomExPlayer_GetCategoryInfo { get; init; }
    public string? criAtomExPlayer_SetData { get; init; }
    public string? criAtomExPlayer_SetCueName { get; init; }
    public string? criAtomExPlayer_UpdateAll { get; init; }
    public string? criAtomExPlayer_LimitLoopCount { get; init; }
    public string? criAtomExPlayer_GetStatus { get; init; }
    public string? criAtomExPlayer_Stop { get; init; }
    public string? criAtomExPlayer_SetAisacControlByName { get; init; }
    public string? criAtomExCategory_GetVolume { get; init; }
    public string? criAtomConfig_GetCategoryIndexById { get; init; }
    public string? criAtomExCategory_SetVolume { get; init; }
    public string? criAtomExAcb_ExistsId { get; init; }
}
