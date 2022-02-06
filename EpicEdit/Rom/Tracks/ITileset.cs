using System;

namespace EpicEdit.Rom.Tracks
{
    internal interface ITileset
    {
        int BitsPerPixel { get; }

        int Length { get; }

        Palettes Palettes { get; }

        Tile this[int index] { get; }
    }
}
