using Ryo.Interfaces.Classes;
using Ryo.Interfaces.Structs;

namespace Ryo.Interfaces;

public interface IRyoApi
{
    /// <summary>
    /// Add a path to add audio from.
    /// </summary>
    /// <param name="path">Audio path, file or folder.</param>
    /// <param name="config">Audio config to apply, if any.</param>
    void AddAudioPath(string path, AudioConfig? config);

    /// <summary>
    /// Add an audio preprocessor.
    /// </summary>
    /// <param name="name">Process name.</param>
    /// <param name="process">Process callback.</param>
    void AddAudioPreprocessor(string name, Func<AudioInfo, AudioInfo> process);

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

    /// <summary>
    /// Checks whether a cue container with the given cue name and ACB name exists.
    /// </summary>
    /// <param name="cueName">Cue name.</param>
    /// <param name="acbName">ACB name.</param>
    bool HasCueContainer(string cueName, string acbName);

    /// <summary>
    /// Checks whether a data container with the given name exists.
    /// </summary>
    /// <param name="dataName"></param>
    bool HasDataContainer(string dataName);

    /// <summary>
    /// Checks whether a file container with the given file path exists.
    /// </summary>
    /// <param name="filePath">Audio file path.</param>
    /// <returns></returns>
    bool HasFileContainer(string filePath);

    /// <summary>
    /// Utility functions.
    /// </summary>
    [Obsolete("Use ICriAtomRegistry")]
    IRyoUtils Utilities { get; }

    /// <summary>
    /// Register audio file.
    /// </summary>
    /// <param name="file">File path.</param>
    [Obsolete("Use AddAudioPath(string path, AudioConfig? config)")]
    void AddAudioFile(string file);

    /// <summary>
    /// Add folder to register audio from.
    /// </summary>
    /// <param name="dir">Folder path.</param>
    [Obsolete("Use AddAudioPath(string path, AudioConfig? config)")]
    void AddAudioFolder(string dir);
}
