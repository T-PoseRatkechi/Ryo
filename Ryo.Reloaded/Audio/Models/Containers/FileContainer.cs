using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio.Models.Containers;

internal class FileContainer : BaseContainer
{
    public FileContainer(string filePath, AudioConfig? config = null)
        : base(config)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
        }

        this.FilePath = filePath;
        this.InternalName = $"Audio File: {this.FilePath}";
    }

    public string FilePath { get; }

    protected override string InternalName { get; }
}
