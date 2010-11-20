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

using System;

namespace EpicEdit.Rom
{
	public enum Address : int
	{
		/// <summary>
		/// Offset to the names of the modes on the title screen.
		/// </summary>
		ModeStrings,

		/// <summary>
		/// Name texts (cups, themes, tracks).
		/// </summary>
		NameStrings,

		/// <summary>
		/// Track map address index.
		/// </summary>
		TrackMaps,

		/// <summary>
		/// Track theme index.
		/// </summary>
		TrackThemes,

		/// <summary>
		/// GP track order index.
		/// </summary>
		GPTrackOrder,

		/// <summary>
		/// GP track name index.
		/// </summary>
		GPTrackNames,

		/// <summary>
		/// Battle track order index.
		/// </summary>
		BattleTrackOrder,

		/// <summary>
		/// Battle track name index.
		/// </summary>
		BattleTrackNames,

		/// <summary>
		/// Index of the first battle track (track displayed by default when entering the battle track selection).
		/// </summary>
		FirstBattleTrack,

		/// <summary>
		/// Track object graphics address index.
		/// </summary>
		TrackObjectGraphics,

		/// <summary>
		/// Track background graphics address index.
		/// </summary>
		TrackBackgroundGraphics,

		/// <summary>
		/// Track background layout address index.
		/// </summary>
		TrackBackgroundLayouts,

		/// <summary>
		/// The leading byte that composes AI-related addresses.
		/// </summary>
		TrackAIDataFirstAddressByte,

		/// <summary>
		/// AI zone index.
		/// </summary>
		TrackAIZones,

		/// <summary>
		/// AI target index.
		/// </summary>
		TrackAITargets,

		/// <summary>
		/// Track objects.
		/// </summary>
		TrackObjects,

		/// <summary>
		/// Track object zones.
		/// </summary>
		TrackObjectZones,

		/// <summary>
		/// Track overlay items.
		/// </summary>
		TrackOverlayItems,

		/// <summary>
		/// Track overlay sizes.
		/// </summary>
		TrackOverlaySizes,

		/// <summary>
		/// Track overlay pattern addresses.
		/// </summary>
		TrackOverlayPatterns,

		/// <summary>
		/// Starting position of the drivers on the GP tracks.
		/// </summary>
		GPTrackStartPositions,

		/// <summary>
		/// Track lap lines.
		/// </summary>
		TrackLapLines,

		/// <summary>
		/// Position of the track lap lines shown in track previews (Match Race / Time Trial).
		/// </summary>
		TrackPreviewLapLines,

		/// <summary>
		/// Addresses to the starting position of the drivers on the battle tracks.
		/// </summary>
		BattleTrackStartPositions,

		/// <summary>
		/// Theme road graphics address index.
		/// </summary>
		ThemeRoadGraphics,

		/// <summary>
		/// Theme color palette address index.
		/// </summary>
		ThemeColorPalettes,

		/// <summary>
		/// Common tileset graphics.
		/// </summary>
		CommonTilesetGraphics,

		/// <summary>
		/// Item probabilities.
		/// </summary>
		ItemProbabilities,

		/// <summary>
		/// Item icon graphics.
		/// </summary>
		ItemIcons,

		/// <summary>
		/// Item icon color tile and palette indexes.
		/// </summary>
		ItemIconTilesPalettes
	}

	public class Offsets
	{
		private Offset[] offsets;

