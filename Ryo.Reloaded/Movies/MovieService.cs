﻿using Reloaded.Hooks.Definitions;
using Ryo.Reloaded.CRI.Mana;
using System.Runtime.InteropServices;
using static Ryo.Reloaded.CRI.Mana.CriManaFunctions;

namespace Ryo.Reloaded.Movies;

internal class MovieService
{
    private readonly CriMana mana;
    private readonly MovieRegistry movieRegistry;
    private IHook<criManaPlayer_SetFile>? setFileHook;
    private bool devMode;

    public MovieService(CriMana mana, MovieRegistry movieRegistry)
    {
        this.mana = mana;
        this.movieRegistry = movieRegistry;

        ScanHooks.Listen(
            nameof(CriManaFunctions.criManaPlayer_SetFile),
            (hooks, result) => this.setFileHook = hooks.CreateHook<criManaPlayer_SetFile>(this.criManaPlayer_SetFile, result).Activate());
    }

    public void SetDevMode(bool devMode)
        => this.devMode = devMode;

#pragma warning disable IDE1006 // Naming Styles
    private void criManaPlayer_SetFile(nint player, nint binder, nint path)
    {
        var file = Marshal.PtrToStringAnsi(path) ?? string.Empty;
        if (this.devMode)
        {
            Log.Information($"{nameof(criManaPlayer_SetFile)} || File: {Path.GetFileName(file)}");
        }
        else
        {
            Log.Debug($"{nameof(criManaPlayer_SetFile)} || Player: {player:X} || Binder: {binder:X} || File: {file}");
        }

        if (this.movieRegistry.TryGetMovie(file, out var movie))
        {
            try
            {
                var movieFile = movie.GetMovieFile();
                var movieFilePtr = StringsCache.GetStringPtr(movieFile);
                this.setFileHook!.OriginalFunction(player, binder, movieFilePtr);
                Log.Debug($"Redirected movie file.\nOriginal: {file}\nNew: {movieFile}");
            }
            catch (Exception ex)
            {
                this.setFileHook!.OriginalFunction(player, binder, path);
                Log.Error(ex, $"Failed to redirect movie file.\nFile: {file}");
            }
        }
        else
        {
            this.setFileHook!.OriginalFunction(player, binder, path);
        }
    }
}
