using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio.Models;

internal class CueContainer
{
    private readonly List<AudioContainer> audios = new();

    public CueContainer(string cueName, string acbName, AudioConfig? config = null)
    {
        if (string.IsNullOrEmpty(cueName))
        {
            throw new ArgumentException($"'{nameof(cueName)}' cannot be null or empty.", nameof(cueName));
        }

        if (string.IsNullOrEmpty(acbName))
        {
            throw new ArgumentException($"'{nameof(acbName)}' cannot be null or empty.", nameof(acbName));
        }

        this.CueName = cueName;
        this.AcbName = acbName;
        this.PlayerId = config?.PlayerId ?? -1;
        this.CategoryIds = config?.CategoryIds ?? Array.Empty<int>();
        this.SharedContainerId = config?.SharedContainerId;
    }

    public string CueName { get; }

    public string AcbName { get; }

    public int PlayerId { get; }

    public int[] CategoryIds { get; }

    public string? SharedContainerId { get; }

    public void AddAudio(AudioContainer audio)
    {
        this.audios.Add(audio);
        Log.Information($"Cue: {this.CueName} / {this.AcbName}\nFile added: {audio.FilePath}");
    }

    public AudioContainer GetAudio()
    {
        if (this.audios.Count > 1)
        {
            var randomIndex = Random.Shared.Next(0, this.audios.Count);
            Log.Debug($"Cue: {this.CueName} / {this.AcbName} || Random Index: {randomIndex} || Total Files: {this.audios.Count}");
            return this.audios[randomIndex];
        }
        else if (this.audios.Count == 1)
        {
            return this.audios[0];
        }

        throw new Exception($"Audio had no files.\nCue: {this.CueName} || ACB: {this.AcbName}");
    }

    public string[] GetContainerFiles() => this.audios.Select(x => x.FilePath).ToArray();
}
