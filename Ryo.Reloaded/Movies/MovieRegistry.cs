using Ryo.Reloaded.Common;
using System.Diagnostics.CodeAnalysis;

namespace Ryo.Reloaded.Movies;

internal class MovieRegistry
{
    private readonly List<MovieContainer> movies = new();

    public void AddMoviePath(string path)
    {
        if (Directory.Exists(path))
        {
            this.AddMovieFolder(path);
        }
        else if (File.Exists(path))
        {
            this.AddMovieFile(path);
        }
        else
        {
            Log.Error($"Movie path was not a folder or file.\nPath: {path}");
        }
    }

    public void AddMovieBind(string moviePath, string bindPath)
        => this.CreateOrGetMovie(moviePath).AddFile(bindPath);

    public bool TryGetMovie(string originalFile, [NotNullWhen(true)] out MovieContainer? movie)
    {
        movie = this.movies.FirstOrDefault(x => x.MatchesPath(originalFile));
        return movie != null;
    }

    private void AddMovieFolder(string folder)
    {
        // Folder named as the movie to add files to.
        if (folder.EndsWith(".usm", StringComparison.OrdinalIgnoreCase))
        {
            var movie = this.CreateOrGetMovie(folder);
            foreach (var file in Directory.EnumerateFiles(folder, "*.usm"))
            {
                movie.AddFile(file);
            }
        }
        else
        {
            // Add files named after the movie.
            foreach (var file in Directory.EnumerateFiles(folder, "*.usm"))
            {
                this.AddMovieFile(file);
            }

            // Add movie configs.
            foreach (var file in Directory.EnumerateFiles(folder, "*.usm.yaml"))
            {
                this.AddMovieConfig(file);
            }

            // Recursively add folders.
            foreach (var dir in Directory.EnumerateDirectories(folder))
            {
                this.AddMoviePath(dir);
            }
        }
    }

    private void AddMovieConfig(string configFile)
    {
        try
        {
            var config = YamlSerializer.DeserializeFile<MovieConfig>(configFile);
            this.AddMovieBind(Path.GetFileNameWithoutExtension(configFile), config.BindPath);

        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to add movie config file.\nFile: {configFile}");
        }
    }

    private void AddMovieFile(string file)
        => this.CreateOrGetMovie(file).AddFile(file);

    private MovieContainer CreateOrGetMovie(string moviePath)
    {
        var movie = this.movies.FirstOrDefault(x => x.MatchesPath(moviePath));
        if (movie == null)
        {
            movie = new(moviePath);
            this.movies.Add(movie);
        }

        return movie;
    }
}

public class MovieConfig
{
    public string BindPath { get; set; } = string.Empty;
}
