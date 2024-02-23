using System.Diagnostics.CodeAnalysis;

namespace Ryo.Reloaded.Audio.Models;

internal class CueComparer : IEqualityComparer<Cue>
{
    public static readonly CueComparer Instance = new();

    public bool Equals(Cue? x, Cue? y)
        => (x?.CueName.Equals(y?.CueName, StringComparison.OrdinalIgnoreCase) ?? false)
        && (x?.AcbName.Equals(y?.AcbName, StringComparison.OrdinalIgnoreCase) ?? false);

    public int GetHashCode([DisallowNull] Cue obj)
        => $"{obj.CueName.ToLower()} {obj.AcbName.ToLower()}".GetHashCode();
}
