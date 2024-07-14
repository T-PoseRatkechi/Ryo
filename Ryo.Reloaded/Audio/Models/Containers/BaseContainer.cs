using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio.Models.Containers;

internal abstract class BaseContainer
{
    private readonly List<RyoAudio> audios = new();

    public BaseContainer(AudioConfig? config)
    {
        PlayerId = config?.PlayerId ?? -1;
        CategoryIds = config?.CategoryIds ?? Array.Empty<int>();
        SharedContainerId = config?.SharedContainerId;
    }

    protected abstract string InternalName { get; }

    public int PlayerId { get; }

    public int[] CategoryIds { get; }

    public string? SharedContainerId { get; }

    public void AddAudio(RyoAudio audio)
    {
        audios.Add(audio);
        Log.Information($"{InternalName}\nFile added: {audio.FilePath}");
    }

    public RyoAudio GetAudio()
    {
        if (audios.Count > 1)
        {
            var randomIndex = Random.Shared.Next(0, audios.Count);
            Log.Debug($"{InternalName} || Random Index: {randomIndex} || Total Files: {audios.Count}");
            return audios[randomIndex];
        }
        else if (audios.Count == 1)
        {
            return audios[0];
        }

        throw new Exception($"Audio had no files.\n{InternalName}");
    }

    public string[] GetContainerFiles() => audios.Select(x => x.FilePath).ToArray();
}
