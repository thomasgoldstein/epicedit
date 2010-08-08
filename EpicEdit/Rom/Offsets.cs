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
		ModeStrings, // Names of the modes on the title screen
		NameStrings, // Name texts (cups, themes, tracks)
		TrackMaps, // Track address index
		TrackThemes, // Track theme index
		GPTrackOrder, // GP track order index
		GPTrackNames, // GP track name index
		BattleTrackOrder, // Battle track order index
		BattleTrackNames, // Battle track name index
		FirstBattleTrack, // Index of the first battle track (track displayed by default when entering the battle track selection)
		TrackObjectGraphics, // Track object graphics address index
		TrackBackgroundGraphics, // Track background graphics address index
		TrackBackgroundLayouts, // Track background layout address index
		TrackAIDataFirstAddressByte, // The leading byte that composes AI-related addresses
		TrackAIZones, // AI zone index
		TrackAITargets, // AI target index
		TrackObjects, // Track objects
		TrackObjectZones, // Track object zones
		TrackOverlayItems, // Track overlay items
		TrackOverlaySizes, // Track overlay sizes
		TrackOverlayPatternAddresses, // Track overlay pattern addresses
		TrackLapLines, // Track lap lines
		TrackStartPositions, // Starting position of the drivers on the tracks
		TimeTrialPreviewTrackLapLines, // Position of the track lap lines shown in Time Trial track previews
		ThemeRoadGraphics, // Theme road graphics address index
		ThemeColorPalettes, // Theme color palette address index
		CommonTilesetGraphics, // Common tileset graphics
		ItemProbabilities, // Item probabilities
		ItemIcons, // Item icon graphics
		ItemIconTilesPalettes, // Item icon color tile and palette indexes
		//UnknownMakeRelated
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
					this[Address.ModeStrings] = 0x58418;
					this[Address.BattleTrackOrder] = 0x1C022;
					this[Address.FirstBattleTrack] = 0x1BF0A;
					this[Address.TrackMaps] = new Offset(Utilities.ReadBlock(romBuffer, new Offset(0x1E74D), 3));
					this[Address.TrackAIDataFirstAddressByte] = new Offset(0x1FBC4);
					this[Address.TrackAIZones] = 0x1FF8C;
					this[Address.TrackStartPositions] = 0x18C27;
					this[Address.TimeTrialPreviewTrackLapLines] = 0x1C886;
					this[Address.ItemProbabilities] = 0x1B39F;
					this[Address.ItemIconTilesPalettes] = 0x1B1DC;
					this[Address.TrackOverlaySizes] = 0x4F1FC;
					this[Address.TrackOverlayPatternAddresses] = 0x4F0B5;
					break;

				case Regions.US:
					this[Address.ModeStrings] = 0x583DB;
					this[Address.BattleTrackOrder] = 0x1C15C;
					this[Address.FirstBattleTrack] = 0x1C04C;
					this[Address.TrackMaps] = new Offset(Utilities.ReadBlock(romBuffer, new Offset(0x1E749), 3));
					this[Address.TrackAIDataFirstAddressByte] = 0x1FBD3;
					this[Address.TrackAIZones] = 0x1FF9B;
					this[Address.TrackStartPositions] = 0x18C13;
					this[Address.TimeTrialPreviewTrackLapLines] = 0x1C915;
					this[Address.ItemProbabilities] = 0x1B4E3;
					this[Address.ItemIconTilesPalettes] = 0x1B320;
					this[Address.TrackOverlaySizes] = 0x4F384;
					this[Address.TrackOverlayPatternAddresses] = 0x4F23D;
					//this[Offsets.UnknownMakeRelated] = new Offset(Utilities.ReadBlock(romBuffer, new Offset(0x1E765), 3)); // TODO: Figure out what that offset is (MAKE-compatibility related)
					break;

				case Regions.Euro:
					this[Address.ModeStrings] = 0x583D9;
					this[Address.BattleTrackOrder] = 0x1BFF8;
					this[Address.FirstBattleTrack] = 0x1BEE8;
					this[Address.TrackMaps] = new Offset(Utilities.ReadBlock(romBuffer, new Offset(0x1E738), 3));
					this[Address.TrackAIDataFirstAddressByte] = 0x1FB9D;
					this[Address.TrackAIZones] = 0x1FF6D;
					this[Address.TrackStartPositions] = 0x18C2C;
					this[Address.TimeTrialPreviewTrackLapLines] = 0x1C7B1;
					this[Address.ItemProbabilities] = 0x1B37F;
					this[Address.ItemIconTilesPalettes] = 0x1B1BC;
					this[Address.TrackOverlaySizes] = 0x4F2A0;
					this[Address.TrackOverlayPatternAddresses] = 0x4F159;
					break;
			}

			this[Address.ItemIcons] = 0x112F8;
			this[Address.TrackObjects] = 0x5C800;
			this[Address.TrackObjectZones] = 0x4DB93;
			this[Address.TrackOverlayItems] = 0x5D000;
			this[Address.TrackLapLines] = 0x180D4;
			this[Address.CommonTilesetGraphics] = 0x40000;

			this[Address.TrackAITargets] = this[Address.TrackAIZones] + 0x30;
			this[Address.BattleTrackNames] = this[Address.TimeTrialPreviewTrackLapLines] + 0x2A;
			this[Address.GPTrackNames] = this[Address.BattleTrackNames] + 0x32;
			this[Address.NameStrings] = this[Address.GPTrackNames] + 0xC1;

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
