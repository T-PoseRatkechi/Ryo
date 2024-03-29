﻿using Ryo.Definitions.Enums;
using Ryo.Reloaded.Audio.Models;

namespace Ryo.Reloaded.Audio;

internal static class GameDefaults
{
    private static readonly Dictionary<string, Func<AudioConfig>> defaults = new()
    {
        ["p3r"] = () => new()
        {
            AcbName = "bgm",
            Format = CriAtomFormat.HCA,
            NumChannels = 2,
            SampleRate = 44100,
            CategoryIds = new int[] { 0, 13 },
            PlayerId = 0,
            Volume = 0.15f,
        },
    };

    public static AudioConfig CreateDefaultConfig(string game)
    {
        if (defaults.TryGetValue(game, out var defaultConfig))
        {
            return defaultConfig();
        }

        return new();
    }
}
