using Reloaded.Hooks.Definitions.X64;

namespace Ryo.Definitions.Functions;

public static class CriManaFunctions
{
    [Function(CallingConventions.Microsoft)]
    public delegate void criManaPlayer_SetFile(nint player, nint binder, nint path);
}
