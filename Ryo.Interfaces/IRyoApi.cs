﻿namespace Ryo.Interfaces;

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
}