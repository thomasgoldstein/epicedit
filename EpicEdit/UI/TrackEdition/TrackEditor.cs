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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.TrackEdition
{
	public enum ActionButton { None, LeftMouseButton, MiddleMouseButton, RightMouseButton, CtrlKey }
	public enum EditionMode { Tileset, Overlay, Start, Objects, AI }

	/// <summary>
	/// A track editor.
	/// </summary>
	public partial class TrackEditor : UserControl
	{
		#region Private members
		/// <summary>
		/// Used to draw the track.
		/// </summary>
		private TrackDrawer trackDrawer;

		/// <summary>
		/// The track currently displayed.
		/// </summary>
		private Track track;

		/// <summary>
		/// All the available zoom levels.
		/// </summary>
		private float[] zoomLevels;

		/// <summary>
		/// The index to the current zoom level.
		/// </summary>
		private int zoomLevelIndex;

		/// <summary>
		/// The index to the default zoom level (x1).
		/// </summary>
		private const int DefaultZoomLevelIndex = 2;

		/// <summary>
		/// The current zoom level of the track display.
		/// </summary>
		private float Zoom
		{
			get { return this.zoomLevels[this.zoomLevelIndex]; }
		}

		// Which pixel the cursor is on (doesn't take scrolling position in consideration).
		private Point pixelPosition;

		// Which pixel the cursor is on (takes scrolling position in consideration).
		private Point AbsolutePixelPosition
		{
			get
			{
				return new Point(this.scrollPosition.X * 8 + this.pixelPosition.X,
								 this.scrollPosition.Y * 8 + this.pixelPosition.Y);
			}
		}

		/// <summary>
		/// Which tile the cursor is on (doesn't take scrolling position in consideration).
		/// </summary>
		private Point TilePosition
		{
			get
			{
				return new Point(this.pixelPosition.X / 8,
								 this.pixelPosition.Y / 8);
			}
		}

		/// <summary>
		/// Which tile the cursor is on (takes scrolling position in consideration).
		/// </summary>
		private Point AbsoluteTilePosition
		{
			get
			{
				return  new Point(this.scrollPosition.X + this.TilePosition.X,
				                  this.scrollPosition.Y + this.TilePosition.Y);
			}
		}

		/// <summary>
		/// Scrolling position in the track (= location of the top-left displayed tile).
		/// </summary>
		private Point scrollPosition;

		/// <summary>
		/// Where copied tiles are stored.
		/// </summary>
		private List<byte> tileClipboard;

		/// <summary>
		/// Dimension of the tile clipboard.
		/// </summary>
		private Size tileClipboardSize;

		/// <summary>
		/// Used to store the anchor point for various operations (position where tile clipboard was started, dragging map or AI elements...).
		/// </summary>
		private Point anchorPoint;

		/// <summary>
		/// Top-left position of the clipboard rectangle (doesn't take scrolling position in consideration).
		/// </summary>
		private Point tileClipboardTopLeft;

		/// <summary>
		/// Defines which action button is currently pressed, if any.
		/// Only one button is considered pressed at a given time,
		/// the ones pressed before the first one has been released are ignored.
		/// </summary>
		private ActionButton buttonPressed;

		/// <summary>
		/// Determines from which side the current element is being resized.
		/// The element can be the track lap line, or an AI zone.
		/// </summary>
		private ResizeHandle resizeHandle;

		private enum StartAction
		{
			None,
			DragLapLine,
			ResizeLapLine,
			DragStartPosition,
			//DragStartPosition2ndRow,
			DragAll
		}

		private enum AIAction
		{
			None,
			DragTarget,
			DragZone,
			ResizeZone
		}

		/// <summary>
		/// The hovered track overlay tile.
		/// </summary>
		private OverlayTile hoveredOverlayTile;

		/// <summary>
		/// The location of the selected overlay tile pattern.
		/// </summary>
		private Point selectedOverlayPatternLocation;

		/// <summary>
		/// The current action the user is doing (or about to do) on the start data.
		/// </summary>
		private EpicEdit.UI.TrackEdition.TrackEditor.StartAction startAction;

		/// <summary>
		/// The hovered track object.
		/// </summary>
		private TrackObject hoveredObject;

		/// <summary>
		/// The hovered AI element.
		/// </summary>
		private TrackAIElement hoveredAIElem;

		/// <summary>
		/// The current action the user is doing (or about to do) on the AI.
		/// </summary>
		private EpicEdit.UI.TrackEdition.TrackEditor.AIAction aiAction;
		#endregion Private members

		[Browsable(true)]
		public event EventHandler<EventArgs<string>> RomDragged;

		[Browsable(true)]
		public event EventHandler<EventArgs> OpenRomDialogRequested
		{
			add { this.menuBar.OpenRomDialogRequested += value; }
			remove { this.menuBar.OpenRomDialogRequested -= value; }
		}

		[Browsable(true)]
		public event EventHandler<EventArgs> SaveRomDialogRequested
		{
			add { this.menuBar.SaveRomDialogRequested += value; }
			remove { this.menuBar.SaveRomDialogRequested -= value; }
		}

		public TrackEditor()
		{
			this.InitializeComponent();

			this.trackDisplayVScrollBar.ValueChanged += UpdateScrollPositionX;
			this.trackDisplayHScrollBar.ValueChanged += UpdateScrollPositionY;

			// Attach MouseMove and MouseLeave events to each of the mode tab pages, 
			// and make it so all their child controls also respond to these events
			foreach (TabPage tab in this.modeTabControl.TabPages)
			{
				tab.MouseMove += this.ModeTabPageMouseMove;
				EventBroadcastProvider.CreateProvider(tab, "MouseMove");

				tab.MouseLeave += this.ModeTabPageMouseLeave;
				EventBroadcastProvider.CreateProvider(tab, "MouseLeave");
				// FIXME: Mouse-wheel scrolling should be possible on the
				// modeTabControl scrollbar, but it isn't.
			}

			this.ResetCurrentPosition();

			this.zoomLevels = new float[] { .5f, .75f, 1, 2, 3, 4 };
			this.zoomLevelIndex = TrackEditor.DefaultZoomLevelIndex;

			this.tileClipboard = new List<byte>();
			this.tileClipboard.Add(this.tilesetControl.SelectedTile);
			this.tileClipboardSize.Width = this.tileClipboardSize.Height = 1;
		}

		#region Menu Options
		public void InitOnFirstRomLoad()
		{
			this.trackDrawer = new TrackDrawer(this.trackDisplayPanel, this.Zoom);
			this.tilesetControl.InitOnFirstRomLoad();
			this.overlayControl.InitOnFirstRomLoad();
			this.trackTreeView.InitOnFirstRomLoad();

			this.SetCurrentTrack();
			this.trackDrawer.LoadTrack(this.track);
			this.tilesetControl.SelectCurrentTrackTheme();

			// Adding these event handlers here rather than in the Designer.cs
			// saves us a null check on this.drawer in each of the corresponding functions,
			// because the drawer hasn't been initialized yet before a ROM is loaded.
			this.trackDisplayPanel.Paint += this.TrackDisplayPanelPaint;
			this.trackDisplayPanel.SizeChanged += this.TrackDisplayPanelSizeChanged;

			this.RecalculateScrollbarMaximums();

			this.trackDisplayPanel.Enabled = true;
			this.modeTabControl.Enabled = true;
			this.menuBar.EnableControls();
		}

		public void InitOnRomLoad()
		{
			this.tilesetControl.InitOnRomLoad();
			this.overlayControl.InitOnRomLoad();
			this.trackTreeView.InitOnRomLoad();
		}

		private void TrackEditorDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Move;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void TrackEditorDragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				EventArgs<string> sea = new EventArgs<string>(files[0]);
				this.RomDragged(this, sea);
			}
		}

		private void MenuBarTrackImportDialogRequested(object sender, EventArgs e)
		{
			this.ImportTrackDialog();
		}

		private void ImportTrackDialog()
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "SMK track file (*.mkt)|*.mkt|All files (*.*)|*.*";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					this.ImportTrack(ofd.FileName);
				}
			}
		}

		private void ImportTrack(string filePath)
		{
			try
			{
				this.track.Import(filePath, MainForm.SmkGame.Themes);
				this.trackTreeView.MarkTrackAsChanged();
				this.DisplayNewTrack();
			}
			catch (UnauthorizedAccessException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			catch (IOException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			catch (InvalidDataException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void MenuBarTrackExportDialogRequested(object sender, EventArgs e)
		{
			this.ExportTrackDialog();
		}

		private void ExportTrackDialog()
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter = "SMK track file (*.mkt)|*.mkt|All files (*.*)|*.*";
				string fileName = this.trackTreeView.SelectedTrackFileName;

				// Remove invalid filename characters
				string invalidChars = new string(Path.GetInvalidFileNameChars());
				invalidChars = "[" + Regex.Escape(invalidChars) + "]*";
				fileName = Regex.Replace(fileName, invalidChars, string.Empty);

				sfd.FileName = fileName;

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					this.ExportTrack(sfd.FileName);
				}
			}
		}

		private void ExportTrack(string filePath)
		{
			try
			{
				this.track.Export(filePath, (byte)this.tilesetControl.SelectedThemeIndex);
			}
			catch (UnauthorizedAccessException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			catch (IOException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void ResetZoom()
		{
			if (this.zoomLevelIndex == TrackEditor.DefaultZoomLevelIndex)
			{
				return;
			}

			this.zoomLevelIndex = TrackEditor.DefaultZoomLevelIndex;
			this.trackDrawer.SetZoom(this.Zoom);

			this.RecalculateScrollbarMaximums();
			this.RepaintTrackDisplay();
		}

		private void ZoomIn()
		{
			this.ZoomInSub();
			this.RepaintTrackDisplay();
		}

		private void ZoomOut()
		{
			this.ZoomOutSub();
			this.RepaintTrackDisplay();
		}

		private void ZoomInSub()
		{
			if (this.zoomLevelIndex < this.zoomLevels.Length - 1)
			{
				this.zoomLevelIndex++;
				this.trackDrawer.SetZoom(this.Zoom);
				this.RecalculateScrollbarMaximums();
			}
		}

		private void ZoomOutSub()
		{
			if (this.zoomLevelIndex > 0)
			{
				this.zoomLevelIndex--;
				this.trackDrawer.SetZoom(this.Zoom);
				this.RecalculateScrollbarMaximums();
			}
		}

		private void MenuBarZoomInRequested(object sender, EventArgs e)
		{
			this.ZoomIn();
		}

		private void MenuBarZoomOutRequested(object sender, EventArgs e)
		{
			this.ZoomOut();
		}

		private void MenuBarZoomResetRequested(object sender, EventArgs e)
		{
			this.ResetZoom();
		}

		private void RemoveFocus()
		{
			if (Form.ActiveForm != null) // If the application is focused
			{
				// Steal the focus from the panel to disable mouse-wheel scrolling
				this.menuBar.Focus();
			}
		}
		#endregion Menu Options

		#region TrackDisplay Events
		private void TrackDisplayPanelPaint(object sender, PaintEventArgs e)
		{
			this.RepaintTrackDisplay();
		}

		private void TrackDisplayPanelMouseEnter(object sender, EventArgs e)
		{
			this.trackDrawer.NotifyFullRepaintNeed();
		}

		private void TrackDisplayPanelEnter(object sender, EventArgs e)
		{
			this.trackDrawer.NotifyFullRepaintNeed();
		}

		private void TrackDisplayVScrollBarMouseMove(object sender, MouseEventArgs e)
		{
			if (Form.ActiveForm != null) // If the application is focused
			{
				this.trackDisplayVScrollBar.Focus(); // Lets you use the mouse wheel to scroll
			}
		}

		private void TrackDisplayVScrollBarMouseLeave(object sender, EventArgs e)
		{
			this.RemoveFocus();
		}

		private void TrackDisplayVScrollBarValueChanged(object sender, EventArgs e)
		{
			this.trackDrawer.NotifyFullRepaintNeed();

			if (this.buttonPressed != ActionButton.MiddleMouseButton)
			{
				this.RepaintTrackDisplay();
			}
		}

		private void TrackDisplayHScrollBarValueChanged(object sender, EventArgs e)
		{
			this.trackDrawer.NotifyFullRepaintNeed();

			if (this.buttonPressed != ActionButton.MiddleMouseButton)
			{
				this.RepaintTrackDisplay();
			}
		}

		private void UpdateScrollPositionX(object sender, EventArgs e)
		{
			// Unlike the TrackDisplayVScrollBarValueChanged event handler,
			// this one is never detached.
			this.scrollPosition.Y = this.trackDisplayVScrollBar.Value;
			this.trackDrawer.ScrollPosition = this.scrollPosition;
		}

		private void UpdateScrollPositionY(object sender, EventArgs e)
		{
			// Unlike the TrackDisplayHScrollBarValueChanged event handler,
			// this one is never detached.
			this.scrollPosition.X = this.trackDisplayHScrollBar.Value;
			this.trackDrawer.ScrollPosition = this.scrollPosition;
		}

		private void TrackDisplayPanelSizeChanged(object sender, EventArgs e)
		{
			this.trackDrawer.ResizeWindow(this.trackDisplayPanel);
			this.RecalculateScrollbarMaximums();
		}

		private void TrackDisplayPanelKeyDown(object sender, KeyEventArgs e)
		{
			if (this.buttonPressed != ActionButton.None)
			{
				return;
			}

			if (e.KeyCode == Keys.ControlKey)
			{
				this.buttonPressed = ActionButton.CtrlKey;
			}
			else if (e.KeyCode == Keys.Delete)
			{
				EditionMode currentMode = this.CurrentMode;
				if (currentMode == EditionMode.Overlay)
				{
					if (this.overlayControl.SelectedTile != null)
					{
						this.DeleteOverlayTile();
					}
				}
				else if (currentMode == EditionMode.AI)
				{
					if (this.aiControl.SelectedElement != null)
					{
						this.DeleteAIElement();
					}
				}
			}
		}

		private void TrackDisplayPanelKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey && this.buttonPressed == ActionButton.CtrlKey)
			{
				this.buttonPressed = ActionButton.None;
			}
		}

		private void TrackDisplayPanelMouseMove(object sender, MouseEventArgs e)
		{
			if (Form.ActiveForm != null) // If the application is focused
			{
				this.trackDisplayPanel.Focus(); // Lets you use the mouse wheel to scroll
			}

			Point tilePositionBefore = this.TilePosition;
			this.SetCurrentPosition(e.Location);

			if (tilePositionBefore == this.TilePosition) // If the cursor has not moved to another tile
			{
				if (this.CurrentMode == EditionMode.Start &&
					this.buttonPressed != ActionButton.MiddleMouseButton)
				{
					// The only mode that needs pixel precision,
					// as opposed to tile precision
					this.InitStartAction();
				}
			}
			else
			{
				if (this.buttonPressed == ActionButton.MiddleMouseButton)
				{
					this.ScrollTrack();
				}
				else
				{
					this.menuBar.UpdatePositionLabel(this.AbsoluteTilePosition);

					this.InitCurrentModeAction();
				}
			}
		}

		private void SetCurrentPosition(Point point)
		{
			int x = (int)(point.X / this.Zoom);
			int y = (int)(point.Y / this.Zoom);

			// We check that the new position isn't out of the track limits, if it is,
			// we set it to the lowest or highest (depending on case) possible coordinate
			if (x + this.scrollPosition.X * 8 >= this.track.Map.Width * 8)
			{
				x = this.track.Map.Width * 8 - 1 - this.scrollPosition.X * 8;
			}
			else if (x + this.scrollPosition.X * 8 < 0)
			{
				x = 0;
			}

			if (y + this.scrollPosition.Y * 8 >= this.track.Map.Height * 8)
			{
				y = this.track.Map.Height * 8 - 1 - this.scrollPosition.Y * 8;
			}
			else if (y + this.scrollPosition.Y * 8 < 0)
			{
				y = 0;
			}

			this.pixelPosition = new Point(x, y);
		}

		private void ScrollTrack()
		{
			int xBefore = this.scrollPosition.X;
			int yBefore = this.scrollPosition.Y;

			int xValue = this.anchorPoint.X - this.TilePosition.X;
			if (xValue < this.trackDisplayHScrollBar.Minimum)
			{
				this.trackDisplayHScrollBar.Value = 0;
			}
			else if (xValue > this.trackDisplayHScrollBar.Maximum - 9)
			{
				this.trackDisplayHScrollBar.Value = this.trackDisplayHScrollBar.Maximum < 9 ?
					0 : this.trackDisplayHScrollBar.Maximum - 9;
			}
			else
			{
				this.trackDisplayHScrollBar.Value = xValue;
			}

			int yValue = this.anchorPoint.Y - this.TilePosition.Y;
			if (yValue < this.trackDisplayVScrollBar.Minimum)
			{
				this.trackDisplayVScrollBar.Value = 0;
			}
			else if (yValue > this.trackDisplayVScrollBar.Maximum - 9)
			{
				this.trackDisplayVScrollBar.Value = this.trackDisplayVScrollBar.Maximum < 9 ?
					0 : this.trackDisplayVScrollBar.Maximum - 9;
			}
			else
			{
				this.trackDisplayVScrollBar.Value = yValue;
			}

			if (xBefore != this.scrollPosition.X ||
				yBefore != this.scrollPosition.Y)
			{
				this.RepaintTrackDisplay();
			}
		}

		private void RecalculateTileClipboard()
		{
			Point hoveredTilePosition = this.AbsoluteTilePosition;

			this.tileClipboardSize.Width = Math.Abs(hoveredTilePosition.X - this.anchorPoint.X) + 1;
			this.tileClipboardSize.Height = Math.Abs(hoveredTilePosition.Y - this.anchorPoint.Y) + 1;

			this.tileClipboardTopLeft.X = Math.Min(this.anchorPoint.X, hoveredTilePosition.X);
			this.tileClipboardTopLeft.Y = Math.Min(this.anchorPoint.Y, hoveredTilePosition.Y);

			this.RepaintTrackDisplay();
		}

		private void TrackDisplayPanelMouseLeave(object sender, EventArgs e)
		{
			this.Cursor = Cursors.Default;

			if (this.buttonPressed == ActionButton.RightMouseButton)
			{
				this.OnRightMouseButtonRelease();
			}

			// Cancel pressed mouse boutons (needed in case the panel lost focus unexpectedly)
			this.buttonPressed = ActionButton.None;

			this.RemoveFocus();

			this.ResetCurrentPosition();
			this.hoveredOverlayTile = null;
			this.selectedOverlayPatternLocation = new Point(-1, -1);
			this.hoveredObject = null;
			this.hoveredAIElem = null;

			this.RepaintTrackDisplay();
		}

		private void TrackDisplayPanelMouseDown(object sender, MouseEventArgs e)
		{
			// We only acknowledge the click if neither the left nor right mouse button is already pressed
			if (this.buttonPressed != ActionButton.None)
			{
				return;
			}

			EditionMode currentMode = this.CurrentMode;

			if (e.Button == MouseButtons.Middle)
			{
				this.buttonPressed = ActionButton.MiddleMouseButton;
				this.Cursor = Cursors.SizeAll;
				this.anchorPoint = this.AbsoluteTilePosition;
				this.RepaintTrackDisplay();
			}
			else if (currentMode == EditionMode.Tileset)
			{
				switch (e.Button)
				{
					case MouseButtons.Left:
						this.buttonPressed = ActionButton.LeftMouseButton;
						this.LayTiles();
						break;

					case MouseButtons.Right:
						byte? hoveredTile = this.GetHoveredTile();
						if (hoveredTile != null)
						{
							this.buttonPressed = ActionButton.RightMouseButton;

							if (this.tileClipboard[0] != hoveredTile)
							{
								this.tilesetControl.SelectedTile = (byte)hoveredTile;
							}

							this.tileClipboard.Clear();
							this.tileClipboard.Add((byte)hoveredTile);

							this.anchorPoint = this.tileClipboardTopLeft = this.AbsoluteTilePosition;

							this.tileClipboardSize.Width = this.tileClipboardSize.Height = 1;

							this.RepaintTrackDisplay();
						}
						break;
				}
			}
			else if (currentMode == EditionMode.Overlay)
			{
				switch (e.Button)
				{
					case MouseButtons.Left:
						if (this.overlayControl.SelectedPattern == null)
						{
							this.overlayControl.SelectedTile = this.hoveredOverlayTile;

							if (this.overlayControl.SelectedTile != null)
							{
								this.buttonPressed = ActionButton.LeftMouseButton;
								Point hoveredTilePosition = this.AbsoluteTilePosition;
								this.anchorPoint = new Point(hoveredTilePosition.X - this.overlayControl.SelectedTile.X,
															 hoveredTilePosition.Y - this.overlayControl.SelectedTile.Y);
							}
						}
						else
						{
							OverlayTile overlayTile = new OverlayTile(this.overlayControl.SelectedPattern, this.selectedOverlayPatternLocation);
							this.track.OverlayTiles.Add(overlayTile);
							this.UpdateOverlayTileCount();
							this.trackTreeView.MarkTrackAsChanged();
						}
						break;

					case MouseButtons.Right:
						if (this.hoveredOverlayTile == null)
						{
							this.overlayControl.SelectedPattern = null;
						}
						else
						{
							this.overlayControl.SelectedPattern = this.hoveredOverlayTile.Pattern;
							this.SetSelectedOverlayPatternLocation();
							this.hoveredOverlayTile = null;
							this.Cursor = Cursors.Default;
						}
						break;
				}

				this.RepaintTrackDisplay();
			}
			else if (currentMode == EditionMode.Start)
			{
				if (e.Button != MouseButtons.Left)
				{
					return;
				}

				if (this.track is GPTrack)
				{
					GPTrack gpTrack = this.track as GPTrack;

					Point absPixelPos = this.AbsolutePixelPosition;
					if (this.startAction == StartAction.DragStartPosition)
					{
						this.buttonPressed = ActionButton.LeftMouseButton;
						this.anchorPoint = new Point(absPixelPos.X - gpTrack.StartPosition.X,
													 absPixelPos.Y - gpTrack.StartPosition.Y);
					}
					else if (this.startAction != StartAction.None)
					{
						// ie: StartAction.DragLapLine, ResizeLapLine or DragAll
						this.buttonPressed = ActionButton.LeftMouseButton;
						this.anchorPoint = new Point(absPixelPos.X - gpTrack.LapLine.X,
													 absPixelPos.Y - gpTrack.LapLine.Y);
					}
				}
			}
			else if (currentMode == EditionMode.Objects)
			{
				if (this.hoveredObject == null)
				{
					return;
				}

				switch (e.Button)
				{
					case MouseButtons.Left:
						this.buttonPressed = ActionButton.LeftMouseButton;
						this.RepaintTrackDisplay();
						break;

					case MouseButtons.Right:
						if (!(this.hoveredObject is TrackObjectMatchRace))
						{
							break;
						}

						TrackObjectMatchRace hoveredObjectMatchRace = this.hoveredObject as TrackObjectMatchRace;

						if (hoveredObjectMatchRace.Direction == Direction.Horizontal)
						{
							hoveredObjectMatchRace.Direction = Direction.Vertical;
						}
						else if (hoveredObjectMatchRace.Direction == Direction.Vertical)
						{
							hoveredObjectMatchRace.Direction = Direction.None;
						}
						else // hoveredObjectMatchRace.Direction == Direction.None
						{
							hoveredObjectMatchRace.Direction = Direction.Horizontal;
						}

						this.trackTreeView.MarkTrackAsChanged();
						this.RepaintTrackDisplay();
						break;
				}
			}
			else if (currentMode == EditionMode.AI)
			{
				if (this.hoveredAIElem == null)
				{
					if (e.Button == MouseButtons.Left)
					{
						this.aiControl.SelectedElement = null;
						this.RepaintTrackDisplay();
					}
					return;
				}

				switch (e.Button)
				{
					case MouseButtons.Left:
						this.buttonPressed = ActionButton.LeftMouseButton;
						this.aiControl.SelectedElement = this.hoveredAIElem;
						this.RepaintTrackDisplay();
						break;

					case MouseButtons.Right:
						if (this.hoveredAIElem.Speed < 3)
						{
							this.hoveredAIElem.Speed++;
						}
						else
						{
							this.hoveredAIElem.Speed = 0;
						}

						this.aiControl.SelectedElement = this.hoveredAIElem;
						this.trackTreeView.MarkTrackAsChanged();
						this.RepaintTrackDisplay();
						break;
				}
			}
		}

		private void TrackDisplayPanelMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle && this.buttonPressed == ActionButton.MiddleMouseButton)
			{
				this.buttonPressed = ActionButton.None;

				if (this.CurrentMode == EditionMode.Tileset)
				{
					// For other modes, the cursor will be reset
					// by the call to the InitCurrentModeAction method below.
					// Tileset mode is a special case as it's the only mode
					// that doesn't change the cursor (middle mouse button aside).
					this.Cursor = Cursors.Default;
				}

				this.InitCurrentModeAction();
			}
			else if (this.CurrentMode == EditionMode.Tileset)
			{
				// When the user releases a mouse button, we ensure this button is the same
				// as the one that was initially hold down before validating the action
				if ((e.Button == MouseButtons.Left && this.buttonPressed == ActionButton.LeftMouseButton)
					|| (e.Button == MouseButtons.Right && this.buttonPressed == ActionButton.RightMouseButton))
				{
					this.buttonPressed = ActionButton.None;
					if (e.Button == MouseButtons.Right)
					{
						this.OnRightMouseButtonRelease();
					}
				}
			}
			else
			{
				this.buttonPressed = ActionButton.None;
			}
		}

		private void TrackDisplayPanelMouseWheel(object sender, MouseEventArgs e)
		{
			if (!this.trackDisplayPanel.Focused)
			{
				return;
			}

			if (this.buttonPressed == ActionButton.CtrlKey)
			{
				if (e.Delta > 0)
				{
					this.ZoomInSub();
				}
				else
				{
					this.ZoomOutSub();
				}

				this.UpdateDataAfterMouseWheel(e.Location);
			}
			else if (this.buttonPressed != ActionButton.MiddleMouseButton)
			{
				int newVsbVal = this.scrollPosition.Y - (e.Delta / 64);
				int limit = Math.Max(this.trackDisplayVScrollBar.Maximum - 9, this.trackDisplayVScrollBar.Minimum);
				// "-9", because unlike when using the scrollbars/arrows,
				// you can actually reach the scrollbar maximum value with the mousewheel

				// Below: detach and reattach vertical scrollbar ValueChanged event handler to avoid an extra repaint
				if (newVsbVal < 0)
				{
					if (this.scrollPosition.Y != 0)
					{
						this.ScrollOnMouseWheel(0, e.Location);
					}
				}
				else if (newVsbVal > limit)
				{
					if (this.scrollPosition.Y != limit)
					{
						this.ScrollOnMouseWheel(limit, e.Location);
					}
				}
				else
				{
					this.ScrollOnMouseWheel(newVsbVal, e.Location);
				}
			}
		}

		private void ScrollOnMouseWheel(int value, Point position)
		{
			this.trackDisplayVScrollBar.ValueChanged -= this.TrackDisplayVScrollBarValueChanged;
			this.trackDisplayVScrollBar.Value = value;
			this.trackDisplayVScrollBar.ValueChanged += this.TrackDisplayVScrollBarValueChanged;
			this.trackDrawer.NotifyFullRepaintNeed();
			this.UpdateDataAfterMouseWheel(position);
		}

		private void TrackDisplayPanelMouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (this.CurrentMode == EditionMode.AI &&
				e.Button == MouseButtons.Left &&
				this.hoveredAIElem == null)
			{
				this.AddAIElement();
			}
		}
		#endregion TrackDisplay Events

		#region TrackDisplay Methods
		private void DisplayNewTrack()
		{
			this.trackDrawer.LoadTrack(this.track);
			this.RepaintTrackDisplay();
		}

		private void RepaintTrackDisplay()
		{
			if (!this.trackDisplayPanel.Focused)
			{
				// Redraw the whole panel if the focus has been lost
				// There may be a better way to do this, by retrieving only the dirty region
				this.trackDrawer.NotifyFullRepaintNeed();
			}

			switch (this.CurrentMode)
			{
				case EditionMode.Tileset:
					this.trackDrawer.DrawTrackTileset(this.TilePosition, this.buttonPressed, this.tileClipboardSize, this.tileClipboardTopLeft);
					break;

				case EditionMode.Overlay:
					this.trackDrawer.DrawTrackOverlay(this.hoveredOverlayTile, this.overlayControl.SelectedTile, this.overlayControl.SelectedPattern, this.selectedOverlayPatternLocation);
					break;

				case EditionMode.Start:
					this.trackDrawer.DrawTrackStart();
					break;

				case EditionMode.Objects:
					this.trackDrawer.DrawTrackObjects(this.hoveredObject, this.objectsControl.FrontZonesView);
					break;

				case EditionMode.AI:
					this.trackDrawer.DrawTrackAI(this.hoveredAIElem, this.aiControl.SelectedElement, this.aiAction == AIAction.DragTarget);
					break;
			}
		}

		private void ResetCurrentPosition()
		{
			this.pixelPosition.X = this.pixelPosition.Y = -8;
			// -8, so that TilePosition returns -1

			this.menuBar.UpdatePositionLabel();
		}

		private void RecalculateScrollbarMaximums()
		{
			int offScreenTileCountX;
			int offScreenTileCountY;
			this.GetOffScreenTileCounts(out offScreenTileCountX, out offScreenTileCountY);

			// Recalculate the maximum value of the horizontal scrollbar
			this.trackDisplayHScrollBar.ValueChanged -= this.TrackDisplayHScrollBarValueChanged;
			TrackEditor.RecalculateScrollbarMaximum(this.trackDisplayHScrollBar, offScreenTileCountX);
			this.trackDisplayHScrollBar.ValueChanged += this.TrackDisplayHScrollBarValueChanged;

			// Recalculate the maximum value of the vertical scrollbar
			this.trackDisplayVScrollBar.ValueChanged -= this.TrackDisplayVScrollBarValueChanged;
			TrackEditor.RecalculateScrollbarMaximum(this.trackDisplayVScrollBar, offScreenTileCountY);
			this.trackDisplayVScrollBar.ValueChanged += this.TrackDisplayVScrollBarValueChanged;
		}

		private void GetOffScreenTileCounts(out int offScreenTileCountX, out int offScreenTileCountY)
		{
			offScreenTileCountX = this.GetOffScreenTileCount(this.trackDisplayPanel.Width);
			offScreenTileCountY = this.GetOffScreenTileCount(this.trackDisplayPanel.Height);
			int offScreenTileCountXWithScrollbar = this.GetOffScreenTileCount(this.trackDisplayPanel.Width - this.trackDisplayVScrollBar.Width);
			int offScreenTileCountYWithScrollbar = this.GetOffScreenTileCount(this.trackDisplayPanel.Height - this.trackDisplayHScrollBar.Height);

			bool? horizontalScrollbarNeeded = TrackEditor.IsScrollBarNeeded(offScreenTileCountX, offScreenTileCountXWithScrollbar);
			bool? verticalScrollbarNeeded = TrackEditor.IsScrollBarNeeded(offScreenTileCountY, offScreenTileCountYWithScrollbar);

			// Replace null (unsure) values with concrete values
			if (horizontalScrollbarNeeded == null &&
			    verticalScrollbarNeeded == null)
			{
				horizontalScrollbarNeeded = false;
				verticalScrollbarNeeded = false;
			}
			else if (horizontalScrollbarNeeded == null)
			{
				horizontalScrollbarNeeded = verticalScrollbarNeeded.Value;
			}
			else if (verticalScrollbarNeeded == null)
			{
				verticalScrollbarNeeded = horizontalScrollbarNeeded.Value;
			}

			// Update (increase) off-screen tile count if the other scrollbar is visible
			if (verticalScrollbarNeeded.Value)
			{
				offScreenTileCountX = offScreenTileCountXWithScrollbar;
			}

			if (horizontalScrollbarNeeded.Value)
			{
				offScreenTileCountY = offScreenTileCountYWithScrollbar;
			}
		}

		/// <summary>
		/// Checks whether the presence of the scrollbar is necessary.
		/// </summary>
		/// <param name="offScreenTileCount">The number of tiles off screen if the scrollbar is hidden.</param>
		/// <param name="offScreenTileCountWithScrollbar">The number of tiles off screen if the scrollbar is visible.</param>
		/// <returns>true = the scrollbar is necessary
		/// null = the scrollbar may be necessary (depending on the visibility of the other scrollbar)
		/// false = the scrollbar is not necessary</returns>
		private static bool? IsScrollBarNeeded(int offScreenTileCount, int offScreenTileCountWithScrollbar)
		{
			if (offScreenTileCount > 0)
			{
				// The scrollbar is necessary (must be visible)
				return true;
			}

			if (offScreenTileCountWithScrollbar > 0)
			{
				// The scrollbar is only necessary if the other scrollbar is visible
				return null;
			}

			// The scrollbar is not necessary (can be hidden)
			return false;
		}

		private int GetOffScreenTileCount(int panelSize)
		{
			int onscreenTileCount = (int)((panelSize) / (8 * this.Zoom));
			int offscreenTileCount = this.track.Map.Width - onscreenTileCount; // Map.Width = Map.Height
			return offscreenTileCount;
		}

		private static void RecalculateScrollbarMaximum(ScrollBar scrollbar, int offScreenTileCount)
		{
			// Show or hide the scrollbar depending on whether there are tiles off screen
			if (offScreenTileCount <= 0)
			{
				scrollbar.Visible = false;
				scrollbar.Maximum = 0;
			}
			else
			{
				scrollbar.Visible = true;

				// Trick to reposition the track content properly (avoiding to show off-track black)
				// when resizing the window and the panel is scrolled to the bottom and/or right limit
				if (!PlatformInformation.IsMono()) // HACK: Reposition track content on resize (.NET only)
				{
					scrollbar.Maximum = offScreenTileCount;
				}
				// End trick

				scrollbar.Maximum = offScreenTileCount + 9;
				// Adding 9 to the maximum, because somehow,
				// you can't scroll up to the maximum using the scrollbars
			}
		}

		private void UpdateDataAfterMouseWheel(Point position)
		{
			this.pixelPosition.X = (int)(position.X / this.Zoom);
			this.pixelPosition.Y = (int)(position.Y / this.Zoom);

			this.menuBar.UpdatePositionLabel(this.AbsoluteTilePosition);

			this.InitCurrentModeAction();

			EditionMode currentMode = this.CurrentMode;

			if (currentMode != EditionMode.Tileset &&
				currentMode != EditionMode.AI)
			{
				// In EditionMode.Tileset and AI, the call to InitCurrentModeAction above
				// already repaints the track display.
				this.RepaintTrackDisplay();
			}
		}

		private void InitCurrentModeAction()
		{
			switch (this.CurrentMode)
			{
				case EditionMode.Tileset:
					this.InitTilesetAction();
					break;

				case EditionMode.Overlay:
					this.InitOverlayAction();
					break;

				case EditionMode.Start:
					this.InitStartAction();
					break;

				case EditionMode.Objects:
					this.InitObjectAction();
					break;

				case EditionMode.AI:
					this.InitAIAction();
					break;
			}
		}
		#endregion TrackDisplay Methods

		#region Track TreeView
		public void RemoveModifiedHints()
		{
			this.trackTreeView.RemoveModifiedHints();
		}

		private void TrackTreeViewSelectedTrackChanged(object sender, EventArgs e)
		{
			this.ResetScrollingPosition();
			this.SetCurrentTrack();
			this.DisplayNewTrack();
			this.tilesetControl.SelectCurrentTrackTheme();
		}

		private void ResetScrollingPosition()
		{
			if (this.scrollPosition.Y != 0)
			{
				this.trackDisplayVScrollBar.ValueChanged -= this.TrackDisplayVScrollBarValueChanged;
				this.trackDisplayVScrollBar.Value = this.scrollPosition.Y;
				this.trackDisplayVScrollBar.ValueChanged += this.TrackDisplayVScrollBarValueChanged;
				this.trackDrawer.NotifyFullRepaintNeed();
			}

			if (this.scrollPosition.X != 0)
			{
				this.trackDisplayHScrollBar.ValueChanged -= this.TrackDisplayHScrollBarValueChanged;
				this.trackDisplayHScrollBar.Value = this.scrollPosition.X;
				this.trackDisplayHScrollBar.ValueChanged += this.TrackDisplayHScrollBarValueChanged;
				this.trackDrawer.NotifyFullRepaintNeed();
			}
		}

		private void SetCurrentTrack()
		{
			this.track = this.trackTreeView.SelectedTrack;

			this.tilesetControl.Track = this.track;
			this.hoveredOverlayTile = null;
			this.overlayControl.SelectedTile = null;
			this.UpdateOverlayTileCount();
			this.aiControl.TrackAI = this.track.AI;
			this.hoveredAIElem = null;

			if (this.CurrentMode == EditionMode.Objects)
			{
				this.SetTrackObjectZones();
			}

			if (this.track is GPTrack)
			{
				this.startTabPage.Enabled = true;
				this.startControl.Track = this.track;
			}
			else
			{
				this.startTabPage.Enabled = false;
			}
		}
		#endregion Track TreeView

		#region EditionMode Tabs
		private EditionMode CurrentMode
		{
			get
			{
				if (this.tilesetTabPage.Visible)
				{
					return EditionMode.Tileset;
				}
				else if (this.overlayTabPage.Visible)
				{
					return EditionMode.Overlay;
				}
				else if (this.startTabPage.Visible)
				{
					return EditionMode.Start;
				}
				else if (this.objectsTabPage.Visible)
				{
					return EditionMode.Objects;
				}
				else // if (this.aiTabPage.Visible)
				{
					return EditionMode.AI;
				}
			}
		}

		private void ModeTabControlSelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.CurrentMode == EditionMode.Objects)
			{
				this.SetTrackObjectZones();
			}

			this.RepaintTrackDisplay();
			this.ResizeModeTabControl();
		}

		private void ModeTabControlClientSizeChanged(object sender, EventArgs e)
		{
			int widthBefore = this.modeTabControl.Width;
			this.ResizeModeTabControl();
			int difference = this.modeTabControl.Width - widthBefore;

			if (difference != 0)
			{
				// Properly reposition the modeTabControl
				this.modeTabControl.Left -= difference;
			}
		}

		/// <summary>
		/// Adapt the modeTabControl width depending on whether its vertical scrollbar is visible.
		/// </summary>
		private void ResizeModeTabControl()
		{
			this.modeTabControl.Width =
				this.modeTabControl.SelectedTab.VerticalScroll.Visible ?
				144 + SystemInformation.VerticalScrollBarWidth : 144;
		}

		private void ModeTabPageMouseMove(object sender, MouseEventArgs e)
		{
			if (Form.ActiveForm != null) // If the application is focused
			{
				this.modeTabControl.SelectedTab.Focus(); // Lets you use the mouse wheel to scroll
			}
		}

		private void ModeTabPageMouseLeave(object sender, EventArgs e)
		{
			this.RemoveFocus();
		}
		#endregion EditionMode Tabs


		#region EditionMode.Tileset
		private void InitTilesetAction()
		{
			switch (this.buttonPressed)
			{
				case ActionButton.LeftMouseButton:
					this.LayTiles();
					break;

				case ActionButton.RightMouseButton:
					this.RecalculateTileClipboard();
					break;

				default:
					this.RepaintTrackDisplay();
					break;
			}
		}

		private byte? GetHoveredTile()
		{
			Point hoveredTilePosition = this.AbsoluteTilePosition;

			if (hoveredTilePosition.X >= 0 && hoveredTilePosition.Y >= 0 &&
			   hoveredTilePosition.X < this.track.Map.Width &&
			   hoveredTilePosition.Y < this.track.Map.Height)
			{
				return this.track.Map.GetTile(hoveredTilePosition);
			}
			return null;
		}

		private void LayTiles()
		{
			this.LayTiles(this.AbsoluteTilePosition);
		}

		private void LayTiles(Point hoveredTilePosition)
		{
			if (hoveredTilePosition.X >= 0 && hoveredTilePosition.Y >= 0 &&
			   hoveredTilePosition.X < this.track.Map.Width && hoveredTilePosition.Y < this.track.Map.Height)
			{
				Size affectedSurface = this.GetTruncatedRectangle();
				this.track.Map.SetTiles(hoveredTilePosition, affectedSurface, this.tileClipboardSize, this.tileClipboard);

				this.trackDrawer.UpdateCacheAfterTileLaying(hoveredTilePosition);

				this.trackTreeView.MarkTrackAsChanged();
				this.RepaintTrackDisplay();
			}
		}

		private Size GetTruncatedRectangle()
		{
			Size rectangleToDisplay = this.tileClipboardSize;
			if ((this.scrollPosition.X + this.TilePosition.X + rectangleToDisplay.Width) >= this.track.Map.Width)
			{
				int subFromWidth = this.scrollPosition.X + this.TilePosition.X + rectangleToDisplay.Width - this.track.Map.Width;
				rectangleToDisplay.Width -= subFromWidth;
			}
			if ((this.scrollPosition.Y + this.TilePosition.Y + rectangleToDisplay.Height) >= this.track.Map.Height)
			{
				int subFromHeight = this.scrollPosition.Y + this.TilePosition.Y + rectangleToDisplay.Height - this.track.Map.Height;
				rectangleToDisplay.Height -= subFromHeight;
			}
			return rectangleToDisplay;
		}

		private void OnRightMouseButtonRelease()
		{
			// Fill the clipboard with the selected tiles
			int xStart = this.tileClipboardTopLeft.X;
			int yStart = this.tileClipboardTopLeft.Y;
			int xLimit = xStart + this.tileClipboardSize.Width;
			int yLimit = yStart + this.tileClipboardSize.Height;
			for (int y = yStart; y < yLimit; y++)
			{
				for (int x = xStart; x < xLimit; x++)
				{
					this.tileClipboard.Add(this.track.Map[x, y]);
				}
			}
			this.trackDrawer.UpdateTileClipboard(xStart, yStart, this.tileClipboardSize);

			if (this.tileClipboard[0] != this.tileClipboard[1])
			{
				// The condition is to avoid triggering an unneeded redraw
				this.tilesetControl.SelectedTile = this.tileClipboard[1];
			}

			// We remove the first tile, which was added in TrackDisplayPanelMouseDown
			this.tileClipboard.RemoveAt(0);

			this.RepaintTrackDisplay();
		}

		private void TilesetControlTrackThemeChanged(object sender, EventArgs e)
		{
			this.trackTreeView.MarkTrackAsChanged();
			this.DisplayNewTrack();
		}

		private void TilesetControlSelectedThemeChanged(object sender, EventArgs e)
		{
			this.trackDrawer.UpdateTileClipboardOnThemeChange(this.tileClipboard, this.tileClipboardSize, this.track.GetRoadTileset());
			this.overlayControl.SetTileset(this.track.GetRoadTileset());
		}

		private void TilesetControlSelectedTileChanged(object sender, EventArgs e)
		{
			byte selectedTile = this.tilesetControl.SelectedTile;
			this.tileClipboard.Clear();
			this.tileClipboard.Add(selectedTile);
			this.tileClipboardSize.Width = this.tileClipboardSize.Height = 1;
			this.trackDrawer.UpdateTileClipboard(this.track.GetRoadTile(selectedTile));
		}

		private void TilesetControlTrackMapChanged(object sender, EventArgs e)
		{
			this.trackDrawer.LoadTrack(this.track);
			this.trackTreeView.MarkTrackAsChanged();
			this.RepaintTrackDisplay();
		}
		#endregion EditionMode.Tileset

		#region EditionMode.Overlay
		private void InitOverlayAction()
		{
			Point hoveredTilePosition = this.AbsoluteTilePosition;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				#region Drag overlay tile
				this.overlayControl.SelectedTile.Location =
					new Point(hoveredTilePosition.X - this.anchorPoint.X,
					          hoveredTilePosition.Y - this.anchorPoint.Y);
				this.trackTreeView.MarkTrackAsChanged();
				#endregion Drag overlay tile
			}
			else
			{
				if (this.overlayControl.SelectedPattern == null)
				{
					#region Try to hover overlay tile
					foreach (OverlayTile overlayTile in this.track.OverlayTiles)
					{
						if (overlayTile.IntersectsWith(hoveredTilePosition))
						{
							this.hoveredOverlayTile = overlayTile;
							this.Cursor = Cursors.Hand;
							this.RepaintTrackDisplay();
							return;
						}
					}

					this.hoveredOverlayTile = null;
					this.Cursor = Cursors.Default;
					#endregion Try to hover overlay tile
				}
				else
				{
					this.SetSelectedOverlayPatternLocation();
				}
			}

			this.RepaintTrackDisplay();
		}

		private void UpdateOverlayTileCount()
		{
			this.overlayControl.UpdateTileCount(this.track.OverlayTiles.Count);
		}

		private void DeleteOverlayTile()
		{
			this.track.OverlayTiles.Remove(this.overlayControl.SelectedTile);
			if (this.hoveredOverlayTile == this.overlayControl.SelectedTile)
			{
				this.hoveredOverlayTile = null;
			}
			this.overlayControl.SelectedTile = null;

			this.InitOverlayAction();

			this.UpdateOverlayTileCount();
			this.trackTreeView.MarkTrackAsChanged();
			this.RepaintTrackDisplay(); // May cause a second unnecessary repaint,
			// due to the InitOverlayAction call above. No big deal.
		}

		private void OverlayControlDeleteRequested(object sender, EventArgs e)
		{
			this.DeleteOverlayTile();
		}

		private void OverlayControlDeleteAllRequested(object sender, EventArgs e)
		{
			this.overlayControl.SelectedTile = null;
			this.track.OverlayTiles.Clear();
			this.UpdateOverlayTileCount();
			this.trackTreeView.MarkTrackAsChanged();
			this.RepaintTrackDisplay();
		}

		private void OverlayControlRepaintRequested(object sender, EventArgs e)
		{
			this.RepaintTrackDisplay();
		}

		private void SetSelectedOverlayPatternLocation()
		{
			OverlayTilePattern pattern = this.overlayControl.SelectedPattern;

			if (pattern == null)
			{
				return;
			}

			Point tilePosition = this.TilePosition;
			if (tilePosition.X == -1)
			{
				// The mouse cursor isn't over the track map
				this.selectedOverlayPatternLocation = tilePosition;
			}
			else
			{
				Point hoveredTilePosition = this.AbsoluteTilePosition;
				int x = hoveredTilePosition.X - pattern.Width / 2;
				int y = hoveredTilePosition.Y - pattern.Height / 2;

				if (x < 0)
				{
					x = 0;
				}
				else if (x + pattern.Width > this.track.Map.Width)
				{
					x = this.track.Map.Width - pattern.Width;
				}

				if (y < 0)
				{
					y = 0;
				}
				else if (y + pattern.Height > this.track.Map.Height)
				{
					y = this.track.Map.Height - pattern.Height;
				}

				this.selectedOverlayPatternLocation = new Point(x, y);
			}
		}
		#endregion EditionMode.Overlay
		
		#region EditionMode.Start
		private void InitStartAction()
		{
			if (this.track is GPTrack)
			{
				this.InitGPStartAction();
			}
		}

		private void InitGPStartAction()
		{
			GPTrack gpTrack = this.track as GPTrack;
			Point absPixelPos = this.AbsolutePixelPosition;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				bool dataChanged = false;

				switch (this.startAction)
				{
					case StartAction.DragLapLine:
						// Move lap line
						int step = this.startControl.Precision;
						Point destination = new Point(absPixelPos.X - this.anchorPoint.X,
													  ((absPixelPos.Y - this.anchorPoint.Y) / step) * step);

						int xBefore = gpTrack.LapLine.X;
						int yBefore = gpTrack.LapLine.Y;

						gpTrack.LapLine.MoveTo(destination.X, destination.Y);

						int xDifference = gpTrack.LapLine.X - xBefore;
						int yDifference = gpTrack.LapLine.Y - yBefore;

						if (xDifference != 0 || yDifference != 0)
						{
							dataChanged = true;
						}
						break;

					case StartAction.ResizeLapLine:
						// Resize lap line
						int lengthBefore = gpTrack.LapLine.Length;

						gpTrack.LapLine.Resize(this.resizeHandle, absPixelPos.X);

						if (lengthBefore != gpTrack.LapLine.Length)
						{
							dataChanged = true;
						}
						break;

					case StartAction.DragStartPosition:
						// Move driver starting position
						step = this.startControl.Precision;
						destination = new Point(((absPixelPos.X - this.anchorPoint.X) / step) * step,
												((absPixelPos.Y - this.anchorPoint.Y) / step) * step);

						xBefore = gpTrack.StartPosition.X;
						yBefore = gpTrack.StartPosition.Y;

						gpTrack.StartPosition.MoveTo(destination.X, destination.Y);

						xDifference = gpTrack.StartPosition.X - xBefore;
						yDifference = gpTrack.StartPosition.Y - yBefore;

						if (xDifference != 0 || yDifference != 0)
						{
							dataChanged = true;
						}
						break;

					case StartAction.DragAll:
						// Move both the lap line and the driver starting position
						step = this.startControl.Precision;
						destination = new Point(absPixelPos.X - this.anchorPoint.X,
												((absPixelPos.Y - this.anchorPoint.Y) / step) * step);

						xBefore = gpTrack.LapLine.X;
						yBefore = gpTrack.LapLine.Y;

						gpTrack.LapLine.MoveTo(destination.X, destination.Y);

						xDifference = gpTrack.LapLine.X - xBefore;
						yDifference = gpTrack.LapLine.Y - yBefore;

						if (xDifference != 0 || yDifference != 0)
						{
							destination = new Point(gpTrack.StartPosition.X + xDifference,
													gpTrack.StartPosition.Y + yDifference);

							gpTrack.StartPosition.MoveTo(destination.X, destination.Y);
							dataChanged = true;
						}
						break;
				}

				if (dataChanged)
				{
					this.trackTreeView.MarkTrackAsChanged();
					this.RepaintTrackDisplay();
				}
			}
			else
			{
				if (gpTrack.LapLine.IntersectsWith(absPixelPos))
				{
					this.resizeHandle = gpTrack.LapLine.GetResizeHandle(absPixelPos);

					if (this.resizeHandle == ResizeHandle.None)
					{
						this.startAction = this.startControl.LapLineAndDriverPositionsBound ?
							StartAction.DragAll : StartAction.DragLapLine;
						this.Cursor = Cursors.SizeAll;
					}
					else
					{
						this.startAction = StartAction.ResizeLapLine;
						this.Cursor = Cursors.SizeWE;
					}
				}
				else if (gpTrack.StartPosition.IntersectsWith(absPixelPos))
				{
					this.startAction = this.startControl.LapLineAndDriverPositionsBound ?
						StartAction.DragAll : StartAction.DragStartPosition;
					this.Cursor = Cursors.SizeAll;
				}
				else
				{
					this.startAction = StartAction.None;
					this.Cursor = Cursors.Default;
				}
			}
		}

		private void StartControlDataChanged(object sender, EventArgs e)
		{
			this.trackTreeView.MarkTrackAsChanged();
			this.RepaintTrackDisplay();
		}
		#endregion EditionMode.Start

		#region EditionMode.Objects
		private void InitObjectAction()
		{
			if (!(this.track is GPTrack))
			{
				return;
			}

			Point hoveredTilePosition = this.AbsoluteTilePosition;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				#region Drag object
				this.hoveredObject.Location = hoveredTilePosition;
				this.trackTreeView.MarkTrackAsChanged();
				this.RepaintTrackDisplay();
				#endregion Drag object
			}
			else
			{
				#region Try to hover object
				GPTrack gpTrack = this.track as GPTrack;

				if (gpTrack.Objects == null)
				{
					return;
				}

				foreach (TrackObject trackObject in gpTrack.Objects)
				{
					if (trackObject.X == hoveredTilePosition.X &&
						trackObject.Y == hoveredTilePosition.Y)
					{
						this.hoveredObject = trackObject;
						this.Cursor = Cursors.Hand;
						return;
					}
				}

				this.hoveredObject = null;
				this.Cursor = Cursors.Default;
				#endregion Try to hover object
			}
		}

		private void SetTrackObjectZones()
		{
			if (this.track is GPTrack &&
				(this.track as GPTrack).ObjectZones != null)
			{
				GPTrack gpTrack = this.track as GPTrack;
				this.objectsTabPage.Enabled = true;
				this.objectsControl.Track = gpTrack;
			}
			else
			{
				this.objectsTabPage.Enabled = false;
			}
		}

		private void ObjectsControlObjectZonesViewChanged(object sender, EventArgs e)
		{
			this.RepaintTrackDisplay();
		}

		private void ObjectsControlObjectZonesChanged(object sender, EventArgs e)
		{
			this.trackTreeView.MarkTrackAsChanged();
			this.RepaintTrackDisplay();
		}
		#endregion EditionMode.Objects

		#region EditionMode.AI
		private void InitAIAction()
		{
			Point hoveredTilePosition = this.AbsoluteTilePosition;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				#region Drag or resize AI element
				bool dataChanged = false;

				if (this.aiAction == AIAction.DragTarget)
				{
					// Drag AI target
					this.hoveredAIElem.Target = hoveredTilePosition;
					dataChanged = true;
				}
				else if (this.aiAction == AIAction.DragZone)
				{
					// Drag AI zone
					int xBefore = this.hoveredAIElem.Zone.X;
					int yBefore = this.hoveredAIElem.Zone.Y;

					this.hoveredAIElem.MoveTo(hoveredTilePosition.X - this.anchorPoint.X,
											  hoveredTilePosition.Y - this.anchorPoint.Y);

					int xAfter = this.hoveredAIElem.Zone.X;
					int yAfter = this.hoveredAIElem.Zone.Y;

					if (xBefore != xAfter || yBefore != yAfter)
					{
						dataChanged = true;
					}
				}
				else if (this.aiAction == AIAction.ResizeZone)
				{
					// Resize AI zone
					int widthBefore = this.hoveredAIElem.Zone.Width;
					int heightBefore = this.hoveredAIElem.Zone.Height;

					this.hoveredAIElem.Resize(this.resizeHandle,
											  hoveredTilePosition.X,
											  hoveredTilePosition.Y);

					int widthAfter = this.hoveredAIElem.Zone.Width;
					int heightAfter = this.hoveredAIElem.Zone.Height;

					if (widthBefore != widthAfter || heightBefore != heightAfter)
					{
						dataChanged = true;
					}
				}

				if (dataChanged)
				{
					this.trackTreeView.MarkTrackAsChanged();
					this.RepaintTrackDisplay();
				}
				#endregion Drag AI element
			}
			else
			{
				#region Try to hover AI target
				foreach (TrackAIElement trackAIElem in this.track.AI)
				{
					if (trackAIElem.Target.X == hoveredTilePosition.X &&
						trackAIElem.Target.Y == hoveredTilePosition.Y)
					{
						// Hover AI target
						this.hoveredAIElem = trackAIElem;
						this.aiAction = AIAction.DragTarget;
						this.Cursor = Cursors.Hand;
						this.RepaintTrackDisplay();
						return;
					}
				}
				#endregion Try to hover AI target

				#region Priority to selected element
				if (this.aiControl.SelectedElement != null &&
					this.TryToHoverAIZone(this.aiControl.SelectedElement, hoveredTilePosition))
				{
					// If an element is already selected, and that it's hovered,
					// don't try to hover anything else
					return;
				}
				#endregion Priority to selected element

				#region Try to hover AI zone
				if (this.hoveredAIElem != null &&
					this.TryToHoverAIZone(this.hoveredAIElem, hoveredTilePosition))
				{
					// If an element is already hovered,
					// don't try to hover anything else
					return;
				}

				foreach (TrackAIElement trackAIElem in this.track.AI)
				{
					if (this.TryToHoverAIZone(trackAIElem, hoveredTilePosition))
					{
						return;
					}
				}
				#endregion Try to hover AI zone

				this.hoveredAIElem = null;
				this.Cursor = Cursors.Default;
				this.RepaintTrackDisplay();
			}
		}

		private bool TryToHoverAIZone(TrackAIElement trackAIElem, Point hoveredTilePosition)
		{
			if (trackAIElem.IntersectsWith(hoveredTilePosition))
			{
				// Hover AI zone
				this.hoveredAIElem = trackAIElem;

				if (this.hoveredAIElem != this.aiControl.SelectedElement)
				{
					this.aiAction = AIAction.DragZone;
					this.Cursor = Cursors.SizeAll;
					this.SetAIElementAnchorPoint(hoveredTilePosition);
				}
				else
				{
					this.resizeHandle = this.aiControl.SelectedElement.GetResizeHandle(hoveredTilePosition);

					if (this.resizeHandle == ResizeHandle.None)
					{
						this.aiAction = AIAction.DragZone;
						this.Cursor = Cursors.SizeAll;
						this.SetAIElementAnchorPoint(hoveredTilePosition);
					}
					else
					{
						this.aiAction = AIAction.ResizeZone;

						switch (this.resizeHandle)
						{
							case ResizeHandle.TopLeft:
								this.Cursor = Cursors.SizeNWSE;
								break;

							case ResizeHandle.Top:
								this.Cursor = Cursors.SizeNS;
								break;

							case ResizeHandle.TopRight:
								this.Cursor = Cursors.SizeNESW;
								break;

							case ResizeHandle.Right:
								this.Cursor = Cursors.SizeWE;
								break;

							case ResizeHandle.BottomRight:
								this.Cursor = Cursors.SizeNWSE;
								break;

							case ResizeHandle.Bottom:
								this.Cursor = Cursors.SizeNS;
								break;

							case ResizeHandle.BottomLeft:
								this.Cursor = Cursors.SizeNESW;
								break;

							case ResizeHandle.Left:
								this.Cursor = Cursors.SizeWE;
								break;
						}
					}
				}

				this.RepaintTrackDisplay();
				return true;
			}

			return false;
		}

		private void SetAIElementAnchorPoint(Point hoveredTilePosition)
		{
			hoveredTilePosition.X = (hoveredTilePosition.X / 2) * 2;
			hoveredTilePosition.Y = (hoveredTilePosition.Y / 2) * 2;
			this.anchorPoint = new Point(hoveredTilePosition.X - this.hoveredAIElem.Zone.X,
										 hoveredTilePosition.Y - this.hoveredAIElem.Zone.Y);
		}

		private void AddAIElement()
		{
			Point position = this.AbsoluteTilePosition;
			TrackAIElement newAIElem = new TrackAIElement(position);

			if (this.track.AI.Add(newAIElem))
			{
				this.aiControl.SetMaximumAIElementIndex();
				this.aiControl.SelectedElement = newAIElem;
				this.InitAIAction();

				this.trackTreeView.MarkTrackAsChanged();
			}
		}

		private void DeleteAIElement()
		{
			this.track.AI.Remove(this.aiControl.SelectedElement);
			if (this.hoveredAIElem == this.aiControl.SelectedElement)
			{
				this.hoveredAIElem = null;
			}

			this.aiControl.SelectedElement = null;
			this.aiControl.SetMaximumAIElementIndex();
			this.InitAIAction();

			this.trackTreeView.MarkTrackAsChanged();
		}

		private void AIControlDataChanged(object sender, EventArgs e)
		{
			this.trackTreeView.MarkTrackAsChanged();
			this.RepaintTrackDisplay();
		}

		private void AIControlDeleteRequested(object sender, EventArgs e)
		{
			this.DeleteAIElement();
		}

		private void AIControlDeleteAllRequested(object sender, EventArgs e)
		{
			this.aiControl.SelectedElement = null;
			this.track.AI.Clear();
			this.trackTreeView.MarkTrackAsChanged();
			this.RepaintTrackDisplay();
		}
		#endregion EditionMode.AI
	}
}
