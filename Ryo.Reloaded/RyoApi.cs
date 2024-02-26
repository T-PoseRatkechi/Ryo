﻿using Ryo.Interfaces;
using Ryo.Reloaded.Audio;
using Ryo.Reloaded.Movies;

namespace Ryo.Reloaded;

internal class RyoApi : IRyoApi
{
    private readonly AudioRegistry audio;
    private readonly MovieRegistry movies;

    public RyoApi(AudioRegistry audio, MovieRegistry movies)
    {
        this.audio = audio;
        this.movies = movies;
    }

    public void AddAudioFile(string file)
        => this.audio.AddAudioFile(file);

    public void AddAudioFolder(string dir)
        => this.audio.AddAudioFolder(dir);

    public void AddMovieBind(string moviePath, string bindPath)
        => this.movies.AddMovieBind(moviePath, bindPath);

    public void AddMoviePath(string path)
        => this.movies.AddMoviePath(path);
}