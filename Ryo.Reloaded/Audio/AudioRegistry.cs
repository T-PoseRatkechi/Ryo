using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Diagnostics.CodeAnalysis;
using Ryo.Interfaces.Types;
using Ryo.Interfaces;

namespace Ryo.Reloaded.Audio;

internal class AudioRegistry : IRyoApi
{
    private readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    private readonly Dictionary<string, AudioConfig> cueNameAudio = new(StringComparer.OrdinalIgnoreCase);
    private readonly string game;

    public AudioRegistry(string game)
    {
        this.game = game;
    }

    public void AddAudioFolder(string dir)
    {
        Log.Information($"Adding folder: {dir}");

        var dirConfigFile = Path.Join(dir, "config.yaml");
        var dirConfig = File.Exists(dirConfigFile) ? this.GetUserConfig(dirConfigFile) : null;
        if (dirConfig != null)
        {
            PrintUserConfig(dirConfig);
        }

        foreach (var file in Directory.EnumerateFiles(dir))
        {
            if (file.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            this.AddAudioFile(file, dirConfig);
        }

        foreach (var folder in Directory.EnumerateDirectories(dir))
        {
            this.AddAudioFolder(folder);
        }
    }

    public void AddAudioFile(string file)
        => this.AddAudioFile(file, null);

    public void AddAudioFile(string file, UserAudioConfig? folderConfig = null)
    {
        var audio = this.GetAudioConfig(file, folderConfig);
        if (string.IsNullOrEmpty(audio.CueName)) throw new Exception("Missing cue data.");
        this.cueNameAudio[audio.CueName] = audio;

        Log.Information($"Assigned cue.\nCue Name: {audio.CueName}\nFile: {audio.AudioFile}");
    }

    public bool TryGetAudio(PlayerConfig player, string cueName, [NotNullWhen(true)] out AudioConfig? audio)
    {
        return this.cueNameAudio.TryGetValue(cueName, out audio);
    }

    private AudioConfig GetAudioConfig(string file, UserAudioConfig? folderConfig = null)
    {
        var config = GameAudio.CreateDefaultConfig(this.game);
        if (folderConfig != null)
        {
            ApplyUserConfig(config, folderConfig);
        }

        // Load file config.
        var configFile = Path.ChangeExtension(file, ".yaml");
        if (File.Exists(configFile))
        {
            var fileConfig = this.GetUserConfig(configFile);
            ApplyUserConfig(config, fileConfig);
        }

        // Use file name for cue data if none set.
        if (string.IsNullOrEmpty(config.CueName))
        {
            config.CueName = GetCueName(file);
        }

        config.AudioFile = file;
        config.Format = GetAudioFormat(file);
        return config;
    }

    private UserAudioConfig GetUserConfig(string configFile)
        => this.deserializer.Deserialize<UserAudioConfig>(File.ReadAllText(configFile));

    private static void ApplyUserConfig(AudioConfig config, UserAudioConfig userConfig)
    {
        config.CueName = userConfig?.CueName ?? string.Empty;
        config.PlayerId = userConfig?.PlayerId ?? config.PlayerId;
        config.CategoryIds = userConfig?.CategoryIds ?? config.CategoryIds;
        config.NumChannels = userConfig?.NumChannels ?? config.NumChannels;
        config.SampleRate = userConfig?.SampleRate ?? config.SampleRate;
        config.Volume = userConfig?.Volume ?? config.Volume;
    }

    private static CriAtom_Format GetAudioFormat(string file)
        => Path.GetExtension(file).ToLower() switch
        {
            ".hca" => CriAtom_Format.HCA,
            ".adx" => CriAtom_Format.ADX,
            _ => throw new Exception("Unknown audio format.")
        };

    private static string GetCueName(string file)
    {
        var nameParts = Path.GetFileNameWithoutExtension(file).Split('_');
        if (nameParts.Length > 0 && int.TryParse(nameParts[0], out var bgmId))
        {
            return bgmId.ToString();
        }

        return Path.GetFileNameWithoutExtension(file);
    }

    private static void PrintUserConfig(UserAudioConfig config)
    {
        var categories = (config.CategoryIds != null) ? string.Join(',', config.CategoryIds.Select(x => x.ToString())) : "Not Set";
        Log.Debug(
            $"User Config\n" +
            $"Cue Name: {config.CueName ?? "Not Set"}\n" +
            $"Player ID: {config.PlayerId?.ToString() ?? "Not Set"}\n" +
            $"Categories: {categories}\n" +
            $"Volume: {config.Volume?.ToString() ?? "Not Set"}\n" +
            $"Sample Rate: {config.SampleRate?.ToString() ?? "Not Set"}\n" +
            $"Channels: {config.NumChannels?.ToString() ?? "Not Set"}");
    }
}
