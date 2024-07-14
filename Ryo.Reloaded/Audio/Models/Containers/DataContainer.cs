﻿using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio.Models.Containers;

internal class DataContainer : BaseContainer
{
    public DataContainer(string audioDataName, AudioConfig? config = null)
        : base(config)
    {
        if (string.IsNullOrEmpty(audioDataName))
        {
            throw new ArgumentException($"'{nameof(audioDataName)}' cannot be null or empty.", nameof(audioDataName));
        }

        this.DataName = audioDataName;
        this.InternalName = $"Audio Data: {this.DataName}";
    }

    public string DataName { get; }

    protected override string InternalName { get; }
}
