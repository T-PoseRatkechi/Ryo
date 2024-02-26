using Reloaded.Hooks.Definitions;
using Ryo.Reloaded.CRI.CriUnreal;
using SharedScans.Interfaces;
using static Ryo.Reloaded.CRI.Mana.CriManaFunctions;

namespace Ryo.Reloaded.CRI.Mana;

internal partial class CriMana
{
    private readonly string game;
    private readonly CriManaPatterns patterns;
    private IFunction<criManaPlayer_SetFile>? setFile;

    public CriMana(ISharedScans scans, string game)
    {
        this.game = game;
        this.patterns = CriManaGames.GetGamePatterns(game);

        scans.AddScan<criManaPlayer_SetFile>(this.patterns.criManaPlayer_SetFile);

        //ScanHooks.Add(
        //    nameof(criManaPlayer_SetFile),
        //    "4C 89 44 24 ?? 48 89 54 24 ?? 48 89 4C 24 ?? 48 83 EC 38 48 83 7C 24 ?? 00 75 ?? 41 B8 FE FF FF FF 48 8D 15 ?? ?? ?? ?? 31 C9",
        //    (hooks, result) => this.setFile = hooks.CreateFunction<criManaPlayer_SetFile>(result));
    }
}
