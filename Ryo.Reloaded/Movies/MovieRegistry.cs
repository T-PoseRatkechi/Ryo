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

    public bool TryGetMovie(string originalFile, [NotNullWhen(true)] out MovieContainer? container)
    {
        container = this.movies.FirstOrDefault(x => x.MatchesPath(originalFile));
        return container != null;
    }

    private void AddMovieFolder(string folder)
    {
        // Folder named as the movie to add files to.
        if (folder.EndsWith(".usm", StringComparison.OrdinalIgnoreCase))
        {
            var movie = this.movies.FirstOrDefault(x => x.MatchesPath(folder));
            if (movie == null)
            {
                movie = new(folder);
                this.movies.Add(movie);
            }

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

            // Recursively add folders.
            foreach (var dir in Directory.EnumerateDirectories(folder))
            {
                this.AddMoviePath(dir);
            }
        }
    }

    private void AddMovieFile(string fileToAdd)
    {
        var fileName = Path.GetFileName(fileToAdd);
        var existingMovie = this.movies.FirstOrDefault(x => x.MatchesPath(fileName));
        if (existingMovie != null)
        {
            existingMovie.AddFile(fileToAdd);
        }
        else
        {
            var newMovie = new MovieContainer(fileToAdd);
            newMovie.AddFile(fileToAdd);

            this.movies.Add(newMovie);
        }
    }
}