		/// <summary>
		/// Loads all the needed offsets depending on the ROM region.
		/// </summary>
		public Offsets(byte[] romBuffer, Regions region)
		{
			this.offsets = new Offset[Enum.GetValues(typeof(Address)).Length];

			switch (region)
			{
				case Regions.Jap:
					this[Address.ModeStrings] = 0x58B19;
					this[Address.BattleTrackOrder] = 0x1C022;
					this[Address.FirstBattleTrack] = 0x1BF0A;
					this[Address.TrackMaps] = new Offset(Utilities.ReadBlock(romBuffer, 0x1E74D, 3));
					this[Address.TrackAIDataFirstAddressByte] = 0x1FBC4;
					this[Address.TrackAIZones] = 0x1FF8C;
					this[Address.BattleTrackStartPositions] = 0x18B5F;
					this[Address.TrackPreviewLapLines] = 0x1C886;
					this[Address.ItemIconTilesPalettes] = 0x1B1DC;
					this[Address.TrackOverlayPatterns] = 0x4F0B5;
					break;

				case Regions.US:
					this[Address.ModeStrings] = 0x58B00;
					this[Address.BattleTrackOrder] = 0x1C15C;
					this[Address.FirstBattleTrack] = 0x1C04C;
					this[Address.TrackMaps] = new Offset(Utilities.ReadBlock(romBuffer, 0x1E749, 3));
					this[Address.TrackAIDataFirstAddressByte] = 0x1FBD3;
					this[Address.TrackAIZones] = 0x1FF9B;
					this[Address.BattleTrackStartPositions] = 0x18B4B;
					this[Address.TrackPreviewLapLines] = 0x1C915;
					this[Address.ItemIconTilesPalettes] = 0x1B320;
					this[Address.TrackOverlayPatterns] = 0x4F23D;
					//this[Offsets.UnknownMakeRelated] = new Offset(Utilities.ReadBlock(romBuffer, 0x1E765, 3)); // TODO: Figure out what that offset is (MAKE-compatibility related)
					break;

				case Regions.Euro:
					this[Address.ModeStrings] = 0x58AF2;
					this[Address.BattleTrackOrder] = 0x1BFF8;
					this[Address.FirstBattleTrack] = 0x1BEE8;
					this[Address.TrackMaps] = new Offset(Utilities.ReadBlock(romBuffer, 0x1E738, 3));
					this[Address.TrackAIDataFirstAddressByte] = 0x1FB9D;
					this[Address.TrackAIZones] = 0x1FF6D;
					this[Address.BattleTrackStartPositions] = 0x18B64;
					this[Address.TrackPreviewLapLines] = 0x1C7B1;
					this[Address.ItemIconTilesPalettes] = 0x1B1BC;
					this[Address.TrackOverlayPatterns] = 0x4F159;
					break;
			}

			this[Address.ItemIcons] = 0x112F8;
			this[Address.TrackObjects] = 0x5C800;
			this[Address.TrackObjectZones] = 0x4DB93;
			this[Address.TrackOverlayItems] = 0x5D000;
			this[Address.TrackLapLines] = 0x180D4;
			this[Address.CommonTilesetGraphics] = 0x40000;

			this[Address.GPTrackStartPositions] = this[Address.BattleTrackStartPositions] + 0xC8;
			this[Address.TrackAITargets] = this[Address.TrackAIZones] + 0x30;
			this[Address.BattleTrackNames] = this[Address.TrackPreviewLapLines] + 0x2A;
			this[Address.GPTrackNames] = this[Address.BattleTrackNames] + 0x32;
			this[Address.NameStrings] = this[Address.GPTrackNames] + 0xC1;
			this[Address.TrackOverlaySizes] = this[Address.TrackOverlayPatterns] + 0x147;
			this[Address.ItemProbabilities] = this[Address.ItemIconTilesPalettes] + 0x1C3;

			this[Address.ThemeRoadGraphics] = this[Address.TrackMaps] + Game.TrackCount * 3;
			this[Address.ThemeColorPalettes] = this[Address.ThemeRoadGraphics] + Game.ThemeCount * 3;
			this[Address.TrackObjectGraphics] = this[Address.ThemeColorPalettes] + Game.ThemeCount * 3;
			this[Address.TrackBackgroundGraphics] = this[Address.TrackObjectGraphics] + Game.ThemeCount * 3;
			this[Address.TrackBackgroundLayouts] = this[Address.TrackBackgroundGraphics] + Game.ThemeCount * 3;
			this[Address.GPTrackOrder] = this[Address.TrackBackgroundLayouts] + Game.ThemeCount * 3;
			this[Address.TrackThemes] = this[Address.GPTrackOrder] + Game.GPTrackCount;
		}

		public int this[Address address]
		{
			get { return offsets[(int)address]; }
			set { this.offsets[(int)address] = new Offset(value); }
		}
	}
}
