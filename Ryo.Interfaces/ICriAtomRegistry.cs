namespace Ryo.Interfaces;

/// <summary>
/// Registry for various CriAtom objects or data used by them. Commonly players, ACBs, audio buffers.
/// </summary>
public interface ICriAtomRegistry
{
    /// <summary>
    /// Gets player by its ID.
    /// </summary>
    /// <param name="playerId">ID of player to get.</param>
    /// <returns>Player, if it's registered</returns>
    Player? GetPlayerById(nint playerId);

    /// <summary>
    /// Gets player by its handle.
    /// </summary>
    /// <param name="playerHn">Handle of player to get.</param>
    /// <returns>Player, if it's registered</returns>
    Player? GetPlayerByHn(int playerHn);

    /// <summary>
    /// Gets ACB by its name.
    /// </summary>
    /// <param name="acbName">Name of ACB to get.</param>
    /// <returns>The ACB, if it's registered.</returns>
    Acb? GetAcbByName(string acbName);

    /// <summary>
    /// Gets ACB by its handle.
    /// </summary>
    /// <param name="acbHn">Handle of ACB to get.</param>
    /// <returns>The ACB, if it's registered.</returns>
    Acb? GetAcbByHn(nint acbHn);
}

public record Player(int Id, nint Handle);

public record Acb(string Name, nint Handle);