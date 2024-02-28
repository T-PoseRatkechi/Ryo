namespace Ryo.Definitions.Classes;

public class PlayerConfig
{
    public PlayerConfig(int id, nint playerHn)
    {
        this.Id = id;
        this.PlayerHn = playerHn;
    }

    public int Id { get; init; }

    public nint PlayerHn { get; init; }

    public AcbConfig Acb { get; set; } = new();
}
