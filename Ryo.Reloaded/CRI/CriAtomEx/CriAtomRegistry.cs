using Ryo.Definitions.Structs;
using Ryo.Interfaces;

namespace Ryo.Reloaded.CRI.CriAtomEx;

internal class CriAtomRegistry : ICriAtomRegistry
{
    private static readonly Dictionary<nint, Player> players = new();
    private static readonly Dictionary<nint, Acb> acbs = new();

    public static Player RegisterPlayer(nint playerHn)
    {
        var player = new Player(players.Count, playerHn);
        players[player.Handle] = player;
        Log.Debug($"Registered Player || ID: {player.Id} || Handle: {player.Handle:X}");
        return player;
    }

    public unsafe static Acb RegisterAcb(AcbHn* acbHn)
    {
        var acb = new Acb(acbHn->GetAcbName(), (nint)acbHn);
        acbs[acb.Handle] = acb;
        Log.Debug($"Registered ACB || Name: {acb.Name} || Handle: {acb.Handle:X}");
        return acb;
    }

    public Acb? GetAcbByHn(nint acbHn)
    {
        if (acbs.TryGetValue(acbHn, out var acbName))
        {
            return acbName;
        }

        Log.Warning($"Unknown ACB Hn: {acbHn:X}");
        return null;
    }

    public Acb? GetAcbByName(string acbName)
    {
        var existingAcb = acbs.Values.FirstOrDefault(x => x.Name.Equals(acbName, StringComparison.OrdinalIgnoreCase));
        if (existingAcb != null)
        {
            return existingAcb;
        }

        Log.Warning($"Unknown ACB: {acbName}");
        return null;
    }

    public Player? GetPlayerByHn(int playerHn)
    {
        if (players.TryGetValue(playerHn, out var player))
        {
            return player;
        }

        Log.Warning($"Unknown Player Hn: {playerHn:X}");
        return null;
    }

    public Player? GetPlayerById(nint playerId)
    {
        var player = players.Values.FirstOrDefault(x => x.Id == playerId);
        if (player == null)
        {
            Log.Warning($"Unknown Player ID: {playerId}");
        }

        return null;
    }
}
