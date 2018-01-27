using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.AI
{
    internal enum TrackAIElementShape
    {
        [Description("Rectangle")]
        Rectangle = 0,
        [Description("Triangle top left")]
        TriangleTopLeft = 2, // Top-left angle is the right angle
        [Description("Triangle top right")]
        TriangleTopRight = 4, // And so on
        [Description("Triangle bottom right")]
        TriangleBottomRight = 6,
        [Description("Triangle bottom left")]
        TriangleBottomLeft = 8
    }
}
