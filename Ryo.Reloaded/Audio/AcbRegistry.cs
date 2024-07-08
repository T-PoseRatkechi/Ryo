namespace Ryo.Reloaded.Audio;

internal static class AcbRegistry
{
    private static readonly Dictionary<nint, string> acbs = new();

    /// <summary>
    /// Register an ACB with its ACB handle.
    /// </summary>
    /// <param name="acbName">The name to associate the handle with, either cue sheet name or file.</param>
    /// <param name="acbHn">ACB handle.</param>
    public static void Register(string acbName, nint acbHn)
    {
        acbs[acbHn] = acbName;
        Log.Debug($"Registered ACB: {acbName} || ACB Hn: {acbHn:X}");
    }

    /// <summary>
    /// Get the name associated with the ACB handle.
    /// </summary>
    /// <param name="acbHn">ACB handle.</param>
    /// <returns>ACB name of the given handle.</returns>
    public static string? GetAcbName(nint acbHn)
    {
        if (acbs.TryGetValue(acbHn, out var acbName))
        {
            return acbName;
        }

        Log.Debug($"Unknown ACB Hn: {acbHn:X}");
        return null;
    }

    public static nint GetAcbHn(string acbName)
    {
        if (acbs.ContainsValue(acbName))
        {
            var acb = acbs.First(x => x.Value == acbName);
            return acb.Key;
        }

        Log.Warning($"Unknown ACB: {acbName}");
        return IntPtr.Zero;
    }
}
