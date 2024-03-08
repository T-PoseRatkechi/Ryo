using Ryo.Interfaces;
using Ryo.Reloaded.Audio;

namespace Ryo.Reloaded;

internal class RyoUtils : IRyoUtils
{
    public nint GetAcbHn(string acbName)
        => AcbRegistry.GetAcbHn(acbName);

    public string? GetAcbName(nint acbHn)
        => AcbRegistry.GetAcbName(acbHn);

    public PlayerInfo GetPlayerInfo(nint playerHn)
        => throw new NotImplementedException();
}
