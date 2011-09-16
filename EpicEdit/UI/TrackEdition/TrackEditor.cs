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
using System.Windows.Forms.VisualStyles;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.ThemeEdition;
using EpicEdit.UI.Tools;
using EpicEdit.UI.Tools.UndoRedo;

namespace EpicEdit.UI.TrackEdition
{
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

        private enum EditionMode
        {
            Tileset,
            Overlay,
            Start,
            Objects,
            AI
        }

        /// <summary>
        /// The current edition mode.
        /// </summary>
        private EditionMode editionMode = EditionMode.Tileset;

        /// <summary>
        /// All the available zoom levels.
        /// </summary>
        private float[] zoomLevels;

        /// <summary>
        /// The index to the current zoom level.
        /// </summary>
        private int zoomLevelIndex;

        /// <summary>
        /// Gets or sets the index to the current zoom level.
        /// </summary>
        private int ZoomLevelIndex
        {
            get { return this.zoomLevelIndex; }
            set
            {
                this.zoomLevelIndex = value;
                this.trackDisplay.Zoom = this.Zoom;
            }
        }

        /// <summary>
        /// The index to the default zoom level (x1).
        /// </summary>
        private const int DefaultZoomLevelIndex = 3;

        /// <summary>
        /// The current zoom level of the track display.
        /// </summary>
        private float Zoom
        {
            get { return this.zoomLevels[this.ZoomLevelIndex]; }
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
                return new Point(this.scrollPosition.X * Tile.Size + (int)(this.pixelPosition.X / this.Zoom),
                                 this.scrollPosition.Y * Tile.Size + (int)(this.pixelPosition.Y / this.Zoom));
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

                return new Point((int)(this.pixelPosition.X / (Tile.Size * this.Zoom)),
                                 (int)(this.pixelPosition.Y / (Tile.Size * this.Zoom)));
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
        /// The position of the tile displayed in the center of the track panel
        /// (doesn't take scrolling position in consideration).
        /// </summary>
        private Point CenterTileLocation
        {
            get
            {
                return new Point(this.GetOnScreenTileCount(this.trackDisplay.Width) / 2,
                                 this.GetOnScreenTileCount(this.trackDisplay.Height) / 2);
            }
        }

        /// <summary>
        /// The position of the tile displayed in the center of the track panel
        /// (takes scrolling position in consideration).
        /// </summary>
        private Point AbsoluteCenterTileLocation
        {
            get
            {
                Point point = this.CenterTileLocation;
                point.Offset(this.scrollPosition);
                return point;
            }
        }

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
        /// Defines which mouse buttons are currently pressed, if any.
        /// </summary>
        private MouseButtons buttonsPressed;

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
        private StartAction startAction;

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
        private AIAction aiAction;

        /// <summary>
        /// Undo/redo buffers for tile changes, for each track.
        /// </summary>
        private Dictionary<Track, UndoRedoBuffer> undoRedoBuffers;

        /// <summary>
        /// Undo/redo buffer for tile changes, for the current track.
        /// </summary>
        private UndoRedoBuffer undoRedoBuffer
        {
            get
            {
                return this.undoRedoBuffers[this.track];
            }
        }

        private bool paletteFormInitialized;

        private PaletteEditorForm paletteForm;
        #endregion Private members

        #region Events
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
        #endregion Events

        #region Constructor
        public TrackEditor()
        {
            this.InitializeComponent();

            this.ResetPosition();

            this.zoomLevels = new float[] { .25f, .5f, .75f, 1, 2, 3, 4, 5, 6, 7, 8 };
            this.ZoomLevelIndex = TrackEditor.DefaultZoomLevelIndex;

            this.tileClipboard = new List<byte>();
            this.tileClipboard.Add(this.tilesetControl.SelectedTile);
            this.tileClipboardSize.Width = this.tileClipboardSize.Height = 1;

            this.undoRedoBuffers = new Dictionary<Track, UndoRedoBuffer>();

            if (VisualStyleRenderer.IsSupported)
            {
                // Force background color to fix the look of TrackBar controls
                foreach (TabPage page in this.modeTabControl.TabPages)
                {
                    // NOTE: Can't retrieve the actual tab background color dynamically,
                    // so use something close enough.
                    page.BackColor = SystemColors.ControlLightLight;
                }
            }
        }
        #endregion Constructor

        #region MenuBar
        public void InitOnFirstRomLoad()
        {
            this.trackDrawer = new TrackDrawer(this.trackDisplay, this.Zoom);
            this.tilesetControl.InitOnFirstRomLoad();
            this.overlayControl.InitOnFirstRomLoad();
            this.trackTreeView.InitOnFirstRomLoad();

            this.SetTrack();
            this.trackDrawer.LoadTrack(this.track);

            // Adding these event handlers here rather than in the Designer.cs
            // saves us a null check on this.drawer in each of the corresponding functions,
            // because the drawer hasn't been initialized yet before a ROM is loaded.
            this.trackDisplay.Paint += this.TrackDisplayPaint;
            this.trackDisplay.SizeChanged += this.TrackDisplaySizeChanged;

            this.UpdateScrollBars();

            this.trackDisplay.Enabled = true;
            this.modeTabControl.Enabled = true;
            this.menuBar.EnableControls();

            this.InitUndoRedo();
        }

        public void InitOnRomLoad()
        {
            this.tilesetControl.InitOnRomLoad();
            this.overlayControl.InitOnRomLoad();
            this.trackTreeView.InitOnRomLoad();
            this.ReInitPaletteEditor();

            foreach (var buffer in this.undoRedoBuffers.Values)
            {
                buffer.Clear();
            }
            this.undoRedoBuffers.Clear();
            this.InitUndoRedo();
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
                this.track.Import(filePath, Context.Game);

                this.undoRedoBuffer.Clear();
                this.menuBar.UndoEnabled = false;
                this.menuBar.RedoEnabled = false;

                this.trackTreeView.MarkTrackAsChanged();
                this.UpdateControlsOnTrackImport();
                this.DisplayNewTrack();
            }
            catch (UnauthorizedAccessException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                UITools.ShowError(ex.Message);
            }
        }

