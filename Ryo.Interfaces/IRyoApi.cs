namespace Ryo.Interfaces;

public interface IRyoApi
{
    /// <summary>
    /// Register audio file.
    /// </summary>
    /// <param name="file">File path.</param>
    void AddAudioFile(string file);

    /// <summary>
    /// Add folder to register audio from.
    /// </summary>
    /// <param name="dir">Folder path.</param>
    void AddAudioFolder(string dir);

    /// <summary>
    /// Add a path to add movies from.
    /// </summary>
    /// <param name="path">Movies path, file or folder.</param>
    void AddMoviePath(string path);

    /// <summary>
    /// Bind a movie path to a custom path.
    /// </summary>
    /// <param name="moviePath">Movie path to bind.</param>
    /// <param name="bindPath">Custom path.</param>
    void AddMovieBind(string moviePath, string bindPath);

    IRyoUtils Utilities { get; }
}
