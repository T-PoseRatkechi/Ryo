namespace Ryo.Interfaces;

/// <summary>
/// Registry for various CriAtom objects or data used by them. Commonly players, ACBs, audio buffers.
/// </summary>
public interface ICriAtomRegistry
{
    /// <summary>
    /// Register audio in memory with a name.
    /// Allows for redirection from <c>CriAtomExPlayer.SetData</c>.
    /// </summary>
    /// <param name="address">Audio data address.</param>
    /// <param name="name">Name to assign to audio.</param>
    void RegisterAudioData(nint address, string name);

    /// <summary>
    /// Gets audio data by its name.
    /// </summary>
    /// <param name="name">Name of audio data to get.</param>
    /// <returns>Audio data, if found.</returns>
    AudioData? GetAudioDataByName(string name);

    /// <summary>
    /// Gets audio data by its address.
    /// </summary>
    /// <param name="address">Address of audio data to get.</param>
    /// <returns>Audio data, if found.</returns>
    AudioData? GetAudioDataByAddress(nint address);

    /// <summary>
    /// Gets player by its ID.
    /// </summary>
    /// <param name="playerId">ID of player to get.</param>
    /// <returns>Player, if found.</returns>
    Player? GetPlayerById(int playerId);

    /// <summary>
    /// Gets player by its handle.
    /// </summary>
    /// <param name="playerHn">Handle of player to get.</param>
    /// <returns>Player, if found.</returns>
    Player? GetPlayerByHn(nint playerHn);

    /// <summary>
    /// Gets ACB by its name.
    /// </summary>
    /// <param name="acbName">Name of ACB to get.</param>
    /// <returns>The ACB, if found.</returns>
    Acb? GetAcbByName(string acbName);

    /// <summary>
    /// Gets ACB by its handle.
    /// </summary>
    /// <param name="acbHn">Handle of ACB to get.</param>
    /// <returns>The ACB, if found.</returns>
    Acb? GetAcbByHn(nint acbHn);

    /// <summary>
    /// Gets AWB by its file path.
    /// </summary>
    /// <param name="awbPath">AWB file path.</param>
    /// <returns>The AWB, if found.</returns>
    Awb? GetAwbByPath(string awbPath);

    /// <summary>
    /// Gets AWB by its handle.
    /// </summary>
    /// <param name="awbHn">Handle of AWB to get.</param>
    /// <returns>The AWB, if found.</returns>
    Awb? GetAwbByHn(nint awbHn);
}

public record Player(int Id, nint Handle);

public record Acb(string Name, nint Handle);

public record Awb(string Path,  nint Handle);

public record AudioData(string Name, nint Address);