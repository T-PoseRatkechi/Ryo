using System.Diagnostics.CodeAnalysis;
using Ryo.Reloaded.Audio.Models;
using Ryo.Definitions.Enums;
using Ryo.Reloaded.Common;
using Ryo.Interfaces.Classes;

namespace Ryo.Reloaded.Audio;

internal class AudioRegistry
{
    private readonly AudioConfig defaultConfig;
    private readonly AudioPreprocessor preprocessor;
    private readonly Dictionary<CueKey, CueContainer> cues = new(CueComparer.Instance);

    public AudioRegistry(string game, AudioPreprocessor preprocessor)
    {
        this.defaultConfig = GameDefaults.CreateDefaultConfig(game);
        this.preprocessor = preprocessor;
    }

    public void AddAudioPath(string path, AudioConfig? config)
    {
        if (Directory.Exists(path))
        {
            this.AddAudioFolder(path, config);
        }
        else if (File.Exists(path))
        {
            this.AddAudioFile(path, config);
        }
        else
        {
            Log.Error($"Audio path was not found.\nPath: {path}");
        }
    }

    public void AddAudioFile(string file, AudioConfig? preConfig)
    {
        // Start with the game default config.
        var config = this.defaultConfig.Clone();

        // Apply config settings from arg.
        if (preConfig != null)
        {
            ApplyUserConfig(config, preConfig);
        }

        // Apply config settings from file config.
        var configFile = Path.ChangeExtension(file, ".yaml");
        if (File.Exists(configFile) && ParseConfigFile(configFile) is AudioConfig fileConfig)
        {
            ApplyUserConfig(config, fileConfig);
        }

        // Use file name for cue name if none set.
        if (string.IsNullOrEmpty(config.CueName))
        {
            config.CueName = GetCueName(file);
        }

        // Get format from ext.
        config.Format = GetAudioFormat(file);

        // Create cue container.
        var cue = new CueContainer(config.CueName, config.AcbName!, config);
        var cueKey = new CueKey(cue.CueName, cue.AcbName);

        // Use pre-existing cue container if shared ID is set and exists.
        if (config.SharedContainerId != null)
        {
            if (this.cues.TryGetValue(cueKey, out var existingCue)
                && existingCue.SharedContainerId?.Equals(config.SharedContainerId, StringComparison.OrdinalIgnoreCase) == true)
            {
                cue = existingCue;
            }
        }

        var audio = new AudioContainer(file, config);
        cue.AddAudio(audio);

        // TODO: Rework audio preprocessing.
        //this.preprocessor.Preprocess(audio);

        // Register cue.
        this.cues[cueKey] = cue;
    }

    public void AddAudioFile(string file)
        => this.AddAudioFile(file, null);

    public void AddAudioFolder(string dir, AudioConfig? preConfig = null)
    {
        Log.Information($"Adding folder: {dir}");

        // Audio config for folder items.
        var config = preConfig?.Clone() ?? new();

        // Apply config from a folder config file.
        var dirConfigFile = Path.Join(dir, "config.yaml");
        if (File.Exists(dirConfigFile) && ParseConfigFile(dirConfigFile) is AudioConfig dirConfig)
        {
            ApplyUserConfig(config, dirConfig);
        }

        // Folder sets the ACB name for items.
        // Ex: A folder named "Voices.acb" indicates to use "Voice" for the ACB Name.
        var folderName = Path.GetFileName(dir);
        if (folderName.EndsWith(".acb", StringComparison.OrdinalIgnoreCase))
        {
            config.AcbName = Path.GetFileNameWithoutExtension(folderName);
        }

        // Folder is a Cue Folder, all audio files get added to the same
        // audio container
        else if (folderName.EndsWith(".cue", StringComparison.OrdinalIgnoreCase))
        {
            // Set cue name.
            config.CueName = Path.GetFileNameWithoutExtension(folderName);

            // Set shared container ID.
            config.SharedContainerId = folderName;
        }

        foreach (var file in Directory.EnumerateFiles(dir))
        {
            var ext = Path.GetExtension(file).ToLower();
            if (ext == ".hca" || ext == ".adx")
            {
                this.AddAudioFile(file, config);
            }
        }

        foreach (var folder in Directory.EnumerateDirectories(dir))
        {
            this.AddAudioFolder(folder, config);
        }
    }

    public bool TryGetCue(string cueName, string acbName, [NotNullWhen(true)] out CueContainer? cue)
        => this.cues.TryGetValue(new(cueName, acbName), out cue);

    public void PreloadAudio()
    {
        var allFiles = this.cues.Values.Select(x => x.GetContainerFiles()).SelectMany(x => x).ToArray();
        foreach (var file in allFiles)
        {
            AudioCache.GetAudioData(file);
        }
    }

    private static AudioConfig? ParseConfigFile(string configFile)
    {
        try
        {
            Log.Debug($"Loading user config: {configFile}");
            var config = YamlSerializer.DeserializeFile<AudioConfig>(configFile);
            return config;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to parse user config.\nFile: {configFile}");
            return null;
        }
    }

    /// <summary>
    /// Applies defined settings in <paramref name="newConfig"/> to <paramref name="mainConfig"/>.
    /// </summary>
    /// <param name="mainConfig">Config to apply new settings to.</param>
    /// <param name="newConfig">Config containing the settings to apply.</param>
    private static void ApplyUserConfig(AudioConfig mainConfig, AudioConfig newConfig)
    {
        mainConfig.CueName = newConfig.CueName ?? mainConfig.CueName;
        mainConfig.AcbName = newConfig.AcbName ?? mainConfig.AcbName;
        mainConfig.PlayerId = newConfig.PlayerId ?? mainConfig.PlayerId;
        mainConfig.CategoryIds = newConfig.CategoryIds ?? mainConfig.CategoryIds;
        mainConfig.NumChannels = newConfig.NumChannels ?? mainConfig.NumChannels;
        mainConfig.SampleRate = newConfig.SampleRate ?? mainConfig.SampleRate;
        mainConfig.Volume = newConfig.Volume ?? mainConfig.Volume;
        mainConfig.Tags = newConfig.Tags ?? mainConfig.Tags;
        mainConfig.Key = newConfig.Key ?? mainConfig.Key;
    }

    private static CriAtomFormat GetAudioFormat(string file)
        => Path.GetExtension(file).ToLower() switch
        {
            ".hca" => CriAtomFormat.HCA,
            ".adx" => CriAtomFormat.ADX,
            _ => throw new Exception($"Unknown audio format.\nFile: {file}")
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

    private static void PrintUserConfig(AudioConfig config)
    {
        var categories = (config.CategoryIds != null) ? string.Join(',', config.CategoryIds.Select(x => x.ToString())) : "Not Set";
        Log.Verbose(
            $"User Config\n" +
            $"Cue Name: {config.CueName ?? "Not Set"}\n" +
            $"ACB Name: {config.AcbName ?? "Not Set"}\n" +
            $"Player ID: {config.PlayerId?.ToString() ?? "Not Set"}\n" +
            $"Categories: {categories}\n" +
            $"Volume: {config.Volume?.ToString() ?? "Not Set"}\n" +
            $"Sample Rate: {config.SampleRate?.ToString() ?? "Not Set"}\n" +
            $"Channels: {config.NumChannels?.ToString() ?? "Not Set"}");
    }
}
