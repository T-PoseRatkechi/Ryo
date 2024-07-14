using Ryo.Reloaded.Audio.Models;
using System.Runtime.InteropServices;

namespace Ryo.Reloaded.Audio;

internal static class AudioCache
{
    private static readonly Dictionary<string, AudioBuffer> cachedAudioData = new(StringComparer.OrdinalIgnoreCase);

    public static AudioBuffer GetAudioData(string audioFile)
    {
        if (cachedAudioData.TryGetValue(audioFile, out var existingData))
        {
            return existingData;
        }

        var data = File.ReadAllBytes(audioFile);
        var buffer = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, buffer, data.Length);
        cachedAudioData[audioFile] = new(buffer, data.Length);

        Log.Debug($"Loading audio: {audioFile}");
        return cachedAudioData[audioFile];
    }
}
