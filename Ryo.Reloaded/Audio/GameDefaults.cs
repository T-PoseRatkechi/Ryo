using Ryo.Interfaces;
using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio;

internal static class GameDefaults
{
    private static readonly Dictionary<string, AudioConfig> defaults = new(StringComparer.OrdinalIgnoreCase)
    {
        ["p5r"] = new()
        {
            //AcbName = "bgm",
            //CategoryIds = new int[] { 1, 8 },
        },
        ["p4g"] = new()
        {
            //AcbName = "snd00_bgm",
            //CategoryIds = new int[] { 6, 13 },
        },
        ["p3r"] = new()
        {
            AcbName = "bgm",
            //CategoryIds = new int[] { 0, 13 },
            Volume = 0.15f,
        },
        ["SMT5V-Win64-Shipping"] = new()
        {
            AcbName = "bgm",
            //CategoryIds = new int[] { 0, 4, 9, 40, 24, 11, 43, 51 },
            Volume = 0.35f,
        },
        ["likeadragon8"] = new()
        {
            //CategoryIds = new int[] { 11 },
            Volume = 0.35f,
        },
        ["likeadragongaiden"] = new()
        {
            //CategoryIds = new int[] { 11 },
            Volume = 0.35f,
        },
        ["LostJudgment"] = new()
        {
            //CategoryIds = new int[] { 11 },
            Volume = 0.35f,
        },
    };

    public static AudioConfig CreateDefaultConfig(string game)
    {
        if (defaults.TryGetValue(game, out var defaultConfig))
        {
            return defaultConfig.Clone();
        }

        return new();
    }

    public static void ConfigureCriAtom(string game, ICriAtomEx criAtomEx)
    {
        var normalizedGame = game.ToLower();
        switch (normalizedGame)
        {
            case "p5r":
                criAtomEx.SetPlayerConfigById(255, new()
                {
                    maxPathStrings = 2,
                    maxPath = 256,
                    enableAudioSyncedTimer = true,
                    updatesTime = true,
                });
                break;
            case "p4g":
                criAtomEx.SetPlayerConfigById(0, new()
                {
                    maxPathStrings = 2,
                    maxPath = 256,
                    enableAudioSyncedTimer = true,
                    updatesTime = true,
                });
                break;
            default: break;
        }
    }
}
