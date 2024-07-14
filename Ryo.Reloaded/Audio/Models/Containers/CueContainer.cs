using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio.Models.Containers;

internal class CueContainer : BaseContainer
{
    public CueContainer(string cueName, string acbName, AudioConfig? config = null)
        : base(config)
    {
        if (string.IsNullOrEmpty(cueName))
        {
            throw new ArgumentException($"'{nameof(cueName)}' cannot be null or empty.", nameof(cueName));
        }

        if (string.IsNullOrEmpty(acbName))
        {
            throw new ArgumentException($"'{nameof(acbName)}' cannot be null or empty.", nameof(acbName));
        }

        CueName = cueName;
        AcbName = acbName;
        Name = $"Cue: {CueName} / {AcbName}";
    }

    public string CueName { get; }

    public string AcbName { get; }

    public override string Name { get; }
}