        private void UpdateControlsOnTrackImport()
        {
            this.SetTrack();
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
                this.track.Export(filePath, Context.Game);
            }
            catch (UnauthorizedAccessException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                UITools.ShowError(ex.Message);
            }
        }

        private void MenuBarUndoRequested(object sender, EventArgs e)
        {
            TileChange change = this.undoRedoBuffer.NextUndo;
            this.undoRedoBuffer.Undo();
            this.menuBar.RedoEnabled = true;
            this.EnableDisableUndo();
            this.EndUndoRedo(change);
        }

        private void MenuBarRedoRequested(object sender, EventArgs e)
        {
            TileChange change = this.undoRedoBuffer.NextRedo;
            this.undoRedoBuffer.Redo();
            this.menuBar.UndoEnabled = true;
            this.EnableDisableRedo();
            this.EndUndoRedo(change);
        }

        private void EnableDisableUndo()
        {
            this.menuBar.UndoEnabled = this.undoRedoBuffer.HasUndo;
        }

        private void EnableDisableRedo()
        {
            this.menuBar.RedoEnabled = this.undoRedoBuffer.HasRedo;
        }

        private void EnableDisableUndoRedo()
        {
            if (!this.undoRedoBuffers.ContainsKey(this.track))
            {
                this.InitUndoRedo();
            }
            else
            {
                if (this.editionMode == EditionMode.Tileset)
                {
                    this.EnableDisableUndo();
                    this.EnableDisableRedo();
                }
                else
                {
                    this.menuBar.UndoEnabled = false;
                    this.menuBar.RedoEnabled = false;
                }
            }
        }

        /// <summary>
        /// Init undo/redo buffer for the current track.
        /// </summary>
        private void InitUndoRedo()
        {
            this.undoRedoBuffers.Add(this.track, new UndoRedoBuffer(this.track));
            this.menuBar.UndoEnabled = false;
            this.menuBar.RedoEnabled = false;
        }

        private void EndUndoRedo(TileChange change)
        {
            this.trackTreeView.MarkTrackAsChanged();
            this.trackDrawer.ReloadTrackPart(change);
            this.trackDisplay.Invalidate();
        }

        private void ResetZoom()
        {
            if (this.ZoomLevelIndex == TrackEditor.DefaultZoomLevelIndex)
            {
                return;
            }

            Point location = this.AbsoluteCenterTileLocation;

            this.ZoomLevelIndex = TrackEditor.DefaultZoomLevelIndex;
            this.trackDrawer.SetZoom(this.Zoom);
            this.UpdateScrollBars();

            this.menuBar.ZoomInEnabled = true;
            this.menuBar.ZoomOutEnabled = true;

            this.CenterTrackDisplayOn(location);

            this.trackDisplay.Invalidate();
        }

        private void ZoomIn()
        {
            if (!this.CanZoomIn())
            {
                return;
            }

            Point location = this.AbsoluteCenterTileLocation;
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

            Point location = this.AbsoluteCenterTileLocation;
            this.ZoomOutSub();
            this.CenterTrackDisplayOn(location);

            this.EndZoom();
        }

        private void EndZoom()
        {
            if (this.pixelPosition.X == -1)
            {
                // The cursor isn't over the track
                this.trackDisplay.Invalidate();
            }
            else
            {
                this.InitEditionModeAction(true);
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

            // Ensure the user will still be hovering the same tile after zooming
            Point cursor = this.TilePosition;
            Point center = this.CenterTileLocation;
            Point diff = new Point(center.X - cursor.X, center.Y - cursor.Y);
            hoveredTilePosition.Offset(diff);

            this.CenterTrackDisplayOn(hoveredTilePosition);

            this.InitEditionModeAction(true);
        }

        private void CenterTrackDisplayOn(Point location)
        {
            Point point = this.CenterTileLocation;
            int x = location.X - point.X;
            int y = location.Y - point.Y;
            this.SetHorizontalScrollingValue(x);
            this.SetVerticalScrollingValue(y);
        }

        private void ZoomInSub()
        {
            this.ZoomLevelIndex++;
            this.trackDrawer.SetZoom(this.Zoom);
            this.UpdateScrollBars();

            this.menuBar.ZoomInEnabled = this.CanZoomIn();
            this.menuBar.ZoomOutEnabled = true;
        }

        private void ZoomOutSub()
        {
            this.ZoomLevelIndex--;
            this.trackDrawer.SetZoom(this.Zoom);
            this.UpdateScrollBars();

            this.menuBar.ZoomInEnabled = true;
            this.menuBar.ZoomOutEnabled = this.CanZoomOut();
        }

        private bool CanZoomIn()
        {
            return this.ZoomLevelIndex < this.zoomLevels.Length - 1;
        }

        private bool CanZoomOut()
        {
            return this.ZoomLevelIndex > 0;
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
            if (Form.ActiveForm != null) // Application focused
            {
                // Steal the focus from the panel to disable mouse-wheel scrolling
                this.menuBar.Focus();
            }
        }

        private void MenuBarPaletteEditorRequested(object sender, EventArgs e)
        {
            if (!this.paletteFormInitialized)
            {
                this.InitPaletteEditorForm();
            }

            this.paletteForm.Show();
            this.paletteForm.Editor.Theme = this.track.Theme;
        }

        private void InitPaletteEditorForm()
        {
            if (this.paletteForm == null)
            {
                this.paletteForm = new PaletteEditorForm();
                this.paletteForm.Owner = this.ParentForm;
                this.paletteForm.ColorChanged += this.PaletteEditorFormColorChanged;
                this.trackDisplay.ColorSelected += this.TileColorSelected;
                this.tilesetControl.ColorSelected += this.TileColorSelected;
                this.overlayControl.ColorSelected += this.TileColorSelected;
                Context.ColorPickerControl = this.paletteForm;
            }

            this.paletteForm.Init();
            this.paletteFormInitialized = true;
        }

        private void ReInitPaletteEditor()
        {
            if (!this.paletteFormInitialized)
            {
                return;
            }

            if (!this.paletteForm.Visible)
            {
                // Reinit the palette editor next time it's shown
                this.paletteFormInitialized = false;
            }
            else
            {
                // Reinit the palette editor now
                this.paletteForm.Init();
                this.paletteForm.Editor.Theme = this.track.Theme;
            }
        }

        private void PaletteEditorFormColorChanged(object sender, EventArgs e)
        {
            this.UpdateTiles();
        }

        private void TileColorSelected(object sender, EventArgs<Palette, int> e)
        {
            this.paletteForm.Editor.Theme = this.track.Theme;
            this.paletteForm.Editor.SetPalette(e.Value1);
            this.paletteForm.Editor.SetColorIndex(e.Value2);
        }

        /// <summary>
        /// If the user changed a color palette, the tiles need to be updated to reflect this.
        /// </summary>
        private void UpdateTiles()
        {
            Theme theme = this.paletteForm.Editor.Theme;
            Palette palette = this.paletteForm.Editor.Palette;
            bool isSpritePalette = theme.Palettes.IndexOf(palette) >= Palettes.SpritePaletteStart;

            if (this.track.Theme != theme)
            {
                if (!isSpritePalette)
                {
                    theme.UpdateTiles(palette);
                }
            }
            else
            {
                if (!isSpritePalette)
                {
                    // The updated color belongs to the theme of the current track,
                    // and is not a sprite color palette, so caches need to be updated

                    theme.UpdateTiles(palette);
                    this.trackDrawer.ReloadPalette(palette);
                    int xStart = this.tileClipboardTopLeft.X;
                    int yStart = this.tileClipboardTopLeft.Y;
                    this.trackDrawer.UpdateTileClipboard(xStart, yStart, this.tileClipboardSize);
                    this.tilesetControl.UpdateTileset();
                    this.overlayControl.UpdateTileset();
                }

                this.trackDisplay.Refresh();
            }
        }
        #endregion MenuBar

        #region TrackDisplay events
        private void TrackDisplayPaint(object sender, PaintEventArgs e)
        {
            if (!this.trackDisplay.Focused)
            {
                // Redraw the whole panel if the focus has been lost
                this.trackDrawer.NotifyFullRepaintNeed();
            }

            Graphics g = e.Graphics;
            switch (this.editionMode)
            {
                case EditionMode.Tileset:
                    this.trackDrawer.DrawTrackTileset(g, this.TilePosition, this.buttonsPressed, this.tileClipboardSize, this.tileClipboardTopLeft);
                    break;

                case EditionMode.Overlay:
                    this.trackDrawer.DrawTrackOverlay(g, this.hoveredOverlayTile, this.overlayControl.SelectedTile, this.overlayControl.SelectedPattern, this.selectedOverlayPatternLocation);
                    break;

                case EditionMode.Start:
                    this.trackDrawer.DrawTrackStart(g);
                    break;

                case EditionMode.Objects:
                    this.trackDrawer.DrawTrackObjects(g, this.hoveredObject, this.objectsControl.FrontZonesView);
                    break;

                case EditionMode.AI:
                    this.trackDrawer.DrawTrackAI(g, this.hoveredAIElem, this.aiControl.SelectedElement, this.aiAction == AIAction.DragTarget);
                    break;
            }
        }

        private void TrackDisplayGotFocus(object sender, EventArgs e)
        {
            this.trackDrawer.NotifyFullRepaintNeed();
        }

        private void TrackDisplayVScrollBarMouseMove(object sender, MouseEventArgs e)
        {
            if (Form.ActiveForm != null) // Application focused
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
            TrackEditor.SetScrollingValue(this.trackDisplayHScrollBar, x);
        }

        private void SetVerticalScrollingValue(int y)
        {
            TrackEditor.SetScrollingValue(this.trackDisplayVScrollBar, y);
        }

        private static void SetScrollingValue(ScrollBar scrollBar, int value)
        {
            if (value < scrollBar.Minimum || scrollBar.Minimum == scrollBar.Maximum)
            {
                scrollBar.Value = scrollBar.Minimum;
            }
            else if (value > scrollBar.Maximum - (scrollBar.LargeChange - 1))
            {
                // The inclusion of the LargeChange - 1 part in the calculation
                // is due to the fact it's not possible for users to reach the scroll bar maximum.
                // See: http://msdn.microsoft.com/en-us/library/system.windows.forms.scrollbar.maximum.aspx
                scrollBar.Value = Math.Max(scrollBar.Minimum,
                                           scrollBar.Maximum - (scrollBar.LargeChange - 1));
            }
            else
            {
                scrollBar.Value = value;
            }
        }

        private void RepaintAfterScrollingIfNeeded()
        {
            if (this.repaintAfterScrolling)
            {
                this.trackDrawer.NotifyFullRepaintNeed();
                this.trackDisplay.Invalidate();
                this.repaintAfterScrolling = false;
            }
        }

        private void TrackDisplaySizeChanged(object sender, EventArgs e)
        {
            this.trackDrawer.ResizeWindow();
            this.UpdateScrollBars();
        }

        private void TrackDisplayKeyDown(object sender, KeyEventArgs e)
        {
            if (this.buttonsPressed != MouseButtons.None)
            {
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                if (this.editionMode == EditionMode.Overlay)
                {
                    if (this.overlayControl.SelectedTile != null)
                    {
                        this.DeleteOverlayTile();
                    }
                }
                else if (this.editionMode == EditionMode.AI)
                {
                    if (this.aiControl.SelectedElement != null)
                    {
                        this.DeleteAIElement();
                    }
                }
            }
        }

        private void TrackDisplayMouseMove(object sender, MouseEventArgs e)
        {
            if (Form.ActiveForm != null) // Application focused
            {
                this.trackDisplay.Focus(); // Lets you use the mouse wheel to scroll
            }

            Point tilePositionBefore = this.TilePosition;
            this.SetPosition(e.Location);

            if (tilePositionBefore == this.TilePosition) // If the cursor has not moved to another tile
            {
                if (this.editionMode == EditionMode.Start &&
                    this.buttonsPressed != MouseButtons.Middle)
                {
                    // The only mode that needs pixel precision,
                    // as opposed to tile precision
                    if (this.InitStartAction())
                    {
                        this.trackDisplay.Invalidate();
                    }
                }
            }
            else
            {
                if (this.buttonsPressed == MouseButtons.Middle)
                {
                    this.ScrollTrack();
                }
                else
                {
                    this.InitEditionModeAction();
                }
            }
        }

        private void SetPosition(Point location)
        {
            int x = location.X;
            int y = location.Y;
            int zoomedTileSize = (int)(Tile.Size * this.Zoom);
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
                this.trackDisplay.Invalidate();
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

        private void TrackDisplayMouseLeave(object sender, EventArgs e)
        {
            this.trackDisplay.Cursor = Cursors.Default;

            if (this.buttonsPressed == MouseButtons.Right)
            {
                this.OnRightMouseButtonRelease();
            }

            // Cancel pressed mouse boutons (needed in case the panel lost focus unexpectedly)
            this.buttonsPressed = MouseButtons.None;

            this.RemoveFocus();

            this.ResetPosition();
            this.hoveredOverlayTile = null;
            this.selectedOverlayPatternLocation = new Point(-1, -1);
            this.hoveredObject = null;
            this.hoveredAIElem = null;

            this.trackDisplay.Invalidate();
        }

        private void TrackDisplayMouseDown(object sender, MouseEventArgs e)
        {
            // We only acknowledge the click if no mouse button is already pressed
            if (this.buttonsPressed != MouseButtons.None)
            {
                return;
            }

            if (e.Button == MouseButtons.Middle)
            {
                this.buttonsPressed = MouseButtons.Middle;
                this.trackDisplay.Cursor = Cursors.SizeAll;
                this.anchorPoint = this.AbsoluteTilePosition;
                this.trackDisplay.Invalidate();
            }
            else if (this.editionMode == EditionMode.Tileset)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        this.buttonsPressed = MouseButtons.Left;

                        this.undoRedoBuffer.BeginAdd();

                        if (this.LayTiles())
                        {
                            this.trackDisplay.Invalidate();
                        }
                        break;

                    case MouseButtons.Right:
                        byte? hoveredTile = this.GetHoveredTile();
                        if (hoveredTile != null)
                        {
                            this.buttonsPressed = MouseButtons.Right;

                            if (this.tileClipboard[0] != hoveredTile)
                            {
                                this.tilesetControl.SelectedTile = (byte)hoveredTile;
                            }

                            this.tileClipboard.Clear();
                            this.tileClipboard.Add((byte)hoveredTile);

                            this.anchorPoint = this.tileClipboardTopLeft = this.AbsoluteTilePosition;

                            this.tileClipboardSize.Width = this.tileClipboardSize.Height = 1;

                            this.trackDisplay.Invalidate();
                        }
                        break;
                }
            }
            else if (this.editionMode == EditionMode.Overlay)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (this.overlayControl.SelectedPattern == null)
                        {
                            this.overlayControl.SelectedTile = this.hoveredOverlayTile;

                            if (this.overlayControl.SelectedTile != null)
                            {
                                this.buttonsPressed = MouseButtons.Left;
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
                            this.InitOverlayAction();
                        }
                        else
                        {
                            this.overlayControl.SelectedPattern = this.hoveredOverlayTile.Pattern;
                            this.SetSelectedOverlayPatternLocation();
                            this.hoveredOverlayTile = null;
                            this.trackDisplay.Cursor = Cursors.Default;
                        }
                        break;
                }

                this.trackDisplay.Invalidate();
            }
            else if (this.editionMode == EditionMode.Start)
            {
                if (e.Button != MouseButtons.Left ||
                    this.startAction == StartAction.None)
                {
                    return;
                }

                this.buttonsPressed = MouseButtons.Left;
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
            else if (this.editionMode == EditionMode.Objects)
            {
                if (this.hoveredObject == null)
                {
                    return;
                }

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        this.buttonsPressed = MouseButtons.Left;
                        this.trackDisplay.Invalidate();
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
                        this.trackDisplay.Invalidate();
                        break;
                }
            }
            else if (this.editionMode == EditionMode.AI)
            {
                if (this.hoveredAIElem == null)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        this.aiControl.SelectedElement = null;
                        this.trackDisplay.Invalidate();
                    }
                    return;
                }

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        this.buttonsPressed = MouseButtons.Left;
                        this.aiControl.SelectedElement = this.hoveredAIElem;
                        this.trackDisplay.Invalidate();
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
                        this.trackDisplay.Invalidate();
                        break;
                }
            }
        }

        private void TrackDisplayMouseUp(object sender, MouseEventArgs e)
        {
            // When the user releases a mouse button, we ensure this button is the same
            // as the one that was initially held down before validating the action.
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    if (this.buttonsPressed != MouseButtons.Middle)
                    {
                        break;
                    }

                    this.buttonsPressed = MouseButtons.None;

                    if (this.editionMode == EditionMode.Tileset ||
                        this.track is BattleTrack && this.editionMode == EditionMode.Objects)
                    {
                        // For other modes, the cursor will be reset
                        // by the call to the InitEditionModeAction method below.
                        this.trackDisplay.Cursor = Cursors.Default;
                    }

                    this.InitEditionModeAction();
                    break;

                case MouseButtons.Left:
                    if (this.buttonsPressed != MouseButtons.Left)
                    {
                        break;
                    }

                    if (this.editionMode == EditionMode.Tileset)
                    {
                        this.undoRedoBuffer.EndAdd();
                        this.menuBar.UndoEnabled = true;
                        this.menuBar.RedoEnabled = false;
                    }

                    this.buttonsPressed = MouseButtons.None;
                    break;

                case MouseButtons.Right:
                    if (this.buttonsPressed != MouseButtons.Right)
                    {
                        break;
                    }

                    this.buttonsPressed = MouseButtons.None;

                    if (this.editionMode == EditionMode.Tileset)
                    {
                        this.OnRightMouseButtonRelease();
                    }
                    break;
            }
        }

        private void TrackDisplayMouseWheel(object sender, MouseEventArgs e)
        {
            if (!this.trackDisplay.Focused)
            {
                return;
            }

            if (Control.ModifierKeys == Keys.Control)
            {
                this.MouseWheelZoom(e);
            }
            else if (this.buttonsPressed != MouseButtons.Middle)
            {
                this.MouseWheelScroll(e);
            }
        }

        private void MouseWheelScroll(MouseEventArgs e)
        {
            int delta = e.Delta / 64;
            if (delta == 0)
            {
                // Ensure some scrolling happens even if the Delta value was low
                // (Makes tablet and stylus scrolling work more nicely,
                // as well as other special settings or hardware)
                delta = e.Delta > 0 ? 1 : -1;
            }

            int before;
            int after;
            if (Control.ModifierKeys != Keys.Shift)
            {
                int y = this.scrollPosition.Y - delta;
                before = this.scrollPosition.Y;
                this.SetVerticalScrollingValue(y);
                after = this.scrollPosition.Y;
            }
            else
            {
                int x = this.scrollPosition.X - delta;
                before = this.scrollPosition.X;
                this.SetHorizontalScrollingValue(x);
                after = this.scrollPosition.X;
            }

            if (before != after)
            {
                this.trackDrawer.NotifyFullRepaintNeed();
                this.InitEditionModeAction(true);
            }
        }

        private void TrackDisplayMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.editionMode == EditionMode.AI &&
                e.Button == MouseButtons.Left &&
                this.hoveredAIElem == null)
            {
                this.AddAIElement(this.AbsoluteTilePosition);
            }
        }
        #endregion TrackDisplay events

        #region TrackDisplay methods
        private void DisplayNewTrack()
        {
            this.trackDrawer.LoadTrack(this.track);
            this.trackDisplay.Invalidate();
        }

        private void ResetPosition()
        {
            this.pixelPosition = new Point(-1, -1);
            this.menuBar.ResetCoordinates();
        }

        /// <summary>
        /// Updates the horizontal and vertical scroll bar properties
        /// depending on the track display panel size and zoom level.
        /// </summary>
        private void UpdateScrollBars()
        {
            int offScreenTileCountX;
            int offScreenTileCountY;
            this.GetOffScreenTileCounts(out offScreenTileCountX, out offScreenTileCountY);

            // Recalculate the maximum value of the horizontal scroll bar
            this.UpdateScrollBar(this.trackDisplayHScrollBar, offScreenTileCountX);

            // Recalculate the maximum value of the vertical scroll bar
            this.UpdateScrollBar(this.trackDisplayVScrollBar, offScreenTileCountY);
        }

        /// <summary>
        /// Gets the number of track tiles that are off screen (not visible), horizontally and vertically.
        /// </summary>
        /// <param name="offScreenTileCountX">The number of horizontally off-screen tiles (from 0 to 128).</param>
        /// <param name="offScreenTileCountY">The number of vertically off-screen tiles (from 0 to 128).</param>
        private void GetOffScreenTileCounts(out int offScreenTileCountX, out int offScreenTileCountY)
        {
            offScreenTileCountX = this.GetOffScreenTileCount(this.trackDisplay.Width);
            offScreenTileCountY = this.GetOffScreenTileCount(this.trackDisplay.Height);
            int offScreenTileCountXWithScrollBar = this.GetOffScreenTileCount(Math.Max(0, this.trackDisplay.Width - this.trackDisplayVScrollBar.Width));
            int offScreenTileCountYWithScrollBar = this.GetOffScreenTileCount(Math.Max(0, this.trackDisplay.Height - this.trackDisplayHScrollBar.Height));

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

        /// <summary>
        /// Gets the number of track tiles that are off screen (not visible).
        /// </summary>
        /// <param name="panelSize">The width or height of the track display panel.</param>
        /// <returns>The number of off-screen tiles (from 0 to 128).</returns>
        private int GetOffScreenTileCount(int panelSize)
        {
            return this.track.Map.Width - this.GetOnScreenTileCount(panelSize); // Map.Width = Map.Height
        }

        /// <summary>
        /// Gets the number of track tiles that are on screen (visible).
        /// </summary>
        /// <param name="panelSize">The width or height of the track display panel.</param>
        /// <returns>The number of on-screen tiles (from 0 to 128).</returns>
        private int GetOnScreenTileCount(int panelSize)
        {
            return Math.Min(this.track.Map.Width, (int)((panelSize) / (Tile.Size * this.Zoom)));
        }

        /// <summary>
        /// Updates the properties of the passed scroll bar.
        /// </summary>
        private void UpdateScrollBar(ScrollBar scrollBar, int offScreenTileCount)
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

                // Reposition the track content properly (avoiding to show off-track black)
                // when resizing the window and the panel is scrolled to the bottom and/or right limit.
                if (scrollBar.Value > offScreenTileCount)
                {
                    scrollBar.Value = offScreenTileCount;
                    this.trackDisplay.Invalidate();
                }

                int onScreenTileCount = this.track.Map.Width - offScreenTileCount; // Map.Width = Map.Height
                scrollBar.Maximum = offScreenTileCount + (onScreenTileCount - 1);
                scrollBar.LargeChange = onScreenTileCount;
                // Adding the equivalent of LargeChange - 1 to the Maximum because
                // it's not possible for users to reach the scroll bar maximum.
                // "The maximum value that can be reached through user interaction is equal to
                // 1 plus the Maximum property value minus the LargeChange property value."
                // See: http://msdn.microsoft.com/en-us/library/system.windows.forms.scrollbar.maximum.aspx
            }
        }
        #endregion TrackDisplay methods

        #region TrackTreeView
        public void RemoveModifiedHints()
        {
            this.trackTreeView.RemoveModifiedHints();
        }

        private void TrackTreeViewSelectedTrackChanged(object sender, EventArgs e)
        {
            this.ResetScrollingPosition();
            this.SetTrack();
            this.EnableDisableUndoRedo();
            this.DisplayNewTrack();
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

        private void SetTrack()
        {
            this.track = this.trackTreeView.SelectedTrack;

            this.tilesetControl.Track = this.track;
            this.hoveredOverlayTile = null;
            this.overlayControl.SelectedTile = null;
            this.UpdateOverlayTileCount();
            this.startControl.Track = this.track;
            this.aiControl.TrackAI = this.track.AI;
            this.hoveredAIElem = null;

            if (this.editionMode == EditionMode.Objects)
            {
                this.SetTrackObjectZones();
            }
        }
        #endregion TrackTreeView

        #region EditionMode tabs
        /// <summary>
        /// Init an action depending on the current edition mode.
        /// </summary>
        private void InitEditionModeAction()
        {
            this.InitEditionModeAction(false);
        }

        /// <summary>
        /// Init an action depending on the current edition mode.
        /// </summary>
        /// <param name="forceRepaint">If true: trigger a repaint after the action, whether something has changed or not.
        /// If false: only repaint if something has visually changed on the track.</param>
        private void InitEditionModeAction(bool forceRepaint)
        {
            if (Context.ColorPickerMode)
            {
                if (forceRepaint)
                {
                    this.trackDisplay.Refresh();
                }
                this.menuBar.UpdateCoordinates(this.AbsoluteTilePosition);
                return;
            }

            bool repaintNeeded;

            switch (this.editionMode)
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
                this.trackDisplay.Refresh();
            }

            this.menuBar.UpdateCoordinates(this.AbsoluteTilePosition);
        }

        private void SetEditionMode()
        {
            if (this.tilesetTabPage.Visible)
            {
                this.editionMode = EditionMode.Tileset;
            }
            else if (this.overlayTabPage.Visible)
            {
                this.editionMode = EditionMode.Overlay;
            }
            else if (this.startTabPage.Visible)
            {
                this.editionMode = EditionMode.Start;
            }
            else if (this.objectsTabPage.Visible)
            {
                this.editionMode = EditionMode.Objects;
                this.SetTrackObjectZones();
            }
            else // if (this.aiTabPage.Visible)
            {
                this.editionMode = EditionMode.AI;
            }

            this.EnableDisableUndoRedo();
        }

        private void ModeTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetEditionMode();
            this.ResizeModeTabControl();
            this.trackDisplay.Invalidate();
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
        #endregion EditionMode tabs


        #region EditionMode.Tileset
        private bool InitTilesetAction()
        {
            bool repaintNeeded;

            switch (this.buttonsPressed)
            {
                case MouseButtons.Left:
                    repaintNeeded = this.LayTiles();
                    break;

                case MouseButtons.Right:
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

                this.AddUndoChange(hoveredTilePosition, affectedSurface);

                this.track.Map.SetTiles(hoveredTilePosition, affectedSurface, this.tileClipboardSize, this.tileClipboard);

                this.trackDrawer.UpdateCacheAfterTileLaying(hoveredTilePosition);

                this.trackTreeView.MarkTrackAsChanged();

                return true;
            }

            return false;
        }

        private void AddUndoChange(Point location, Size size)
        {
            TileChange tileChange = new TileChange(location, size, this.track);
            this.undoRedoBuffer.Add(tileChange);
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

            // We remove the first tile, which was added in TrackDisplayMouseDown
            this.tileClipboard.RemoveAt(0);

            this.trackDisplay.Invalidate();
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

            this.undoRedoBuffer.Clear();
            this.menuBar.UndoEnabled = false;
            this.menuBar.RedoEnabled = false;

            this.trackTreeView.MarkTrackAsChanged();
            this.trackDisplay.Invalidate();
        }
        #endregion EditionMode.Tileset

        #region EditionMode.Overlay
        private bool InitOverlayAction()
        {
            Point hoveredTilePosition = this.AbsoluteTilePosition;

            if (this.buttonsPressed == MouseButtons.Left)
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
                        this.trackDisplay.Cursor = Cursors.Hand;

                        if (this.hoveredOverlayTile == overlayTile)
                        {
                            return false;
                        }

                        this.hoveredOverlayTile = overlayTile;
                        return true;
                    }
                }

                this.trackDisplay.Cursor = Cursors.Default;

                if (this.hoveredOverlayTile == null)
                {
                    return false;
                }

                this.hoveredOverlayTile = null;
                return true;
            }

            // Move selected tile pattern
            this.trackDisplay.Cursor = Cursors.Default;
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
            this.trackDisplay.Invalidate();
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
            this.trackDisplay.Invalidate();
        }

        private void OverlayControlRepaintRequested(object sender, EventArgs e)
        {
            this.trackDisplay.Invalidate();
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
            if (Context.ColorPickerMode)
            {
                return false;
            }

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

            if (this.buttonsPressed == MouseButtons.Left)
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
                    this.trackDisplay.Cursor = Cursors.SizeAll;
                }
                else
                {
                    this.startAction = StartAction.ResizeLapLine;
                    this.trackDisplay.Cursor = Cursors.SizeWE;
                }
            }
            else if (gpTrack.StartPosition.IntersectsWith(absPixelPos))
            {
                this.startAction = this.startControl.LapLineAndDriverPositionsBound ?
                    StartAction.DragAll : StartAction.DragStartPosition;
                this.trackDisplay.Cursor = Cursors.SizeAll;
            }
            else
            {
                this.startAction = StartAction.None;
                this.trackDisplay.Cursor = Cursors.Default;
            }

            return false;
        }

        private bool InitBattleStartAction()
        {
            BattleTrack bTrack = this.track as BattleTrack;

            if (this.buttonsPressed == MouseButtons.Left)
            {
                BattleStartPosition position = this.startAction == StartAction.DragStartPosition ?
                    bTrack.StartPositionP1 : bTrack.StartPositionP2;

                return this.InitBattleStartActionSub(position);
            }

            Point absPixelPos = this.AbsolutePixelPosition;

            if (bTrack.StartPositionP1.IntersectsWith(absPixelPos))
            {
                this.startAction = StartAction.DragStartPosition;
                this.trackDisplay.Cursor = Cursors.Hand;
            }
            else if (bTrack.StartPositionP2.IntersectsWith(absPixelPos))
            {
                this.startAction = StartAction.DragStartPosition2;
                this.trackDisplay.Cursor = Cursors.Hand;
            }
            else
            {
                this.startAction = StartAction.None;
                this.trackDisplay.Cursor = Cursors.Default;
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
            this.trackDisplay.Invalidate();
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

            if (this.buttonsPressed == MouseButtons.Left)
            {
                // Drag object
                this.hoveredObject.Location = hoveredTilePosition;
                this.trackTreeView.MarkTrackAsChanged();
                return true;
            }

            // Try to hover object
            GPTrack gpTrack = this.track as GPTrack;

            if (gpTrack.ObjectRoutine == ObjectType.Pillar)
            {
                // Not supported yet
                this.trackDisplay.Cursor = Cursors.Default;
                return false;
            }

            foreach (TrackObject trackObject in gpTrack.Objects)
            {
                if (trackObject.X == hoveredTilePosition.X &&
                    trackObject.Y == hoveredTilePosition.Y)
                {
                    this.hoveredObject = trackObject;
                    this.trackDisplay.Cursor = Cursors.Hand;
                    return true;
                }
            }

            this.trackDisplay.Cursor = Cursors.Default;

            if (this.hoveredObject != null)
            {
                this.hoveredObject = null;
                return true;
            }

            return false;
        }

        private void SetTrackObjectZones()
        {
            if (this.track is GPTrack)
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

        private void ObjectsControlViewChanged(object sender, EventArgs e)
        {
            this.trackDisplay.Invalidate();
        }

        private void ObjectsControlDataChanged(object sender, EventArgs e)
        {
            this.trackTreeView.MarkTrackAsChanged();
            this.trackDisplay.Invalidate();
        }

        private void ObjectsControlDataChangedNoRepaint(object sender, EventArgs e)
        {
            this.trackTreeView.MarkTrackAsChanged();
        }
        #endregion EditionMode.Objects

        #region EditionMode.AI
        private bool InitAIAction()
        {
            Point hoveredTilePosition = this.AbsoluteTilePosition;

            if (this.buttonsPressed == MouseButtons.Left)
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
                    this.trackDisplay.Cursor = Cursors.Hand;
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

            this.trackDisplay.Cursor = Cursors.Default;

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
                    this.trackDisplay.Cursor = Cursors.SizeAll;
                    this.SetAIElementAnchorPoint(hoveredTilePosition);
                }
                else
                {
                    this.resizeHandle = this.aiControl.SelectedElement.GetResizeHandle(hoveredTilePosition);

                    if (this.resizeHandle == ResizeHandle.None)
                    {
                        this.aiAction = AIAction.DragZone;
                        this.trackDisplay.Cursor = Cursors.SizeAll;
                        this.SetAIElementAnchorPoint(hoveredTilePosition);
                    }
                    else
                    {
                        this.aiAction = AIAction.ResizeZone;

                        switch (this.resizeHandle)
                        {
                            case ResizeHandle.TopLeft:
                                this.trackDisplay.Cursor = Cursors.SizeNWSE;
                                break;

                            case ResizeHandle.Top:
                                this.trackDisplay.Cursor = Cursors.SizeNS;
                                break;

                            case ResizeHandle.TopRight:
                                this.trackDisplay.Cursor = Cursors.SizeNESW;
                                break;

                            case ResizeHandle.Right:
                                this.trackDisplay.Cursor = Cursors.SizeWE;
                                break;

                            case ResizeHandle.BottomRight:
                                this.trackDisplay.Cursor = Cursors.SizeNWSE;
                                break;

                            case ResizeHandle.Bottom:
                                this.trackDisplay.Cursor = Cursors.SizeNS;
                                break;

                            case ResizeHandle.BottomLeft:
                                this.trackDisplay.Cursor = Cursors.SizeNESW;
                                break;

                            case ResizeHandle.Left:
                                this.trackDisplay.Cursor = Cursors.SizeWE;
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

        private void AddAIElement(Point location)
        {
            TrackAIElement newAIElem = new TrackAIElement(location);

            if (this.track.AI.Add(newAIElem))
            {
                this.aiControl.SetMaximumAIElementIndex();
                this.aiControl.SelectedElement = newAIElem;
                this.InitAIAction();

                this.trackTreeView.MarkTrackAsChanged();
                this.trackDisplay.Invalidate();

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
            this.trackDisplay.Invalidate();

            if (this.track.AI.ElementCount == 0)
            {
                this.aiControl.ShowWarning();
            }
        }

        private void AIControlDataChanged(object sender, EventArgs e)
        {
            this.trackTreeView.MarkTrackAsChanged();
            this.trackDisplay.Invalidate();
        }

        private void AIControlDeleteRequested(object sender, EventArgs e)
        {
            this.DeleteAIElement();
        }

        private void AIControlAddRequested(object sender, EventArgs e)
        {
            this.AddAIElement(this.AbsoluteCenterTileLocation);
        }

        private void AIControlDeleteAllRequested(object sender, EventArgs e)
        {
            this.aiControl.SelectedElement = null;
            this.track.AI.Clear();
            this.trackTreeView.MarkTrackAsChanged();
            this.trackDisplay.Invalidate();
            this.aiControl.ShowWarning();
        }
        #endregion EditionMode.AI

        #region class TrackPanel
        private class TrackPanel : TilePanel
        {
            protected override Tile GetTileAt(int x, int y)
            {
                // Convert from pixel precision to tile precision
                x /= Tile.Size;
                y /= Tile.Size;

                var parent = this.Parent as TrackEditor;

                Point scrollPosition = parent.scrollPosition;
                x += scrollPosition.X;
                y += scrollPosition.Y;

                if (x > TrackMap.Limit || y > TrackMap.Limit)
                {
                    return null;
                }

                Track track = parent.track;
                byte index = track.Map[x, y];

                if (parent.editionMode == EditionMode.Overlay)
                {
                    Point location = new Point(x, y);

                    var overlay = track.OverlayTiles;
                    for (int i = overlay.Count - 1; i >= 0; i--)
                    {
                        var overlayTile = overlay[i];
                        if (overlayTile.IntersectsWith(location))
                        {
                            int relativeX = x - overlayTile.X;
                            int relativeY = y - overlayTile.Y;
                            byte tileId = overlayTile.Pattern[relativeX, relativeY];

                            if (tileId != 0xFF) // Ignore empty tiles
                            {
                                index = tileId;
                                break;
                            }
                        }
                    }
                }

                return track.GetRoadTile(index);
            }
        }
        #endregion class TrackPanel
    }
}