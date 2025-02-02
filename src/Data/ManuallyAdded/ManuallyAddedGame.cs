﻿using System.Threading.Tasks;
using DLSS_Swapper.Interfaces;
using SQLite;

namespace DLSS_Swapper.Data.ManuallyAdded;

[Table("ManuallyAddedGame")]
public class ManuallyAddedGame : Game
{
    public override GameLibrary GameLibrary => GameLibrary.ManuallyAdded;

//    public override string HeaderImage => string.Empty;

    public ManuallyAddedGame()
    {

    }
    public ManuallyAddedGame(string id)
    {
        PlatformId = id;
        SetID();
    }

    public async Task ImportCoverImage(string imagePath)
    {
        await ResizeCoverAsync(imagePath).ConfigureAwait(false);
    }

    protected override async Task UpdateCacheImageAsync()
    {
        // NOOP, the image is manually managed by the user.
    }

    public override bool UpdateFromGame(Game game)
    {
        var didChange = ParentUpdateFromGame(game);

        if (game is ManuallyAddedGame manuallyAddedGame)
        {

        }

        return didChange;
    }
}
