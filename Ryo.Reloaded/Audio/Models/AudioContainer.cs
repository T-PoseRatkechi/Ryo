using Ryo.Definitions.Enums;

namespace Ryo.Reloaded.Audio.Models;

internal class AudioContainer
{
    private readonly AudioConfig config;
    private readonly List<string> files = new();

    public AudioContainer(AudioConfig? config = null)
    {
        this.config = config ?? new();
    }

    public string CueName => this.config.CueName ?? string.Empty;

    public string AcbName => this.config.AcbName ?? string.Empty;

    public int SampleRate => this.config.SampleRate ?? 44100;

    public CriAtomFormat Format => this.config.Format ?? CriAtomFormat.HCA;

    public int NumChannels => this.config.NumChannels ?? 2;

    public int PlayerId => this.config.PlayerId ?? -1;

    public int[] CategoryIds => this.config.CategoryIds ?? Array.Empty<int>();

    public float Volume => this.config.Volume ?? -1f;

    public string? Key => this.config.Key ?? null;

    public string[] Tags => this.config.Tags ?? Array.Empty<string>();

    public void AddFile(string filePath)
    {
        this.files.Add(filePath);
        Log.Information($"Audio: {this.CueName} / {this.AcbName}\nFile added: {filePath}");
    }

    public string GetAudioFile()
    {
        if (this.files.Count > 1)
        {
            var randomIndex = Random.Shared.Next(0, this.files.Count);
            Log.Debug($"Audio: {this.CueName} / {this.AcbName} || Random Index: {randomIndex} || Total Files: {this.files.Count}");
            return this.files[randomIndex];
        }
        else if (this.files.Count == 1)
        {
            return this.files[0];
        }

        throw new Exception($"Audio had no files.\nCue: {this.CueName} || ACB: {this.AcbName}");
    }

    public string[] GetContainerFiles() => this.files.ToArray();
}
