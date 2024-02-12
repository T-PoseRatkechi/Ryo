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

    private readonly Dictionary<string, FileAudioConfig> cueNameAudio = new(StringComparer.OrdinalIgnoreCase);
    private readonly string game;

    public AudioRegistry(string game)
    {
        this.game = game;
    }

    public void AddAudioFolder(string dir)
    {
        Log.Information($"Adding folder: {dir}");

        var dirConfigFile = Path.Join(dir, "config.yaml");
        var dirConfig = File.Exists(dirConfigFile) ? this.GetFolderConfig(dirConfigFile) : null;
        if (dirConfig != null)
        {
            PrintConfig(dirConfig);
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

    public void AddAudioFile(string file, FolderAudioConfig? baseConfig = null)
    {
        var fileConfig = this.GetFileAudioConfig(file, baseConfig);
        if (string.IsNullOrEmpty(fileConfig.CueName)) throw new Exception("Missing cue data.");
        this.cueNameAudio[fileConfig.CueName] = fileConfig;

        Log.Information($"Assigned cue.\nCue Name: {fileConfig.CueName}\nFile: {fileConfig.AudioFile}");
    }

    public bool TryGetAudio(PlayerConfig player, string cueName, [NotNullWhen(true)] out FileAudioConfig? audio)
    {
        return this.cueNameAudio.TryGetValue(cueName, out audio);
    }

    private FileAudioConfig GetFileAudioConfig(string file, FolderAudioConfig? baseConfig = null)
    {
        var defaultConfig = GameAudio.GetDefaultConfig(this.game);
        var config = new FileAudioConfig()
        {
            PlayerId = baseConfig?.PlayerId ?? defaultConfig.PlayerId,
            CategoryIds = baseConfig?.CategoryIds ?? defaultConfig.CategoryIds,
            NumChannels = baseConfig?.NumChannels ?? defaultConfig.NumChannels,
            SampleRate = baseConfig?.SampleRate ?? defaultConfig.SampleRate,
            Volume = baseConfig?.Volume ?? defaultConfig.Volume,
        };

        // Load cue data from config.
        var configFile = Path.ChangeExtension(file, ".yaml");
        if (File.Exists(configFile))
        {
            config = this.GetFileConfig(configFile);
        }

        // Use file name for cue data if none set.
        if (string.IsNullOrEmpty(config.CueName))
        {
            config.CueName = Path.GetFileNameWithoutExtension(file);
        }

        config.AudioFile = file;
        config.Format = GetAudioFormat(file);
        return config;
    }

    private FileAudioConfig GetFileConfig(string configFile)
        => this.deserializer.Deserialize<FileAudioConfig>(File.ReadAllText(configFile));

    private FolderAudioConfig GetFolderConfig(string configFile)
        => this.deserializer.Deserialize<FolderAudioConfig>(File.ReadAllText(configFile));

    private static CriAtom_Format GetAudioFormat(string file)
        => Path.GetExtension(file).ToLower() switch
        {
            ".hca" => CriAtom_Format.HCA,
            ".adx" => CriAtom_Format.ADX,
            _ => throw new Exception("Unknown audio format.")
        };

    private static void PrintConfig(FolderAudioConfig config)
    {
        var categories = (config.CategoryIds != null) ? string.Join(',', config.CategoryIds.Select(x => x.ToString())) : "Not Set";
        Log.Debug(
            $"Folder Config\n" +
            $"Player ID: {config.PlayerId?.ToString() ?? "Not Set"}\n" +
            $"Categories: {categories}\n" +
            $"Volume: {config.Volume?.ToString() ?? "Not Set"}\n" +
            $"Sample Rate: {config.SampleRate?.ToString() ?? "Not Set"}\n" +
            $"Channels: {config.NumChannels?.ToString() ?? "Not Set"}");
    }
}
