using Ryo.Interfaces;
using Ryo.Reloaded.Audio;

namespace Ryo.Reloaded;

internal class RyoUtils : IRyoUtils
{
    public string? GetAcbName(nint acbHn)
        => AcbRegistry.GetAcbName(acbHn);
}
