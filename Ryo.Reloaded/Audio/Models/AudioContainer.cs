﻿using Ryo.Definitions.Enums;
using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio.Models;

internal class AudioContainer
{
    private readonly AudioConfig config;

    public AudioContainer(string filePath, AudioConfig? config = null)
    {
        this.FilePath = filePath;
        this.config = config ?? new();
    }

    public string FilePath { get; }

    public int SampleRate => this.config.SampleRate ?? 44100;

    public CriAtomFormat Format => this.config.Format ?? CriAtomFormat.HCA;

    public int NumChannels => this.config.NumChannels ?? 2;

    public float Volume => this.config.Volume ?? -1f;

    public string? Key => this.config.Key ?? null;

    public string[] Tags => this.config.Tags ?? Array.Empty<string>();
}
