using Ryo.Interfaces;
using Ryo.Interfaces.Classes;
using Ryo.Interfaces.Structs;
using Ryo.Reloaded.Audio;
using Ryo.Reloaded.CRI.CriAtomEx;
using Ryo.Reloaded.Movies;

namespace Ryo.Reloaded;

internal class RyoApi : IRyoApi
{
    private readonly AudioRegistry audio;
    private readonly AudioPreprocessor preprocessor;
    private readonly MovieRegistry movies;

    public RyoApi(
        CriAtomRegistry criAtomRegistry,
        AudioRegistry audio,
        AudioPreprocessor preprocessor,
        MovieRegistry movies)
    {
        this.audio = audio;
        this.preprocessor = preprocessor;
        this.movies = movies;
        this.Utilities = new RyoUtils(criAtomRegistry);
    }

    public IRyoUtils Utilities { get; }

    public void AddAudioPath(string path, AudioConfig? config)
        => this.audio.AddAudioPath(path, config);

    public void AddAudioPreprocessor(string name, Func<AudioInfo, AudioInfo> process)
        => this.preprocessor.AddPreprocessor(name, process);

    public void AddMoviePath(string path)
        => this.movies.AddMoviePath(path);

    public void AddMovieBind(string moviePath, string bindPath)
        => this.movies.AddMovieBind(moviePath, bindPath);

    public void AddAudioFile(string file)
        => this.audio.AddAudioFile(file);

    public void AddAudioFolder(string dir)
        => this.audio.AddAudioFolder(dir);

    public bool HasCueContainer(string cueName, string acbName)
        => this.audio.TryGetCueContainer(cueName, acbName, out _);

    public bool HasDataContainer(string dataName)
        => this.audio.TryGetDataContainer(dataName, out _);

    public bool HasFileContainer(string filePath)
        => this.audio.TryGetFileContainer(filePath, out _);
}
