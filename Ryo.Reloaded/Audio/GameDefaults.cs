using Ryo.Definitions.Enums;
using Ryo.Reloaded.Audio.Models;

namespace Ryo.Reloaded.Audio;

internal static class GameDefaults
{
    private static readonly Dictionary<string, AudioConfig> defaults = new(StringComparer.OrdinalIgnoreCase)
    {
        ["p3r"] = new()
        {
            AcbName = "bgm",
            Format = CriAtomFormat.HCA,
            NumChannels = 2,
            SampleRate = 44100,
            CategoryIds = new int[] { 0, 13 },
            PlayerId = 0,
            Volume = 0.15f,
        },
        ["SMT5V-Win64-Shipping"] = new()
        {
            AcbName = "bgm",
            Format = CriAtomFormat.HCA,
            NumChannels = 2,
            SampleRate = 44100,
            CategoryIds = new int[] { 0, 4, 9, 40, 24, 11, 43, 51 },
            PlayerId = -1,
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
}
