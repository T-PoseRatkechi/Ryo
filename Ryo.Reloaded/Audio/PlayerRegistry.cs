using Ryo.Interfaces;

namespace Ryo.Reloaded.Audio;

internal static class PlayerRegistry
{
    private static readonly List<Player> players = new();
    private static readonly Dictionary<nint, PlayerInfo> playersInfo = new();

    public static Player Register(nint playerHn)
    {
        var player = new Player(players.Count, playerHn);
        players.Add(player);
        return player;
    }

    public static void SetPlayerInfo(nint playerHn, nint acbHn, string cueName)
    {
        var player = players.FirstOrDefault(x => x.PlayerHn == playerHn) ?? Register(playerHn);
        var acb = AcbRegistry.GetAcbName(acbHn) ?? "Unknown";
        playersInfo[playerHn] = new(player, new(acb, acbHn), cueName);
    }
}
