namespace Ryo.Interfaces;

public interface IRyoUtils
{
    [Obsolete("Use ICriAtomRegistry.GetAcbByName")]
    string? GetAcbName(nint acbHn);

    [Obsolete("Use ICriAtomRegistry.GetAcbByHn")]
    nint GetAcbHn(string acbName);
}
