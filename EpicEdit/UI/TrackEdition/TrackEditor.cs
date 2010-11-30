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
		/// The current edition mode.
		/// </summary>
		private EditionMode currentMode = EditionMode.Tileset;

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

		/// <summary>
		/// Flag to determine whether to repaint the track display
		/// when the scrolling position has changed.
		/// </summary>
		private bool repaintAfterScrolling = false;

		// Which pixel the cursor is on (doesn't take scrolling position in consideration).
		private Point pixelPosition;

		// Which pixel the cursor is on (takes scrolling position in consideration).
		private Point AbsolutePixelPosition
		{
			get
			{
				return new Point(this.scrollPosition.X * 8 + (int)(this.pixelPosition.X / this.Zoom),
								 this.scrollPosition.Y * 8 + (int)(this.pixelPosition.Y / this.Zoom));
			}
		}

		/// <summary>
		/// Which tile the cursor is on (doesn't take scrolling position in consideration).
		/// </summary>
		private Point TilePosition
		{
			get
			{
				if (this.pixelPosition.X == -1)
				{
					// The mouse cursor isn't over the track
					return this.pixelPosition;
				}

				return new Point((int)(this.pixelPosition.X / (8 * this.Zoom)),
								 (int)(this.pixelPosition.Y / (8 * this.Zoom)));
			}
		}

		/// <summary>
		/// Which tile the cursor is on (takes scrolling position in consideration).
		/// </summary>
		private Point AbsoluteTilePosition
		{
			get
			{
				return new Point(this.scrollPosition.X + this.TilePosition.X,
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
			DragStartPosition2,
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
		public event EventHandler<EventArgs<string>> FileDragged;

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

		[Browsable(true)]
		public event EventHandler<EventArgs> ToggleScreenModeRequested
		{
			add { this.menuBar.ToggleScreenModeRequested += value; }
			remove { this.menuBar.ToggleScreenModeRequested -= value; }
		}

		public TrackEditor()
		{
			this.InitializeComponent();

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

			this.RecalculateScrollBarMaximums();

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
				this.FileDragged(this, sea);
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
				ofd.Filter =
					"Full track (*.smkc)|*.smkc|" +
					"Track map only (*.mkt)|*.mkt|" +
					"All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					this.ImportTrack(ofd.FileName);
				}
			}
		}

		public void ImportTrack(string filePath)
		{
			try
			{
				this.track.Import(filePath, MainForm.SmkGame);
				this.trackTreeView.MarkTrackAsChanged();
				this.UpdateControlsOnTrackImport();
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

		private void UpdateControlsOnTrackImport()
		{
			this.tilesetControl.SelectCurrentTrackTheme();
			this.SetCurrentTrackSub();
		}

		private void MenuBarTrackExportDialogRequested(object sender, EventArgs e)
		{
			this.ExportTrackDialog();
		}

		private void ExportTrackDialog()
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter =
					"Full track (*.smkc)|*.smkc|" +
					"Track map only (*.mkt)|*.mkt|" +
					"All files (*.*)|*.*";

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
				this.track.Export(filePath, MainForm.SmkGame);
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

			this.RecalculateScrollBarMaximums();
			this.trackDisplayPanel.Invalidate();
		}

		private void ZoomIn()
		{
			if (!this.CanZoomIn())
			{
				return;
			}

			Point location = this.GetCenterTileLocation();
			this.ZoomInSub();
			this.CenterTrackDisplayOn(location);

			this.EndZoom();
		}

		private void ZoomOut()
		{
			if (!this.CanZoomOut())
			{
				return;
			}

			Point location = this.GetCenterTileLocation();
			this.ZoomOutSub();
			this.CenterTrackDisplayOn(location);

			this.EndZoom();
		}

		private Point GetCenterTileLocation()
		{
			return new Point(this.scrollPosition.X + Math.Min(this.GetOnScreenTileCount(this.trackDisplayPanel.Width), this.track.Map.Width) / 2,
							 this.scrollPosition.Y + Math.Min(this.GetOnScreenTileCount(this.trackDisplayPanel.Height), this.track.Map.Height) / 2);
		}

		private void EndZoom()
		{
			if (this.pixelPosition.X == -1)
			{
				// The cursor isn't over the track
				this.trackDisplayPanel.Invalidate();
			}
			else
			{
				this.InitCurrentModeAction(true);
			}
		}

		private void MouseWheelZoom(MouseEventArgs e)
		{
			Point hoveredTilePosition;
			
			if (e.Delta > 0)
			{
				if (!this.CanZoomIn())
				{
					return;
				}
				hoveredTilePosition = this.AbsoluteTilePosition;
				this.ZoomInSub();
			}
			else
			{
				if (!this.CanZoomOut())
				{
					return;
				}
				hoveredTilePosition = this.AbsoluteTilePosition;
				this.ZoomOutSub();
			}

			this.CenterTrackDisplayOn(hoveredTilePosition);

			this.InitCurrentModeAction(true);
		}

		private void CenterTrackDisplayOn(Point location)
		{
			int x = location.X - (this.GetOnScreenTileCount(this.trackDisplayPanel.Width) / 2);
			int y = location.Y - (this.GetOnScreenTileCount(this.trackDisplayPanel.Height) / 2);

			this.SetHorizontalScrollingValue(x);
			this.SetVerticalScrollingValue(y);
		}

		private void ZoomInSub()
		{
			this.zoomLevelIndex++;
			this.trackDrawer.SetZoom(this.Zoom);
			this.RecalculateScrollBarMaximums();

			this.menuBar.ZoomInEnabled = this.CanZoomIn();
			this.menuBar.ZoomOutEnabled = true;
		}

		private void ZoomOutSub()
		{
			this.zoomLevelIndex--;
			this.trackDrawer.SetZoom(this.Zoom);
			this.RecalculateScrollBarMaximums();

			this.menuBar.ZoomInEnabled = true;
			this.menuBar.ZoomOutEnabled = this.CanZoomOut();
		}

		private bool CanZoomIn()
		{
			return this.zoomLevelIndex < this.zoomLevels.Length - 1;
		}

		private bool CanZoomOut()
		{
			return this.zoomLevelIndex > 0;
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
			if (!this.trackDisplayPanel.Focused)
			{
				// Redraw the whole panel if the focus has been lost
				// There may be a better way to do this, by retrieving only the dirty region
				this.trackDrawer.NotifyFullRepaintNeed();
			}

			Graphics gfx = e.Graphics;
			switch (this.currentMode)
			{
				case EditionMode.Tileset:
					this.trackDrawer.DrawTrackTileset(gfx, this.TilePosition, this.buttonPressed, this.tileClipboardSize, this.tileClipboardTopLeft);
					break;

				case EditionMode.Overlay:
					this.trackDrawer.DrawTrackOverlay(gfx, this.hoveredOverlayTile, this.overlayControl.SelectedTile, this.overlayControl.SelectedPattern, this.selectedOverlayPatternLocation);
					break;

				case EditionMode.Start:
					this.trackDrawer.DrawTrackStart(gfx);
					break;

				case EditionMode.Objects:
					this.trackDrawer.DrawTrackObjects(gfx, this.hoveredObject, this.objectsControl.FrontZonesView);
					break;

				case EditionMode.AI:
					this.trackDrawer.DrawTrackAI(gfx, this.hoveredAIElem, this.aiControl.SelectedElement, this.aiAction == AIAction.DragTarget);
					break;
			}
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
			this.scrollPosition.Y = this.trackDisplayVScrollBar.Value;
			this.trackDrawer.ScrollPosition = this.scrollPosition;
			this.RepaintAfterScrollingIfNeeded();
		}

		private void TrackDisplayVScrollBarScroll(object sender, ScrollEventArgs e)
		{
			if (e.OldValue != e.NewValue)
			{
				this.repaintAfterScrolling = true;
			}
		}

		private void TrackDisplayHScrollBarValueChanged(object sender, EventArgs e)
		{
			this.scrollPosition.X = this.trackDisplayHScrollBar.Value;
			this.trackDrawer.ScrollPosition = this.scrollPosition;
			this.RepaintAfterScrollingIfNeeded();
		}

		private void TrackDisplayHScrollBarScroll(object sender, ScrollEventArgs e)
		{
			if (e.OldValue != e.NewValue)
			{
				this.repaintAfterScrolling = true;
			}
		}

		private void SetHorizontalScrollingValue(int x)
		{
			if (x < this.trackDisplayHScrollBar.Minimum)
			{
				this.trackDisplayHScrollBar.Value = this.trackDisplayHScrollBar.Minimum;
			}
			else if (x > this.trackDisplayHScrollBar.Maximum - (this.trackDisplayHScrollBar.LargeChange - 1))
			{
				// The inclusion of the LargeChange - 1 part in the calculation
				// is due to the fact it's not possible for users to reach the scroll bar maximum.
				// See: http://msdn.microsoft.com/en-us/library/system.windows.forms.scrollbar.maximum.aspx
				this.trackDisplayHScrollBar.Value = Math.Max(this.trackDisplayHScrollBar.Minimum,
				                                             this.trackDisplayHScrollBar.Maximum - (this.trackDisplayHScrollBar.LargeChange - 1));
			}
			else
			{
				this.trackDisplayHScrollBar.Value = x;
			}
		}

		private void SetVerticalScrollingValue(int y)
		{
			if (y < this.trackDisplayVScrollBar.Minimum)
			{
				this.trackDisplayVScrollBar.Value = this.trackDisplayVScrollBar.Minimum;
			}
			else if (y > this.trackDisplayVScrollBar.Maximum - (this.trackDisplayVScrollBar.LargeChange - 1))
			{
				// The inclusion of the LargeChange - 1 part in the calculation
				// is due to the fact it's not possible for users to reach the scroll bar maximum.
				// See: http://msdn.microsoft.com/en-us/library/system.windows.forms.scrollbar.maximum.aspx
				this.trackDisplayVScrollBar.Value = Math.Max(this.trackDisplayVScrollBar.Minimum,
				                                             this.trackDisplayVScrollBar.Maximum - (this.trackDisplayVScrollBar.LargeChange - 1));
			}
			else
			{
				this.trackDisplayVScrollBar.Value = y;
			}
		}

		private void RepaintAfterScrollingIfNeeded()
		{
			if (this.repaintAfterScrolling)
			{
				this.trackDrawer.NotifyFullRepaintNeed();
				this.trackDisplayPanel.Invalidate();
				this.repaintAfterScrolling = false;
			}
		}

		private void TrackDisplayPanelSizeChanged(object sender, EventArgs e)
		{
			this.trackDrawer.ResizeWindow();
			this.RecalculateScrollBarMaximums();
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
				if (this.currentMode == EditionMode.Overlay)
				{
					if (this.overlayControl.SelectedTile != null)
					{
						this.DeleteOverlayTile();
					}
				}
				else if (this.currentMode == EditionMode.AI)
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
				if (this.currentMode == EditionMode.Start &&
					this.buttonPressed != ActionButton.MiddleMouseButton)
				{
					// The only mode that needs pixel precision,
					// as opposed to tile precision
					if (this.InitStartAction())
					{
						this.trackDisplayPanel.Invalidate();
					}
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
					this.InitCurrentModeAction();
				}
			}
		}

		private void SetCurrentPosition(Point location)
		{
			int x = location.X;
			int y = location.Y;
			int zoomedTileSize = (int)(8 * this.Zoom);
			int zoomedScrollPositionX = this.scrollPosition.X * zoomedTileSize;
			int zoomedScrollPositionY = this.scrollPosition.Y * zoomedTileSize;
			int zoomedTrackWidth = this.track.Map.Width * zoomedTileSize;
			int zoomedTrackHeight = this.track.Map.Height * zoomedTileSize;
			int absoluteX = x + zoomedScrollPositionX;
			int absoluteY = y + zoomedScrollPositionY;

			// We check that the new position isn't out of the track limits, if it is,
			// we set it to the lowest or highest (depending on case) possible coordinate
			if (absoluteX < 0)
			{
				x = 0;
			}
			else if (absoluteX >= zoomedTrackWidth)
			{
				x = zoomedTrackWidth - 1 - zoomedScrollPositionX;
			}

			if (absoluteY < 0)
			{
				y = 0;
			}
			else if (absoluteY >= zoomedTrackHeight)
			{
				y = zoomedTrackHeight - 1 - zoomedScrollPositionY;
			}

			this.pixelPosition = new Point(x, y);
		}

		private void ScrollTrack()
		{
			int xBefore = this.scrollPosition.X;
			int yBefore = this.scrollPosition.Y;

			this.SetHorizontalScrollingValue(this.anchorPoint.X - this.TilePosition.X);
			this.SetVerticalScrollingValue(this.anchorPoint.Y - this.TilePosition.Y);

			if (xBefore != this.scrollPosition.X ||
				yBefore != this.scrollPosition.Y)
			{
				this.trackDrawer.NotifyFullRepaintNeed();
				this.trackDisplayPanel.Invalidate();
			}
		}

		private void RecalculateTileClipboard()
		{
			Point hoveredTilePosition = this.AbsoluteTilePosition;

			this.tileClipboardSize.Width = Math.Abs(hoveredTilePosition.X - this.anchorPoint.X) + 1;
			this.tileClipboardSize.Height = Math.Abs(hoveredTilePosition.Y - this.anchorPoint.Y) + 1;

			this.tileClipboardTopLeft.X = Math.Min(this.anchorPoint.X, hoveredTilePosition.X);
			this.tileClipboardTopLeft.Y = Math.Min(this.anchorPoint.Y, hoveredTilePosition.Y);
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

			this.trackDisplayPanel.Invalidate();
		}

		private void TrackDisplayPanelMouseDown(object sender, MouseEventArgs e)
		{
			// We only acknowledge the click if neither the left nor right mouse button is already pressed
			if (this.buttonPressed != ActionButton.None)
			{
				return;
			}

			if (e.Button == MouseButtons.Middle)
			{
				this.buttonPressed = ActionButton.MiddleMouseButton;
				this.Cursor = Cursors.SizeAll;
				this.anchorPoint = this.AbsoluteTilePosition;
				this.trackDisplayPanel.Invalidate();
			}
			else if (this.currentMode == EditionMode.Tileset)
			{
				switch (e.Button)
				{
					case MouseButtons.Left:
						this.buttonPressed = ActionButton.LeftMouseButton;
						if (this.LayTiles())
						{
							this.trackDisplayPanel.Invalidate();
						}
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

							this.trackDisplayPanel.Invalidate();
						}
						break;
				}
			}
			else if (this.currentMode == EditionMode.Overlay)
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

				this.trackDisplayPanel.Invalidate();
			}
			else if (this.currentMode == EditionMode.Start)
			{
				if (e.Button != MouseButtons.Left ||
					this.startAction == StartAction.None)
				{
					return;
				}

				this.buttonPressed = ActionButton.LeftMouseButton;
				Point absPixelPos = this.AbsolutePixelPosition;

				if (this.track is GPTrack)
				{
					GPTrack gpTrack = this.track as GPTrack;

					if (this.startAction == StartAction.DragStartPosition)
					{
						this.anchorPoint = new Point(absPixelPos.X - gpTrack.StartPosition.X,
													 absPixelPos.Y - gpTrack.StartPosition.Y);
					}
					else
					{
						// ie: StartAction.DragLapLine, ResizeLapLine or DragAll
						this.anchorPoint = new Point(absPixelPos.X - gpTrack.LapLine.X,
													 absPixelPos.Y - gpTrack.LapLine.Y);
					}
				}
				else
				{
					BattleTrack bTrack = this.track as BattleTrack;

					if (this.startAction == StartAction.DragStartPosition)
					{
						this.anchorPoint = new Point(absPixelPos.X - bTrack.StartPositionP1.X,
													 absPixelPos.Y - bTrack.StartPositionP1.Y);
					}
					else
					{
						this.anchorPoint = new Point(absPixelPos.X - bTrack.StartPositionP2.X,
													 absPixelPos.Y - bTrack.StartPositionP2.Y);
					}
				}
			}
			else if (this.currentMode == EditionMode.Objects)
			{
				if (this.hoveredObject == null)
				{
					return;
				}

				switch (e.Button)
				{
					case MouseButtons.Left:
						this.buttonPressed = ActionButton.LeftMouseButton;
						this.trackDisplayPanel.Invalidate();
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
						this.trackDisplayPanel.Invalidate();
						break;
				}
			}
			else if (this.currentMode == EditionMode.AI)
			{
				if (this.hoveredAIElem == null)
				{
					if (e.Button == MouseButtons.Left)
					{
						this.aiControl.SelectedElement = null;
						this.trackDisplayPanel.Invalidate();
					}
					return;
				}

				switch (e.Button)
				{
					case MouseButtons.Left:
						this.buttonPressed = ActionButton.LeftMouseButton;
						this.aiControl.SelectedElement = this.hoveredAIElem;
						this.trackDisplayPanel.Invalidate();
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
						this.trackDisplayPanel.Invalidate();
						break;
				}
			}
		}

		private void TrackDisplayPanelMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle && this.buttonPressed == ActionButton.MiddleMouseButton)
			{
				this.buttonPressed = ActionButton.None;

				if (this.currentMode == EditionMode.Tileset ||
					this.track is BattleTrack && this.currentMode == EditionMode.Objects)
				{
					// For other modes, the cursor will be reset
					// by the call to the InitCurrentModeAction method below.
					this.Cursor = Cursors.Default;
				}

				this.InitCurrentModeAction();
			}
			else if (this.currentMode == EditionMode.Tileset)
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
				this.MouseWheelZoom(e);
			}
			else if (this.buttonPressed != ActionButton.MiddleMouseButton)
			{
				this.MouseWheelScroll(e);
			}
		}

		private void MouseWheelScroll(MouseEventArgs e)
		{
			int y = this.scrollPosition.Y - (e.Delta / 64);
			int yBefore = this.scrollPosition.Y;
			this.SetVerticalScrollingValue(y);
			int yAfter = this.scrollPosition.Y;

			if (yBefore != yAfter)
			{
				this.trackDrawer.NotifyFullRepaintNeed();
				this.InitCurrentModeAction(true);
			}
		}

		private void TrackDisplayPanelMouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (this.currentMode == EditionMode.AI &&
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
			this.trackDisplayPanel.Invalidate();
		}

		private void ResetCurrentPosition()
		{
			this.pixelPosition = new Point(-1, -1);
			this.menuBar.ResetCoordinates();
		}

		private void RecalculateScrollBarMaximums()
		{
			int offScreenTileCountX;
			int offScreenTileCountY;
			this.GetOffScreenTileCounts(out offScreenTileCountX, out offScreenTileCountY);

			// Recalculate the maximum value of the horizontal scroll bar
			TrackEditor.RecalculateScrollBarMaximum(this.trackDisplayHScrollBar, offScreenTileCountX);

			// Recalculate the maximum value of the vertical scroll bar
			TrackEditor.RecalculateScrollBarMaximum(this.trackDisplayVScrollBar, offScreenTileCountY);
		}

		private void GetOffScreenTileCounts(out int offScreenTileCountX, out int offScreenTileCountY)
		{
			offScreenTileCountX = this.GetOffScreenTileCount(this.trackDisplayPanel.Width);
			offScreenTileCountY = this.GetOffScreenTileCount(this.trackDisplayPanel.Height);
			int offScreenTileCountXWithScrollBar = this.GetOffScreenTileCount(this.trackDisplayPanel.Width - this.trackDisplayVScrollBar.Width);
			int offScreenTileCountYWithScrollBar = this.GetOffScreenTileCount(this.trackDisplayPanel.Height - this.trackDisplayHScrollBar.Height);

			bool? horizontalScrollBarNeeded = TrackEditor.IsScrollBarNeeded(offScreenTileCountX, offScreenTileCountXWithScrollBar);
			bool? verticalScrollBarNeeded = TrackEditor.IsScrollBarNeeded(offScreenTileCountY, offScreenTileCountYWithScrollBar);

			// Replace null (unsure) values with concrete values
			if (horizontalScrollBarNeeded == null &&
				verticalScrollBarNeeded == null)
			{
				horizontalScrollBarNeeded = false;
				verticalScrollBarNeeded = false;
			}
			else if (horizontalScrollBarNeeded == null)
			{
				horizontalScrollBarNeeded = verticalScrollBarNeeded.Value;
			}
			else if (verticalScrollBarNeeded == null)
			{
				verticalScrollBarNeeded = horizontalScrollBarNeeded.Value;
			}

			// Update (increase) off-screen tile count if the other scroll bar is visible
			if (verticalScrollBarNeeded.Value)
			{
				offScreenTileCountX = offScreenTileCountXWithScrollBar;
			}

			if (horizontalScrollBarNeeded.Value)
			{
				offScreenTileCountY = offScreenTileCountYWithScrollBar;
			}
		}

		/// <summary>
		/// Checks whether the presence of the scroll bar is necessary.
		/// </summary>
		/// <param name="offScreenTileCount">The number of tiles off screen if the scroll bar is hidden.</param>
		/// <param name="offScreenTileCountWithScrollBar">The number of tiles off screen if the scroll bar is visible.</param>
		/// <returns>true = the scroll bar is necessary
		/// null = the scroll bar may be necessary (depending on the visibility of the other scroll bar)
		/// false = the scroll bar is not necessary</returns>
		private static bool? IsScrollBarNeeded(int offScreenTileCount, int offScreenTileCountWithScrollBar)
		{
			if (offScreenTileCount > 0)
			{
				// The scroll bar is necessary (must be visible)
				return true;
			}

			if (offScreenTileCountWithScrollBar > 0)
			{
				// The scroll bar is only necessary if the other scroll bar is visible
				return null;
			}

			// The scroll bar is not necessary (can be hidden)
			return false;
		}

		private int GetOffScreenTileCount(int panelSize)
		{
			int offscreenTileCount = this.track.Map.Width - this.GetOnScreenTileCount(panelSize); // Map.Width = Map.Height
			return offscreenTileCount;
		}

		private int GetOnScreenTileCount(int panelSize)
		{
			return (int)((panelSize) / (8 * this.Zoom));
		}

		private static void RecalculateScrollBarMaximum(ScrollBar scrollBar, int offScreenTileCount)
		{
			// Show or hide the scroll bar depending on whether there are tiles off screen
			if (offScreenTileCount <= 0)
			{
				scrollBar.Visible = false;
				scrollBar.Maximum = 0;
			}
			else
			{
				scrollBar.Visible = true;

				// Trick to reposition the track content properly (avoiding to show off-track black)
				// when resizing the window and the panel is scrolled to the bottom and/or right limit
				if (!PlatformInformation.IsMono()) // HACK: Reposition track content on resize (.NET only)
				{
					scrollBar.Maximum = offScreenTileCount;
				}
				// End trick

				scrollBar.Maximum = offScreenTileCount + (scrollBar.LargeChange - 1);
				// The inclusion of the LargeChange - 1 part in the calculation
				// is due to the fact it's not possible for users to reach the scroll bar maximum.
				// See: http://msdn.microsoft.com/en-us/library/system.windows.forms.scrollbar.maximum.aspx
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
			int xBefore = this.scrollPosition.X;
			int yBefore = this.scrollPosition.Y;

			this.trackDisplayHScrollBar.Value = 0;
			this.trackDisplayVScrollBar.Value = 0;

			if (xBefore != this.scrollPosition.X ||
				yBefore != this.scrollPosition.Y)
			{
				this.trackDrawer.NotifyFullRepaintNeed();
			}
		}

		private void SetCurrentTrack()
		{
			this.track = this.trackTreeView.SelectedTrack;

			this.tilesetControl.Track = this.track;
			this.SetCurrentTrackSub();

			this.startControl.Track = this.track;
		}

		private void SetCurrentTrackSub()
		{
			this.hoveredOverlayTile = null;
			this.overlayControl.SelectedTile = null;
			this.UpdateOverlayTileCount();
			this.aiControl.TrackAI = this.track.AI;
			this.hoveredAIElem = null;

			if (this.currentMode == EditionMode.Objects)
			{
				this.SetTrackObjectZones();
			}
		}
		#endregion Track TreeView

		#region EditionMode Tabs
		/// <summary>
		/// Init an action depending on the current edition mode.
		/// </summary>
		private void InitCurrentModeAction()
		{
			this.InitCurrentModeAction(false);
		}

		/// <summary>
		/// Init an action depending on the current edition mode.
		/// </summary>
		/// <param name="forceRepaint">If true: trigger a repaint after the action, whether something has changed or not.
		/// If false: only repaint if something has visually changed on the track.</param>
		private void InitCurrentModeAction(bool forceRepaint)
		{
			bool repaintNeeded;

			switch (this.currentMode)
			{
				case EditionMode.Tileset:
					repaintNeeded = this.InitTilesetAction();
					break;

				case EditionMode.Overlay:
					repaintNeeded = this.InitOverlayAction();
					break;

				case EditionMode.Start:
					repaintNeeded = this.InitStartAction();
					break;

				case EditionMode.Objects:
					repaintNeeded = this.InitObjectAction();
					break;

				case EditionMode.AI:
					repaintNeeded = this.InitAIAction();
					break;

				default:
					repaintNeeded = false;
					break;
			}

			if (repaintNeeded || forceRepaint)
			{
				this.trackDisplayPanel.Invalidate();
			}

			this.menuBar.UpdateCoordinates(this.AbsoluteTilePosition);
		}

		private void SetCurrentMode()
		{
			if (this.tilesetTabPage.Visible)
			{
				this.currentMode = EditionMode.Tileset;
			}
			else if (this.overlayTabPage.Visible)
			{
				this.currentMode = EditionMode.Overlay;
			}
			else if (this.startTabPage.Visible)
			{
				this.currentMode = EditionMode.Start;
			}
			else if (this.objectsTabPage.Visible)
			{
				this.currentMode = EditionMode.Objects;
			}
			else // if (this.aiTabPage.Visible)
			{
				this.currentMode = EditionMode.AI;
			}
		}

		private void ModeTabControlSelectedIndexChanged(object sender, EventArgs e)
		{
			this.SetCurrentMode();

			if (this.currentMode == EditionMode.Objects)
			{
				this.SetTrackObjectZones();
			}

			this.ResizeModeTabControl();

			this.trackDisplayPanel.Invalidate();
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
		/// Adapt the modeTabControl width depending on whether its vertical scroll bar is visible.
		/// </summary>
		private void ResizeModeTabControl()
		{
			this.modeTabControl.Width =
				this.modeTabControl.SelectedTab.VerticalScroll.Visible ?
				144 + SystemInformation.VerticalScrollBarWidth : 144;
		}
		#endregion EditionMode Tabs


		#region EditionMode.Tileset
		private bool InitTilesetAction()
		{
			bool repaintNeeded;

			switch (this.buttonPressed)
			{
				case ActionButton.LeftMouseButton:
					repaintNeeded = this.LayTiles();
					break;

				case ActionButton.RightMouseButton:
					this.RecalculateTileClipboard();
					repaintNeeded = true;
					break;

				default:
					repaintNeeded = true;
					break;
			}

			return repaintNeeded;
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

		private bool LayTiles()
		{
			return this.LayTiles(this.AbsoluteTilePosition);
		}

		private bool LayTiles(Point hoveredTilePosition)
		{
			if (hoveredTilePosition.X >= 0 && hoveredTilePosition.Y >= 0 &&
				hoveredTilePosition.X < this.track.Map.Width && hoveredTilePosition.Y < this.track.Map.Height)
			{
				Size affectedSurface = this.GetTruncatedRectangle();
				this.track.Map.SetTiles(hoveredTilePosition, affectedSurface, this.tileClipboardSize, this.tileClipboard);

				this.trackDrawer.UpdateCacheAfterTileLaying(hoveredTilePosition);

				this.trackTreeView.MarkTrackAsChanged();
				return true;
			}

			return false;
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

			this.trackDisplayPanel.Invalidate();
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
			this.trackDisplayPanel.Invalidate();
		}
		#endregion EditionMode.Tileset

		#region EditionMode.Overlay
		private bool InitOverlayAction()
		{
			Point hoveredTilePosition = this.AbsoluteTilePosition;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				// Drag overlay tile
				this.overlayControl.SelectedTile.Location =
					new Point(hoveredTilePosition.X - this.anchorPoint.X,
							  hoveredTilePosition.Y - this.anchorPoint.Y);
				this.trackTreeView.MarkTrackAsChanged();
				return true;
			}

			if (this.overlayControl.SelectedPattern == null)
			{
				// Try to hover overlay tile
				foreach (OverlayTile overlayTile in this.track.OverlayTiles)
				{
					if (overlayTile.IntersectsWith(hoveredTilePosition))
					{
						this.Cursor = Cursors.Hand;

						if (this.hoveredOverlayTile == overlayTile)
						{
							return false;
						}

						this.hoveredOverlayTile = overlayTile;
						return true;
					}
				}

				this.Cursor = Cursors.Default;

				if (this.hoveredOverlayTile == null)
				{
					return false;
				}

				this.hoveredOverlayTile = null;
				return true;
			}

			Point originalPatternLocation = this.selectedOverlayPatternLocation;
			this.SetSelectedOverlayPatternLocation();
			// Return whether the location has changed
			return originalPatternLocation.X != this.selectedOverlayPatternLocation.X ||
				   originalPatternLocation.Y != this.selectedOverlayPatternLocation.Y;
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
			this.trackDisplayPanel.Invalidate();
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
			this.trackDisplayPanel.Invalidate();
		}

		private void OverlayControlRepaintRequested(object sender, EventArgs e)
		{
			this.trackDisplayPanel.Invalidate();
		}

		private void SetSelectedOverlayPatternLocation()
		{
			OverlayTilePattern pattern = this.overlayControl.SelectedPattern;

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
		private bool InitStartAction()
		{
			if (this.track is GPTrack)
			{
				return this.InitGPStartAction();
			}

			return this.InitBattleStartAction();
		}

		private bool InitGPStartAction()
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

						gpTrack.LapLine.Location = destination;

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

						gpTrack.StartPosition.Location = destination;

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

						gpTrack.LapLine.Location = destination;

						xDifference = gpTrack.LapLine.X - xBefore;
						yDifference = gpTrack.LapLine.Y - yBefore;

						if (xDifference != 0 || yDifference != 0)
						{
							destination = new Point(gpTrack.StartPosition.X + xDifference,
													gpTrack.StartPosition.Y + yDifference);

							gpTrack.StartPosition.Location = destination;
							dataChanged = true;
						}
						break;
				}

				if (dataChanged)
				{
					this.trackTreeView.MarkTrackAsChanged();
				}

				return dataChanged;
			}

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

			return false;
		}

		private bool InitBattleStartAction()
		{
			BattleTrack bTrack = this.track as BattleTrack;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				BattleStartPosition position = this.startAction == StartAction.DragStartPosition ?
					bTrack.StartPositionP1 : bTrack.StartPositionP2;

				return this.InitBattleStartActionSub(position);
			}

			Point absPixelPos = this.AbsolutePixelPosition;

			if (bTrack.StartPositionP1.IntersectsWith(absPixelPos))
			{
				this.startAction = StartAction.DragStartPosition;
				this.Cursor = Cursors.Hand;
			}
			else if (bTrack.StartPositionP2.IntersectsWith(absPixelPos))
			{
				this.startAction = StartAction.DragStartPosition2;
				this.Cursor = Cursors.Hand;
			}
			else
			{
				this.startAction = StartAction.None;
				this.Cursor = Cursors.Default;
			}

			return false;
		}

		private bool InitBattleStartActionSub(BattleStartPosition position)
		{
			Point absPixelPos = this.AbsolutePixelPosition;

			int step = this.startControl.Precision;
			Point destination = new Point(((absPixelPos.X - this.anchorPoint.X) / step) * step,
										 ((absPixelPos.Y - this.anchorPoint.Y) / step) * step);

			int xBefore = position.X;
			int yBefore = position.Y;

			position.Location = destination;

			int xDifference = position.X - xBefore;
			int yDifference = position.Y - yBefore;

			bool dataChanged = xDifference != 0 || yDifference != 0;

			if (dataChanged)
			{
				this.trackTreeView.MarkTrackAsChanged();
			}

			return dataChanged;
		}

		private void StartControlDataChanged(object sender, EventArgs e)
		{
			this.trackTreeView.MarkTrackAsChanged();
			this.trackDisplayPanel.Invalidate();
		}
		#endregion EditionMode.Start

		#region EditionMode.Objects
		private bool InitObjectAction()
		{
			if (!(this.track is GPTrack))
			{
				return false;
			}

			Point hoveredTilePosition = this.AbsoluteTilePosition;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				// Drag object
				this.hoveredObject.Location = hoveredTilePosition;
				this.trackTreeView.MarkTrackAsChanged();
				return true;
			}

			// Try to hover object
			GPTrack gpTrack = this.track as GPTrack;

			if (gpTrack.Objects == null) // Ghost Valley pillar objects (not supported)
			{
				this.Cursor = Cursors.Default;
				return false;
			}

			foreach (TrackObject trackObject in gpTrack.Objects)
			{
				if (trackObject.X == hoveredTilePosition.X &&
					trackObject.Y == hoveredTilePosition.Y)
				{
					this.hoveredObject = trackObject;
					this.Cursor = Cursors.Hand;
					return false;
				}
			}

			this.Cursor = Cursors.Default;
			this.hoveredObject = null;
			return false;
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
			this.trackDisplayPanel.Invalidate();
		}

		private void ObjectsControlObjectZonesChanged(object sender, EventArgs e)
		{
			this.trackTreeView.MarkTrackAsChanged();
			this.trackDisplayPanel.Invalidate();
		}
		#endregion EditionMode.Objects

		#region EditionMode.AI
		private bool InitAIAction()
		{
			Point hoveredTilePosition = this.AbsoluteTilePosition;

			if (this.buttonPressed == ActionButton.LeftMouseButton)
			{
				// Drag or resize AI element
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

					this.hoveredAIElem.Location =
						new Point(hoveredTilePosition.X - this.anchorPoint.X,
								  hoveredTilePosition.Y - this.anchorPoint.Y);

					if (xBefore != this.hoveredAIElem.Zone.X ||
						yBefore != this.hoveredAIElem.Zone.Y)
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
				}

				return dataChanged;
			}

			// Try to hover AI target
			foreach (TrackAIElement trackAIElem in this.track.AI)
			{
				if (trackAIElem.Target.X == hoveredTilePosition.X &&
					trackAIElem.Target.Y == hoveredTilePosition.Y)
				{
					// Hover AI target
					this.hoveredAIElem = trackAIElem;
					this.aiAction = AIAction.DragTarget;
					this.Cursor = Cursors.Hand;
					return true;
				}
			}

			// Priority to selected element
			if (this.aiControl.SelectedElement != null &&
				this.TryToHoverAIZone(this.aiControl.SelectedElement, hoveredTilePosition))
			{
				// If an element is already selected, and that it's hovered,
				// don't try to hover anything else
				return false;
			}

			// Try to hover AI zone
			if (this.hoveredAIElem != null &&
				this.TryToHoverAIZone(this.hoveredAIElem, hoveredTilePosition))
			{
				// If an element is already hovered,
				// don't try to hover anything else
				return false;
			}

			foreach (TrackAIElement trackAIElem in this.track.AI)
			{
				if (this.TryToHoverAIZone(trackAIElem, hoveredTilePosition))
				{
					return true;
				}
			}

			this.Cursor = Cursors.Default;

			if (this.hoveredAIElem == null)
			{
				return false;
			}

			this.hoveredAIElem = null;
			return true;
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
			Point location = this.AbsoluteTilePosition;
			TrackAIElement newAIElem = new TrackAIElement(location);

			if (this.track.AI.Add(newAIElem))
			{
				this.aiControl.SetMaximumAIElementIndex();
				this.aiControl.SelectedElement = newAIElem;
				this.InitAIAction();

				this.trackTreeView.MarkTrackAsChanged();
				this.trackDisplayPanel.Invalidate();

				if (this.track.AI.ElementCount == 1)
				{
					this.aiControl.HideWarning();
				}
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
			this.trackDisplayPanel.Invalidate();

			if (this.track.AI.ElementCount == 0)
			{
				this.aiControl.ShowWarning();
			}
		}

		private void AIControlDataChanged(object sender, EventArgs e)
		{
			this.trackTreeView.MarkTrackAsChanged();
			this.trackDisplayPanel.Invalidate();
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
			this.trackDisplayPanel.Invalidate();
			this.aiControl.ShowWarning();
		}
		#endregion EditionMode.AI
	}
}