using Ryo.Interfaces;
using Ryo.Interfaces.Structs;
using Ryo.Reloaded.Audio;
using Ryo.Reloaded.Movies;

namespace Ryo.Reloaded;

internal class RyoApi : IRyoApi
{
    private readonly AudioRegistry audio;
    private readonly AudioPreprocessor preprocessor;
    private readonly MovieRegistry movies;

    public RyoApi(
        AudioRegistry audio,
        AudioPreprocessor preprocessor,
        MovieRegistry movies)
    {
        this.audio = audio;
        this.preprocessor = preprocessor;
        this.movies = movies;
    }

    public IRyoUtils Utilities { get; } = new RyoUtils();

    public void AddAudioFile(string file)
        => this.audio.AddAudioFile(file);

    public void AddAudioFolder(string dir)
        => this.audio.AddAudioFolder(dir);

    public void AddAudioPreprocessor(string name, Func<AudioInfo, AudioInfo> process)
        => this.preprocessor.AddPreprocessor(name, process);

    public void AddMovieBind(string moviePath, string bindPath)
        => this.movies.AddMovieBind(moviePath, bindPath);

    public void AddMoviePath(string path)
        => this.movies.AddMoviePath(path);
}
