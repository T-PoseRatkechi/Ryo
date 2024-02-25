namespace Ryo.Reloaded.Movies;

internal class MovieContainer
{
    private readonly List<string> files = new();

    public MovieContainer(string originalFile)
    {
        this.OriginalFile = originalFile;
        this.FileName = Path.GetFileName(originalFile);
    }

    public string OriginalFile { get; }

    public string FileName { get; set; }

    public void AddFile(string filePath)
    {
        this.files.Add(filePath);
        Log.Information($"File added to movie: {this.FileName}\nFile: {filePath}");
    }

    public string GetMovieFile()
    {
        if (this.files.Count > 1)
        {
            var randomIndex = Random.Shared.Next(0, this.files.Count);
            Log.Debug($"Random movie index: {randomIndex} || Total Files: {this.files.Count}");
            return this.files[randomIndex];
        }
        else if (this.files.Count == 1)
        {
            return this.files[0];
        }

        throw new Exception($"Movie had no files.\nOriginal File: {this.OriginalFile}");
    }

    /// <summary>
    /// Whether the movie container matches the given path.
    /// </summary>
    /// <param name="path">Path to test.</param>
    public bool MatchesPath(string path)
    {
        var fileName = Path.GetFileName(path);
        return this.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase);
    }
}
