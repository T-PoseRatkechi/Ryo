using System.Diagnostics.CodeAnalysis;
using Ryo.Reloaded.Audio.Models;
using Ryo.Definitions.Enums;
using Ryo.Reloaded.Common;

namespace Ryo.Reloaded.Audio;

internal class AudioRegistry
{
    private readonly string game;
    private readonly AudioConfig defaultConfig;
    private readonly AudioPreprocessor preprocessor;
    private readonly Dictionary<Cue, AudioContainer> cues = new(CueComparer.Instance);

    public AudioRegistry(string game, AudioPreprocessor preprocessor)
    {
        this.game = game;
        this.defaultConfig = GameDefaults.CreateDefaultConfig(game);
        this.preprocessor = preprocessor;
    }

    public void AddAudioFolder(string dir, AudioConfig? preConfig = null)
    {
        Log.Information($"Adding folder: {dir}");

        // Audio config for folder items.
        var currentConfig = preConfig?.Clone() ?? this.defaultConfig.Clone();

        // Apply config from a folder config file.
        var dirConfigFile = Path.Join(dir, "config.yaml");
        if (File.Exists(dirConfigFile) && ParseConfigFile(dirConfigFile) is AudioConfig dirConfig)
        {
            ApplyUserConfig(currentConfig, dirConfig);
        }

        // Folder sets the ACB name for items.
        // Ex: A folder named "Voices.acb" indicates to use "Voice" for the ACB Name.
        var folderName = Path.GetFileName(dir);
        if (folderName.EndsWith(".acb", StringComparison.OrdinalIgnoreCase))
        {
            currentConfig.AcbName = Path.GetFileNameWithoutExtension(folderName);
        }

        // Folder is a Cue Folder, all audio files get added to the same
        // audio container
        else if (folderName.EndsWith(".cue", StringComparison.OrdinalIgnoreCase))
        {
            // Set cue name.
            currentConfig.CueName = Path.GetFileNameWithoutExtension(folderName);

            // Create new audio container.
            var cueAudio = new AudioContainer(currentConfig);

            // Add all audio files (hca/adx) in folder to container.
            // TODO: Create general function to handle adding files appropriately
            //       to audio containers.
            foreach (var file in Directory.EnumerateFiles(dir))
            {
                var ext = Path.GetExtension(file).ToLower();
                if (ext == ".hca" || ext == ".adx")
                {
                    cueAudio.AddFile(file);
                }
            }

            this.RegisterCue(cueAudio);
            return;
        }

        PrintUserConfig(currentConfig);

        // Folder is a normal folder.

        // For files in normal folders, match existing Ryo
        // functionality.

        // Files in normal folders create audio containers
        // per file.
        foreach (var file in Directory.EnumerateFiles(dir))
        {
            var ext = Path.GetExtension(file).ToLower();
            if (ext == ".hca" || ext == ".adx")
            {
                this.AddAudioFile(file, currentConfig);
            }
        }

        foreach (var folder in Directory.EnumerateDirectories(dir))
        {
            this.AddAudioFolder(folder, currentConfig);
        }
    }

    public void AddAudioFile(string file)
        => this.AddAudioFile(file, this.defaultConfig.Clone());

    public void AddAudioFile(string file, AudioConfig preConfig)
    {
        var audio = CreateContainerFromFile(file, preConfig);
        if (string.IsNullOrEmpty(audio.CueName) || string.IsNullOrEmpty(audio.AcbName)) throw new Exception("Missing cue or ACB name.");

        // TODO: Rework audio preprocessing.
        //this.preprocessor.Preprocess(audio);

        this.RegisterCue(audio);
    }

    private void RegisterCue(AudioContainer audio)
    {
        var cue = new Cue(audio.CueName, audio.AcbName);
        this.cues[cue] = audio;
    }

    public bool TryGetAudio(string cueName, string acbName, [NotNullWhen(true)] out AudioContainer? audio)
        => this.cues.TryGetValue(new(cueName, acbName), out audio);

    public void PreloadAudio()
    {
        var allFiles = this.cues.Values.Select(x => x.GetContainerFiles()).SelectMany(x => x).ToArray();
        foreach (var file in allFiles)
        {
            AudioCache.GetAudioData(file);
        }
    }

    private static AudioContainer CreateContainerFromFile(string file, AudioConfig preConfig)
    {
        var currentConfig = preConfig.Clone();

        // Load file config.
        var configFile = Path.ChangeExtension(file, ".yaml");
        if (File.Exists(configFile) && ParseConfigFile(configFile) is AudioConfig fileConfig)
        {
            ApplyUserConfig(currentConfig, fileConfig);
        }

        // Use file name for cue name if none set.
        if (string.IsNullOrEmpty(currentConfig.CueName))
        {
            currentConfig.CueName = GetCueName(file);
        }

        // Get format from ext.
        currentConfig.Format = GetAudioFormat(file);

        // Create container and add file.
        var audioContainer = new AudioContainer(currentConfig);
        audioContainer.AddFile(file);

        return audioContainer;
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
