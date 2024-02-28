using Ryo.Reloaded.CRI.CriUnreal;
using SharedScans.Interfaces;
using static Ryo.Definitions.Functions.CriManaFunctions;

namespace Ryo.Reloaded.CRI.Mana;

internal partial class CriMana
{
    private readonly CriManaPatterns patterns;

    public CriMana(ISharedScans scans, string game)
    {
        this.patterns = CriManaGames.GetGamePatterns(game);
        scans.AddScan<criManaPlayer_SetFile>(this.patterns.criManaPlayer_SetFile);
    }
}
