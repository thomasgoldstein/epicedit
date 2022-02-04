using System;

namespace EpicEdit.Rom.Tracks
{
    internal interface ITileset
    {
        int BitsPerPixel { get; }

        int Length { get; }

        Tile this[int index] { get; }
    }
}
