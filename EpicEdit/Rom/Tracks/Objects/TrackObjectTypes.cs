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

namespace EpicEdit.Rom.Tracks.Objects
{
    internal enum TrackObjectType : byte
    {
        Pipe = 0,
        Pillar = 1,
        Thwomp = 2,
        Mole = 3,
        Plant = 4,
        Fish = 5,
        RThwomp = 6
    }

    internal enum TrackObjectLoading : byte
    {
        Regular = 0,
        Fish = 1,
        Pillar = 2,
        None = 3
    }
}
