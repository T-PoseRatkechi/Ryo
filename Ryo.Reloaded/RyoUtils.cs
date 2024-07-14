using Ryo.Interfaces;

namespace Ryo.Reloaded;

internal class RyoUtils : IRyoUtils
{
    private readonly ICriAtomRegistry registry;

    public RyoUtils(ICriAtomRegistry criAtomRegistry)
    {
        this.registry = criAtomRegistry;
    }

    public nint GetAcbHn(string acbName)
        => this.registry.GetAcbByName(acbName)?.Handle ?? 0;

    public string? GetAcbName(nint acbHn)
        => this.registry.GetAcbByHn(acbHn)?.Name;
}
