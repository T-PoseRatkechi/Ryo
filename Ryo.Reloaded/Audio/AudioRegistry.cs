using Ryo.Reloaded.CRI;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Diagnostics.CodeAnalysis;
using Ryo.Reloaded.CRI.Types;

namespace Ryo.Reloaded.Audio;

internal class AudioRegistry
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
        foreach (var file in Directory.EnumerateFiles(dir))
        {
            this.AddAudioFile(file);
        }
    }

    public void AddAudioFile(string file)
    {
        var config = GetAudioConfig(file);
        if (string.IsNullOrEmpty(config.CueName)) throw new Exception("Missing cue data.");
        this.cueNameAudio[config.CueName] = config;

        Log.Information($"Assigned cue.\nCue Name: {config.CueName}\nFile: {config.AudioFile}");
    }

    public bool TryGetAudio(PlayerConfig player, string cueName, [NotNullWhen(true)] out AudioConfig? audio)
    {
        return this.cueNameAudio.TryGetValue(cueName, out audio);
    }

    private AudioConfig GetAudioConfig(string file)
    {
        var defaultConfig  = GameAudio.GetDefaultConfig(this.game);
        var config = new AudioConfig()
        {
            CategoryIds = defaultConfig.CategoryIds,
            Format = defaultConfig.Format,
            NumChannels = defaultConfig.NumChannels,
            SampleRate = defaultConfig.SampleRate,
        };

        // Load cue data from config.
        var configFile = Path.ChangeExtension(file, "*.yaml");
        if (File.Exists(configFile))
        {
            config ??= this.deserializer.Deserialize<AudioConfig>(File.ReadAllText(configFile));
        }

        // Use file name for cue data.
        else
        {
            config.CueName = Path.GetFileNameWithoutExtension(file);
        }

        config.AudioFile = file;
        config.Format = GetAudioFormat(file);
        return config;
    }

    private static CRIATOM_FORMAT GetAudioFormat(string file)
        => Path.GetExtension(file).ToLower() switch
        {
            ".hca" => CRIATOM_FORMAT.HCA,
            ".adx" => CRIATOM_FORMAT.ADX,
            _ => throw new Exception("Unknown audio format.")
        };
}
