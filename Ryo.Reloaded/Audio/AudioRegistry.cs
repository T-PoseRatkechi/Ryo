﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Ryo.Reloaded.Audio.Models;
using Ryo.Definitions.Enums;
using Ryo.Reloaded.Common;

namespace Ryo.Reloaded.Audio;

internal class AudioRegistry
{
    private readonly string game;
    private readonly AudioPreprocessor preprocessor;
    private readonly Dictionary<Cue, AudioConfig> assignedCues = new(CueComparer.Instance);
    private readonly Dictionary<string, AudioData> cachedAudioData = new(StringComparer.OrdinalIgnoreCase);

    public AudioRegistry(string game, AudioPreprocessor preprocessor)
    {
        this.game = game;
        this.preprocessor = preprocessor;
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
            var ext = Path.GetExtension(file).ToLower();
            if (ext == ".hca" || ext == ".adx")
            {
                this.AddAudioFile(file, dirConfig);
            }
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
        if (string.IsNullOrEmpty(audio.CueName) || string.IsNullOrEmpty(audio.AcbName)) throw new Exception("Missing cue or ACB name.");

        this.preprocessor.Preprocess(audio);

        var cue = new Cue(audio.CueName, audio.AcbName);
        this.assignedCues[cue] = audio;

        Log.Information($"Assigned Cue: {cue.CueName} || ACB: {cue.AcbName}\nFile: {audio.AudioFile}");
    }

    public bool TryGetAudio(string cueName, string acbName, [NotNullWhen(true)] out AudioConfig? audio)
        => this.assignedCues.TryGetValue(new(cueName, acbName), out audio);

    public record AudioData(nint Buffer, int Size);

    public AudioData GetAudioData(string audioFile)
    {
        if (this.cachedAudioData.TryGetValue(audioFile, out var existingData))
        {
            return existingData;
        }

        var data = File.ReadAllBytes(audioFile);
        var buffer = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, buffer, data.Length);
        this.cachedAudioData[audioFile] = new(buffer, data.Length);

        Log.Debug($"Loading audio: {audioFile}");
        return this.cachedAudioData[audioFile];
    }

    public void PreloadAudio()
    {
        foreach (var cue in this.assignedCues)
        {
            this.GetAudioData(cue.Value.AudioFile);
        }
    }

    private AudioConfig GetAudioConfig(string file, UserAudioConfig? folderConfig = null)
    {
        var config = GameDefaults.CreateDefaultConfig(this.game);
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

    private UserAudioConfig? GetUserConfig(string configFile)
    {
        try
        {
            Log.Debug($"Loading user config: {configFile}");
            var config = YamlSerializer.DeserializeFile<UserAudioConfig>(configFile);
            return config;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to parse user config.\nFile: {configFile}");
            return null;
        }
    }

    private static void ApplyUserConfig(AudioConfig config, UserAudioConfig? userConfig)
    {
        config.CueName = userConfig?.CueName ?? config.CueName;
        config.AcbName = userConfig?.AcbName ?? config.AcbName;
        config.PlayerId = userConfig?.PlayerId ?? config.PlayerId;
        config.CategoryIds = userConfig?.CategoryIds ?? config.CategoryIds;
        config.NumChannels = userConfig?.NumChannels ?? config.NumChannels;
        config.SampleRate = userConfig?.SampleRate ?? config.SampleRate;
        config.Volume = userConfig?.Volume ?? config.Volume;
        config.Tags = userConfig?.Tags ?? config.Tags;
        config.Key = userConfig?.Key ?? config.Key;
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

    private static void PrintUserConfig(UserAudioConfig config)
    {
        var categories = (config.CategoryIds != null) ? string.Join(',', config.CategoryIds.Select(x => x.ToString())) : "Not Set";
        Log.Debug(
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
