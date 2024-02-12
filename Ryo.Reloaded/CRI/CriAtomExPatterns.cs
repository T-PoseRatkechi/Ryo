namespace Ryo.Reloaded.CRI;

internal static class CriAtomExGames
{
    private static readonly CriAtomExPatterns[] patterns = new CriAtomExPatterns[]
    {
        new("p3r")
        {
            CriAtomExPlayer_Create = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8B EC 48 83 EC 40 45 33 ED 41 8B F8",
            CriAtomExPlayer_Start = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C8 FF",
            CriAtomExPlayer_SetData = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 44 89 C6",
            CriAtomExPlayer_SetFormat = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 48 8D 15",
            CriAtomExPlayer_SetNumChannels = "48 89 5C 24 ?? 57 48 83 EC 20 89 D7 48 89 CB 48 85 C9 74 ?? 8D 42",
            CriAtomExPlayer_SetSamplingRate = "48 89 5C 24 ?? 57 48 83 EC 20 89 D7 48 89 CB 48 85 C9 74 ?? 85 D2",
            CriAtomExPlayer_SetCategoryById = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 50 48 8B F9 8B F2",
            CriAtomExPlayer_SetCueName = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 49 8B F8 48 8B EA 48 8B D9",
            CriAtomExPlayer_SetVolume = "48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8B 89 ?? ?? ?? ?? 33 D2",
            CriAtomExCategory_SetVolumeById = "40 53 48 83 EC 30 8B D9 0F 29 74 24",
            CriAtomExPlayer_UpdateAll = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 48 85 C9 75 ?? 44 8D 41 ?? 48 8D 15 ?? ?? ?? ?? E8 ?? ?? ?? ?? EB",
        }
    };

    public static CriAtomExPatterns GetGamePatterns(string game)
        => patterns.First(x => x.Games.Contains(game, StringComparer.OrdinalIgnoreCase));
}

internal class CriAtomExPatterns
{
    public CriAtomExPatterns(params string[] games)
    {
        this.Games = games;
    }

    public string[] Games { get; }

    public string? CriAtomExPlayer_GetNumPlayedSamples { get; init; }

    public string? CriAtomExAcb_LoadAcbFile { get; init; }

    public string? CriAtomExPlayer_SetCueId { get; init; }

    public string? CriAtomExPlayer_Start { get; init; }

    public string? CriAtomExPlayer_SetFile { get; init; }

    public string? CriAtomExPlayer_SetFormat { get; init; }

    public string? CriAtomExPlayer_SetSamplingRate { get; init; }

    public string? CriAtomExPlayer_SetNumChannels { get; init; }

    public string? CriAtomExCategory_GetVolumeById { get; init; }

    public string? CriAtomExPlayer_SetVolume { get; init; }

    public string? CriAtomExCategory_SetVolumeById { get; init; }

    public string? CriAtomExPlayer_SetCategoryById { get; init; }

    public string? CriAtomExPlayer_SetStartTime { get; init; }

    public string? CriAtomExPlayback_GetTimeSyncedWithAudio { get; init; }

    public string? CriAtomExPlayer_Create { get; init; }

    public string? CriAtomExPlayer_GetLastPlaybackId { get; init; }

    public string? CriAtomExPlayer_SetCategoryByName { get; init; }

    public string? CriAtomExPlayer_GetCategoryInfo { get; init; }

    public string? CriAtomExPlayer_SetData { get; init; }

    public string? CriAtomExPlayer_SetCueName { get; init; }

    public string? CriAtomExPlayer_UpdateAll { get; init; }
}
