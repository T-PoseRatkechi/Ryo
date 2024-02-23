using Reloaded.Hooks.Definitions.X64;

namespace Ryo.Reloaded.P3R;

/// <summary>
/// Re-add getting category volume by ID.
/// </summary>
internal class VolumeGetterAlt
{
    private GetCategoryIndexFromId? getCategoryIndex;
    private GetCategoryVolumeByIndex? getCategoryVolume;

    [Function(CallingConventions.Microsoft)]
    private delegate ushort GetCategoryIndexFromId(uint id);

    [Function(CallingConventions.Microsoft)]
    private delegate float GetCategoryVolumeByIndex(ushort index);

    public VolumeGetterAlt()
    {
        ScanHooks.Add(
            nameof(GetCategoryVolumeByIndex),
            "40 53 48 83 EC 20 83 64 24 ?? 00",
            (hooks, result) => this.getCategoryVolume = hooks.CreateWrapper<GetCategoryVolumeByIndex>(result, out _));

        ScanHooks.Add(
            nameof(GetCategoryIndexFromId),
            "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B F9 BE FF FF 00 00",
            (hooks, result) => this.getCategoryIndex = hooks.CreateWrapper<GetCategoryIndexFromId>(result, out _));
    }

    public float CriAtomExCategory_GetVolumeById(uint id)
     => this.GetCategoryVolumeByIndexImpl(this.GetCategoryIndexById(id));

    private ushort GetCategoryIndexById(uint id) => this.getCategoryIndex!(id);

    private float GetCategoryVolumeByIndexImpl(ushort index) => this.getCategoryVolume!(index);
}
