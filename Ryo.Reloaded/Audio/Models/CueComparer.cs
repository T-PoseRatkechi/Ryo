using System.Diagnostics.CodeAnalysis;

namespace Ryo.Reloaded.Audio.Models;

internal class CueComparer : IEqualityComparer<CueKey>
{
    public static readonly CueComparer Instance = new();

    public bool Equals(CueKey? x, CueKey? y)
        => (x?.CueName.Equals(y?.CueName, StringComparison.OrdinalIgnoreCase) ?? false)
        && (x?.AcbName.Equals(y?.AcbName, StringComparison.OrdinalIgnoreCase) ?? false);

    public int GetHashCode([DisallowNull] CueKey obj)
        => $"{obj.CueName.ToLower()} {obj.AcbName.ToLower()}".GetHashCode();
}
