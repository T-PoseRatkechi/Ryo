using System.Diagnostics.CodeAnalysis;
using Ryo.Reloaded.Audio.Models;
using Ryo.Definitions.Enums;
using Ryo.Reloaded.Common;
using Ryo.Interfaces.Classes;
using Ryo.Reloaded.Audio.Models.Containers;

namespace Ryo.Reloaded.Audio;

internal class AudioRegistry
{
    private const string RYO_DATA_DIR_NAME = "DATA.ryo";
    private const string RYO_FILE_DIR_NAME = "FILE.ryo";

    private readonly AudioConfig defaultConfig;
    private readonly AudioPreprocessor preprocessor;
    private readonly Dictionary<CueKey, BaseContainer> cueContainers = new(CueComparer.Instance);
    private readonly Dictionary<string, BaseContainer> dataContainers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, BaseContainer> fileContainers = new(StringComparer.OrdinalIgnoreCase);

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
            config.Apply(preConfig);
        }

        // Apply config settings from file config.
        var configFile = Path.ChangeExtension(file, ".yaml");
        if (File.Exists(configFile) && ParseConfigFile(configFile) is AudioConfig fileConfig)
        {
            config.Apply(fileConfig);
        }

        // Get format from ext.
        config.Format = GetAudioFormat(file);

        // Using parent directory forces replacement files to be
        // in a sub-folder of the actual file path. Basically a requirement since
        // new files can be different format.
        var parentDir = Path.GetDirectoryName(file)!;
        if (file.Contains(RYO_FILE_DIR_NAME, StringComparison.OrdinalIgnoreCase))
        {
            var fileDirIndex = parentDir.IndexOf(RYO_FILE_DIR_NAME, StringComparison.OrdinalIgnoreCase);
            config.AudioFilePath = parentDir[(fileDirIndex + RYO_FILE_DIR_NAME.Length + 1)..].Replace('\\', '/');
        }
        else if (file.Contains(RYO_DATA_DIR_NAME, StringComparison.OrdinalIgnoreCase))
        {
            var dataDirIndex = parentDir.IndexOf(RYO_DATA_DIR_NAME, StringComparison.OrdinalIgnoreCase);
            config.AudioDataName = parentDir[(dataDirIndex + RYO_DATA_DIR_NAME.Length + 1)..].Replace('\\', '/');
        }

        BaseContainer? container;
        var audio = new RyoAudio(file, config);

        // Create audio file container.
        if (config.AudioFilePath != null)
        {
            container = this.CreateOrGetContainer(ContainerType.File, config);
        }

        // Create audio data container.
        else if (config.AudioDataName != null)
        {
            container = this.CreateOrGetContainer(ContainerType.Data, config);
        }

        // Default to cue container.
        else
        {
            // Use file name for cue name if none set.
            if (string.IsNullOrEmpty(config.CueName))
            {
                config.CueName = GetCueName(file);
            }

            container = this.CreateOrGetContainer(ContainerType.Cue, config);
        }

        container.AddAudio(audio);
    }

    private BaseContainer CreateOrGetContainer(ContainerType type, AudioConfig config)
    {
        // Create container.
        BaseContainer? container;

        switch (type)
        {
            case ContainerType.Cue:
                container = new CueContainer(config.CueName!, config.AcbName!, config);
                var cueKey = new CueKey(config.CueName!, config.AcbName!);

                // Use pre-existing cue container if shared ID is set and exists.
                if (config.SharedContainerId != null)
                {
                    if (this.cueContainers.TryGetValue(cueKey, out var existingCue)
                        && existingCue.SharedContainerId?.Equals(config.SharedContainerId, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        container = existingCue;
                    }
                }

                this.cueContainers[cueKey] = container;
                break;
            case ContainerType.File:
                container = new FileContainer(config.AudioFilePath!, config);

                // Use pre-existing cue container if shared ID is set and exists.
                if (config.SharedContainerId != null)
                {
                    if (this.fileContainers.TryGetValue(config.AudioFilePath!, out var existingFile)
                        && existingFile.SharedContainerId?.Equals(config.SharedContainerId, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        container = existingFile;
                    }
                }

                this.fileContainers[config.AudioFilePath!] = container;
                break;
            case ContainerType.Data:
                container = new DataContainer(config.AudioDataName!, config);

                // Use pre-existing cue container if shared ID is set and exists.
                if (config.SharedContainerId != null)
                {
                    if (this.dataContainers.TryGetValue(config.AudioFilePath!, out var existingData)
                        && existingData.SharedContainerId?.Equals(config.SharedContainerId, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        container = existingData;
                    }
                }

                this.dataContainers[config.AudioDataName!] = container;
                break;
            default: throw new Exception("Unknown container.");
        }

        return container;
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
            config.Apply(dirConfig);
        }

        var folderExt = Path.GetExtension(dir);

        // Folder likely contains items for a single item/file,
        // and thus items should be added to a single container.
        if (string.IsNullOrEmpty(folderExt) == false)
        {
            // Set shared container ID.
            config.SharedContainerId = Guid.NewGuid().ToString();
        }

        // Folder sets the ACB name for items.
        // Ex: A folder named "Voices.acb" indicates to use "Voice" for the ACB Name.
        if (folderExt.Equals(".acb", StringComparison.OrdinalIgnoreCase))
        {
            config.AcbName = Path.GetFileNameWithoutExtension(dir);
        }

        // Folder is a Cue Folder, all audio files get added to the same
        // audio container
        else if (folderExt.Equals(".cue", StringComparison.OrdinalIgnoreCase))
        {
            // Set cue name.
            config.CueName = Path.GetFileNameWithoutExtension(dir);
        }

        foreach (var file in Directory.EnumerateFiles(dir))
        {
            var ext = Path.GetExtension(file).ToLower();
            if (ext == ".hca" || ext == ".adx" || ext == ".wav")
            {
                this.AddAudioFile(file, config);
            }
        }

        foreach (var folder in Directory.EnumerateDirectories(dir))
        {
            this.AddAudioFolder(folder, config);
        }
    }

    public bool TryGetCueContainer(string cueName, string acbName, [NotNullWhen(true)] out BaseContainer? container)
        => this.cueContainers.TryGetValue(new(cueName, acbName), out container);

    public bool TryGetDataContainer(string dataName, [NotNullWhen(true)] out BaseContainer? container)
        => this.dataContainers.TryGetValue(dataName, out container);

    public bool TryGetFileContainer(string filePath, [NotNullWhen(true)] out BaseContainer? container)
        => this.fileContainers.TryGetValue(filePath, out container);

    public void PreloadAudio()
    {
        var allFiles = this.cueContainers.Values.Select(x => x.GetContainerFiles()).SelectMany(x => x).ToArray();
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

    private static CriAtomFormat GetAudioFormat(string file)
        => Path.GetExtension(file).ToLower() switch
        {
            ".hca" => CriAtomFormat.HCA,
            ".adx" => CriAtomFormat.ADX,
            ".wav" => CriAtomFormat.WAVE,
            _ => throw new Exception($"Unknown format: {file}"),
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

    private enum ContainerType
    {
        Cue,
        File,
        Data,
    }
}
