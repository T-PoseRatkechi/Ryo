using Ryo.Interfaces;
using Ryo.Reloaded.Audio.Models.Containers;
using Ryo.Reloaded.CRI.CriAtomEx;
using static Ryo.Definitions.Functions.CriAtomExFunctions;

namespace Ryo.Reloaded.Audio.Services;

internal unsafe class RyoService
{
    private readonly ICriAtomEx criAtomEx;
    private readonly ICriAtomRegistry criAtomRegistry;
    private readonly Dictionary<nint, CategoryVolume> modifiedCategories = new();
    private readonly bool useSetFile;

    public RyoService(string game, ICriAtomEx criAtomEx, ICriAtomRegistry criAtomRegistry)
    {
        this.criAtomEx = criAtomEx;
        this.criAtomRegistry = criAtomRegistry;

        var patterns = CriAtomExGames.GetGamePatterns(game);
        if (patterns.criAtomExPlayer_SetFile != null)
        {
            Log.Debug($"New audio uses: {nameof(criAtomExPlayer_SetFile)}");
            this.useSetFile = true;
        }
        else
        {
            Log.Debug($"New audio uses: {nameof(criAtomExPlayer_SetData)}");
        }
    }

    public void SetAudio(Player player, BaseContainer container, int[]? categories)
    {
        var currentPlayer = player;

        var manualStart = false;
        if (container.PlayerId != -1
            && currentPlayer.Id != container.PlayerId
            && this.criAtomRegistry.GetPlayerById(container.PlayerId) is Player newPlayer)
        {
            currentPlayer = newPlayer;
            manualStart = true;
        }

        var newAudio = container.GetAudio();

        if (this.useSetFile)
        {
            this.criAtomEx.Player_SetFile(currentPlayer.Handle, IntPtr.Zero, (byte*)StringsCache.GetStringPtr(newAudio.FilePath));
        }
        else
        {
            var audioData = AudioCache.GetAudioData(newAudio.FilePath);
            this.criAtomEx.Player_SetData(currentPlayer.Handle, (byte*)audioData.Address, audioData.Size);
        }

        this.criAtomEx.Player_SetFormat(currentPlayer.Handle, newAudio.Format);
        this.criAtomEx.Player_SetSamplingRate(currentPlayer.Handle, newAudio.SampleRate);
        this.criAtomEx.Player_SetNumChannels(currentPlayer.Handle, newAudio.NumChannels);

        // Use first category for setting custom volume.
        if (categories != null)
        {
            Log.Debug($"Categories: {string.Join(", ", categories.Select(x => $"{x}"))}");
            int volumeCategory = categories.Length > 0 ? categories[0] : -1;
            if (volumeCategory > -1 && newAudio.Volume >= 0 && !this.modifiedCategories.ContainsKey(currentPlayer.Handle))
            {
                var currentVolume = this.criAtomEx.Category_GetVolumeById((uint)volumeCategory);
                this.modifiedCategories[currentPlayer.Handle] = new CategoryVolume(currentPlayer.Id, volumeCategory, currentVolume);
                this.criAtomEx.Category_SetVolumeById((uint)volumeCategory, newAudio.Volume);
                Log.Debug($"Modified volume. Player ID: {currentPlayer.Id} || Category ID: {volumeCategory} || Volume: {newAudio.Volume}");
            }

            // Apply categories.
            foreach (var id in categories)
            {
                this.criAtomEx.Player_SetCategoryById(currentPlayer.Handle, (uint)id);
            }
        }

        if (manualStart)
        {
            this.criAtomEx.Player_Start(currentPlayer.Handle);
            Log.Debug($"Manually started player with ID: {currentPlayer.Id}");
        }

        Log.Debug($"Redirected {container.Name}\nFile: {newAudio.FilePath}");
    }

    public void ResetPlayerVolume(nint playerHn)
    {
        // Reset modified category volume.
        // Limited to one modified category per player.
        if (this.modifiedCategories.TryGetValue(playerHn, out var modifiedVolume))
        {
            Log.Debug($"Reseting volume. Player ID: {modifiedVolume.PlayerId} || Category ID: {modifiedVolume.CategoryId}");
            this.criAtomEx.Category_SetVolumeById((uint)modifiedVolume.CategoryId, modifiedVolume.OriginalVolume);
            this.modifiedCategories.Remove(playerHn);
        }
    }

    private record CategoryVolume(int PlayerId, int CategoryId, float OriginalVolume);
}
