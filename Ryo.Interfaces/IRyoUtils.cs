namespace Ryo.Interfaces;

public interface IRyoUtils
{
    string? GetAcbName(nint acbHn);

    nint GetAcbHn(string acbName);

    PlayerInfo GetPlayerInfo(nint playerHn);
}

public record PlayerInfo(Player Player, Acb Acb, string CueName);

public record Player(int PlayerId, nint PlayerHn);

public record Acb(string AcbName, nint AcbHn);

public record PlayerAudio(nint Id);