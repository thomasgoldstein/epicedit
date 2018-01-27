#region GPL statement
/*Epic Edit is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
#endregion

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
