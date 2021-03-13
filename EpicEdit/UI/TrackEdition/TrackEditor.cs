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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Tracks.Start;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.SettingEdition;
using EpicEdit.UI.ThemeEdition;
using EpicEdit.UI.Tools;
using EpicEdit.UI.Tools.UndoRedo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DrawRegion = System.Drawing.Region;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// A track editor.
    /// </summary>
    internal partial class TrackEditor : UserControl
    {
        #region Private members
        /// <summary>
        /// If a Point is equal to OutOfBounds, then it's not within the track map.
        /// </summary>
        public static readonly Point OutOfBounds = new Point(int.MinValue, int.MinValue);

        /// <summary>
        /// Used to draw the track.
        /// </summary>
        private TrackDrawer _drawer;

        /// <summary>
        /// The track currently displayed.
        /// </summary>
        private Track _track;

        /// <summary>
        /// The current edition mode.
        /// </summary>
        private EditionMode _editionMode = EditionMode.Tileset;

        /// <summary>
        /// All the available zoom levels.
        /// </summary>
        private readonly float[] _zoomLevels;

        /// <summary>
        /// The index to the current zoom level.
        /// </summary>
        private int _zoomLevelIndex;

        /// <summary>
        /// Gets or sets the index to the current zoom level.
        /// </summary>
        private int ZoomLevelIndex
        {
            get => _zoomLevelIndex;
            set
            {
                _zoomLevelIndex = value;
                trackDisplay.Zoom = Zoom;
            }
        }

        /// <summary>
        /// The index to the default zoom level (x1).
        /// </summary>
        private const int DefaultZoomLevelIndex = 2;

        /// <summary>
        /// The current zoom level of the track display.
        /// </summary>
        private float Zoom => _zoomLevels[ZoomLevelIndex];

        /// <summary>
        /// Region which needs to be invalidated.
        /// </summary>
        private DrawRegion _dirtyRegion = new DrawRegion(Rectangle.Empty);

        /// <summary>
        /// Flag to determine whether to repaint the track display
        /// when the scrolling position has changed.
        /// </summary>
        private bool _repaintAfterScrolling;

        // Which pixel the cursor is on (doesn't take scrolling position in consideration).
        private Point _pixelPosition;

        // Which pixel the cursor is on (takes scrolling position in consideration).
        private Point AbsolutePixelPosition
        {
            get
            {
                return new Point(_scrollPosition.X * Tile.Size + (int)(_pixelPosition.X / Zoom),
                                 _scrollPosition.Y * Tile.Size + (int)(_pixelPosition.Y / Zoom));
            }
        }

        /// <summary>
        /// Which tile the cursor is on (doesn't take scrolling position in consideration).
        /// </summary>
        private Point TilePosition
        {
            get
            {
                if (_pixelPosition == OutOfBounds)
                {
                    // The mouse cursor isn't over the track
                    return _pixelPosition;
                }

                return new Point((int)(_pixelPosition.X / (Tile.Size * Zoom)),
                                 (int)(_pixelPosition.Y / (Tile.Size * Zoom)));
            }
        }

        /// <summary>
        /// Which tile the cursor is on (takes scrolling position in consideration).
        /// </summary>
        private Point AbsoluteTilePosition
        {
            get
            {
                return new Point(_scrollPosition.X + TilePosition.X,
                                 _scrollPosition.Y + TilePosition.Y);
            }
        }

        /// <summary>
        /// Scrolling position in the track (= location of the top-left displayed tile).
        /// </summary>
        private Point _scrollPosition;

        /// <summary>
        /// The position of the tile displayed in the center of the track panel
        /// (doesn't take scrolling position in consideration).
        /// </summary>
        private Point CenterTileLocation
        {
            get
            {
                return new Point(GetOnScreenTileCount(trackDisplay.Width) / 2,
                                 GetOnScreenTileCount(trackDisplay.Height) / 2);
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
                Point point = CenterTileLocation;
                point.Offset(_scrollPosition);
                return point;
            }
        }

        /// <summary>
        /// A collection of tiles copied by the user.
        /// </summary>
        private TileClipboard _tileClipboard;

        /// <summary>
        /// Used to store the anchor point for various operations (position where tile clipboard was started, dragging map or AI elements...).
        /// </summary>
        private Point _anchorPoint;

        private Rectangle TileSelectionRectangle
        {
            get
            {
                if (_buttonsPressed == MouseButtons.Middle ||
                    TilePosition == OutOfBounds ||
                    Context.ColorPickerMode)
                {
                    return Rectangle.Empty;
                }

                Point position;
                if (_buttonsPressed != MouseButtons.Right) // The user is simply hovering tiles
                {
                    position = new Point(TilePosition.X + _scrollPosition.X,
                                         TilePosition.Y + _scrollPosition.Y);
                }
                else // A tile selection is happening now
                {
                    position = _tileClipboard.Location;
                }

                return new Rectangle(position, _tileClipboard.Size);
            }
        }

        /// <summary>
        /// Defines which mouse buttons are currently pressed, if any.
        /// </summary>
        private MouseButtons _buttonsPressed;

        /// <summary>
        /// Determines from which side the current element is being resized.
        /// The element can be the track lap line, or an AI area.
        /// </summary>
        private ResizeHandle _resizeHandle;

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
            DragArea,
            ResizeArea
        }

        /// <summary>
        /// The hovered track overlay tile.
        /// </summary>
        private OverlayTile _hoveredOverlayTile;

        /// <summary>
        /// The location of the selected overlay tile pattern.
        /// </summary>
        private Point _selectedOverlayPatternLocation;

        /// <summary>
        /// The current action the user is doing (or about to do) on the start data.
        /// </summary>
        private StartAction _startAction;

        /// <summary>
        /// The hovered track object.
        /// </summary>
        private TrackObject _hoveredObject;

        /// <summary>
        /// The hovered AI element.
        /// </summary>
        private TrackAIElement _hoveredAIElem;

        /// <summary>
        /// The current action the user is doing (or about to do) on the AI.
        /// </summary>
        private AIAction _aiAction;

        /// <summary>
        /// Undo/redo buffers for tile changes, for each track.
        /// </summary>
        private Dictionary<Track, UndoRedoBuffer> _undoRedoBuffers;

        /// <summary>
        /// Undo/redo buffer for tile changes, for the current track.
        /// </summary>
        private UndoRedoBuffer UndoRedoBuffer => _undoRedoBuffers[_track];

        /// <summary>
        /// Determines whether the palette editor form has been initialized.
        /// </summary>
        private bool _paletteFormInitialized;

        /// <summary>
        /// The color palette editor form.
        /// </summary>
        private PaletteEditorForm _paletteForm;

        /// <summary>
        /// Determines whether the background editor form has been initialized.
        /// </summary>
        private bool _backgroundFormInitialized;

        /// <summary>
        /// The background editor form.
        /// </summary>
        private BackgroundEditorForm _backgroundForm;

        /// <summary>
        /// Determines whether the setting editor form has been initialized.
        /// </summary>
        private bool _settingFormInitialized;

        /// <summary>
        /// The setting editor form.
        /// </summary>
        private SettingEditorForm _settingForm;

        /// <summary>
        /// Determines whether the codec form has been initialized.
        /// </summary>
        private bool _codecFormInitialized;

        /// <summary>
        /// The codec form.
        /// </summary>
        private CodecForm _codecForm;
        #endregion Private members

        #region Events
        [Browsable(true), Category("Drag Drop")]
        public event EventHandler<EventArgs<string>> FileDragged;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> OpenRomDialogRequested
        {
            add => menuBar.OpenRomDialogRequested += value;
            remove => menuBar.OpenRomDialogRequested -= value;
        }

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> SaveRomDialogRequested
        {
            add => menuBar.SaveRomDialogRequested += value;
            remove => menuBar.SaveRomDialogRequested -= value;
        }

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> ToggleScreenModeRequested
        {
            add => menuBar.ToggleScreenModeRequested += value;
            remove => menuBar.ToggleScreenModeRequested -= value;
        }
        #endregion Events

        #region Constructor
        public TrackEditor()
        {
            InitializeComponent();

            ResetPosition();

            _zoomLevels = new float[] { .5f, .75f, 1, 2, 3, 4, 5, 6, 7, 8 };
            ZoomLevelIndex = DefaultZoomLevelIndex;

            _tileClipboard = new TileClipboard(tilesetControl.SelectedTile);

            _undoRedoBuffers = new Dictionary<Track, UndoRedoBuffer>();

            if (VisualStyleRenderer.IsSupported)
            {
                // Force background color to fix the look of TrackBar controls
                foreach (TabPage page in modeTabControl.TabPages)
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
            _drawer = new TrackDrawer(_tileClipboard);
            _drawer.GraphicsChanged += drawer_GraphicsChanged;

            trackTreeView.InitOnFirstRomLoad();
            tilesetControl.InitOnFirstRomLoad();
            overlayControl.InitOnFirstRomLoad();

            SetTrack();
            InitUndoRedo();

            // Adding these event handlers here rather than in the Designer.cs
            // saves us a null check on this.drawer in each of the corresponding functions,
            // because the drawer hasn't been initialized yet before a ROM is loaded.
            trackDisplay.Paint += TrackDisplayPaint;
            trackDisplay.SizeChanged += TrackDisplaySizeChanged;

            UpdateScrollBars();

            trackDisplay.Enabled = true;
            modeTabControl.Enabled = true;
            menuBar.EnableControls();
        }

        private void drawer_GraphicsChanged(object sender, EventArgs e)
        {
            InvalidateWholeTrackDisplay();
            trackDisplay.Update();
        }

        public void InitOnRomLoad()
        {
            tilesetControl.InitOnRomLoad();
            overlayControl.InitOnRomLoad();
            trackTreeView.InitOnRomLoad();
            ReInitPaletteEditor();
            ReInitBackgroundEditor();
            ReInitSettingEditor();
            ReInitCodecForm();

            foreach (UndoRedoBuffer buffer in _undoRedoBuffers.Values)
            {
                buffer.Clear();
            }
            _undoRedoBuffers.Clear();
            InitUndoRedo();
        }

        private void TrackEditorDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                DragDropEffects.Move :
                DragDropEffects.None;
        }

        private void TrackEditorDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                EventArgs<string> sea = new EventArgs<string>(files[0]);
                FileDragged(this, sea);
            }
        }

        private void MenuBarTrackImportDialogRequested(object sender, EventArgs e)
        {
            UITools.ShowImportDataDialog(fileName => ImportTrack(fileName), FileDialogFilters.Track);
        }

        private void MenuBarTrackImportAllDialogRequested(object sender, EventArgs e)
        {
            try
            {
                UITools.ShowImportDataDialog((index, fileName) =>
                    Context.Game.TrackGroups.GetTrack(index).Import(fileName, Context.Game),
                    FileDialogFilters.Track, Track.Count);
            }
            finally
            {
                RefreshCurrentTrack();
            }
        }

        public void ImportTrack(string filePath)
        {
            _track.Import(filePath, Context.Game);
            RefreshCurrentTrack();
        }

        public void RefreshCurrentTrack()
        {
            UndoRedoBuffer.Clear();
            menuBar.UndoEnabled = false;
            menuBar.RedoEnabled = false;

            _hoveredOverlayTile = null;
            overlayControl.SelectedTile = null;
            _hoveredAIElem = null;
            aiControl.SelectedElement = null;

            InvalidateTrack();
        }

        private void MenuBarTrackExportDialogRequested(object sender, EventArgs e)
        {
            UITools.ShowExportDataDialog(fileName => _track.Export(fileName, Context.Game), trackTreeView.SelectedTrackFileName, FileDialogFilters.Track);
        }

        private void MenuBarTrackExportAllDialogRequested(object sender, EventArgs e)
        {
            const string trackNumberPattern = "TRACKNO";
            const string trackNamePattern = "TRACKNAME";
            const string fileNamePattern = trackNumberPattern + "- " + trackNamePattern;

            UITools.ShowExportDataDialog(fileName =>
            {
                int trackId = 1;
                foreach (TrackGroup trackGroup in Context.Game.TrackGroups)
                {
                    foreach (Track t in trackGroup)
                    {
                        t.Export(fileName
                            .Replace(trackNumberPattern, trackId.ToString())
                            .Replace(trackNamePattern, UITools.SanitizeFileName(t.Name)),
                            Context.Game);
                        trackId++;
                    }
                }
            }, fileNamePattern, FileDialogFilters.Track);
        }

        private void MenuBarUndoRequested(object sender, EventArgs e)
        {
            ApplyUndoRedo(UndoRedoBuffer.Undo());
        }

        private void MenuBarRedoRequested(object sender, EventArgs e)
        {
            ApplyUndoRedo(UndoRedoBuffer.Redo());
        }

        private void ToggleUndoRedo()
        {
            menuBar.UndoEnabled = UndoRedoBuffer.HasUndo;
            menuBar.RedoEnabled = UndoRedoBuffer.HasRedo;
        }

        private void SetUndoRedo()
        {
            if (!_undoRedoBuffers.ContainsKey(_track))
            {
                InitUndoRedo();
            }
            else
            {
                if (_editionMode == EditionMode.Tileset)
                {
                    ToggleUndoRedo();
                }
                else
                {
                    menuBar.UndoEnabled = false;
                    menuBar.RedoEnabled = false;
                }
            }
        }

        /// <summary>
        /// Inits undo/redo buffer for the current track.
        /// </summary>
        private void InitUndoRedo()
        {
            _undoRedoBuffers.Add(_track, new UndoRedoBuffer(_track.Map));
            menuBar.UndoEnabled = false;
            menuBar.RedoEnabled = false;
        }

        private void ApplyUndoRedo(TileChange change)
        {
            if (change == null)
            {
                return;
            }

            ToggleUndoRedo();
            DrawRegion region = _drawer.UpdateCache(change.Rectangle);
            InvalidateTrackDisplay(region);
        }

        private void ResetZoom()
        {
            if (ZoomLevelIndex == DefaultZoomLevelIndex)
            {
                return;
            }

            Point location = AbsoluteCenterTileLocation;
            ZoomCommon(DefaultZoomLevelIndex);
            CenterTrackDisplayOn(location);

            InvalidateWholeTrackDisplay();
        }

        private void ZoomIn()
        {
            if (!CanZoomIn())
            {
                return;
            }

            Point location = AbsoluteCenterTileLocation;
            ZoomInSub();
            EndZoom(location);
        }

        private void ZoomOut()
        {
            if (!CanZoomOut())
            {
                return;
            }

            Point location = AbsoluteCenterTileLocation;
            ZoomOutSub();
            EndZoom(location);
        }

        private void EndZoom(Point location)
        {
            CenterTrackDisplayOn(location);

            if (_pixelPosition == OutOfBounds)
            {
                // The cursor isn't over the track
                InvalidateWholeTrackDisplay();
            }
            else
            {
                InitEditionModeAction(true);
            }
        }

        private void MouseWheelZoom(MouseEventArgs e)
        {
            Point hoveredTilePosition;

            if (e.Delta > 0)
            {
                if (!CanZoomIn())
                {
                    return;
                }
                hoveredTilePosition = AbsoluteTilePosition;
                ZoomInSub();
            }
            else
            {
                if (!CanZoomOut())
                {
                    return;
                }
                hoveredTilePosition = AbsoluteTilePosition;
                ZoomOutSub();
            }

            // Ensure the user will still be hovering the same tile after zooming
            Point cursor = TilePosition;
            Point center = CenterTileLocation;
            Point diff = new Point(center.X - cursor.X, center.Y - cursor.Y);
            hoveredTilePosition.Offset(diff);

            EndZoom(hoveredTilePosition);
        }

        private void CenterTrackDisplayOn(Point location)
        {
            Point point = CenterTileLocation;
            int x = location.X - point.X;
            int y = location.Y - point.Y;
            SetHorizontalScrollingValue(x);
            SetVerticalScrollingValue(y);
        }

        private void ZoomInSub()
        {
            ZoomCommon(ZoomLevelIndex + 1);
        }

        private void ZoomOutSub()
        {
            ZoomCommon(ZoomLevelIndex - 1);
        }

        private void ZoomCommon(int zoomLevelIndex)
        {
            ZoomLevelIndex = zoomLevelIndex;
            _drawer.Zoom = Zoom;
            UpdateScrollBars();

            menuBar.ZoomInEnabled = CanZoomIn();
            menuBar.ZoomOutEnabled = CanZoomOut();
        }

        private bool CanZoomIn()
        {
            return ZoomLevelIndex < _zoomLevels.Length - 1;
        }

        private bool CanZoomOut()
        {
            return ZoomLevelIndex > 0;
        }

        private void MenuBarZoomInRequested(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void MenuBarZoomOutRequested(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void MenuBarZoomResetRequested(object sender, EventArgs e)
        {
            ResetZoom();
        }

        private void RemoveFocus()
        {
            if (Form.ActiveForm != null) // Application focused
            {
                // Steal the focus from the panel to disable mouse-wheel scrolling
                menuBar.Focus();
            }
        }

        private void MenuBarPaletteEditorRequested(object sender, EventArgs e)
        {
            if (!_paletteFormInitialized)
            {
                InitPaletteEditorForm();
            }

            if (_paletteForm.Visible)
            {
                _paletteForm.Visible = false;
            }
            else
            {
                _paletteForm.Editor.Theme = _track.Theme;
                _paletteForm.Visible = true;
            }
        }

        private void InitPaletteEditorForm()
        {
            if (_paletteForm == null)
            {
                _paletteForm = new PaletteEditorForm();
                _paletteForm.Owner = ParentForm;
                trackDisplay.ColorSelected += TileColorSelected;
                tilesetControl.ColorSelected += TileColorSelected;
                overlayControl.ColorSelected += TileColorSelected;
                Context.ColorPickerControl = _paletteForm;
            }

            _paletteForm.Init();
            _paletteFormInitialized = true;
        }

        private void ReInitPaletteEditor()
        {
            if (!_paletteFormInitialized)
            {
                return;
            }

            if (!_paletteForm.Visible)
            {
                // Reinit the palette editor next time it's shown
                _paletteFormInitialized = false;
            }
            else
            {
                // Reinit the palette editor now
                _paletteForm.Init();
                _paletteForm.Editor.Theme = _track.Theme;
            }
        }

        private void TileColorSelected(object sender, EventArgs<Palette, int> e)
        {
            _paletteForm.Editor.Palette = e.Value1;
            _paletteForm.Editor.ColorIndex = e.Value2;
        }

        private void MenuBarBackgroundEditorRequested(object sender, EventArgs e)
        {
            if (!_backgroundFormInitialized)
            {
                InitBackgroundEditor();
            }

            if (_backgroundForm.Visible)
            {
                _backgroundForm.Visible = false;
            }
            else
            {
                _backgroundForm.Editor.Theme = _track.Theme;
                _backgroundForm.Visible = true;
            }
        }

        private void InitBackgroundEditor()
        {
            if (_backgroundForm == null)
            {
                _backgroundForm = new BackgroundEditorForm();
                _backgroundForm.Owner = ParentForm;
                _backgroundForm.ColorSelected += TileColorSelected;
            }

            _backgroundForm.Init();
            _backgroundFormInitialized = true;
        }

        private void ReInitBackgroundEditor()
        {
            if (!_backgroundFormInitialized)
            {
                return;
            }

            if (!_backgroundForm.Visible)
            {
                // Reinit the background editor next time it's shown
                _backgroundFormInitialized = false;
            }
            else
            {
                // Reinit the background editor now
                _backgroundForm.Init();
            }
        }

        private void MenuBarSettingEditorRequested(object sender, EventArgs e)
        {
            if (!_settingFormInitialized)
            {
                InitSettingEditor();
            }

            if (_settingForm.Visible)
            {
                _settingForm.Visible = false;
            }
            else
            {
                _settingForm.ShowTrackItemProbabilities(_track, false);
                _settingForm.Visible = true;
            }
        }

        private void ItemProbaEditorRequested(object sender, EventArgs e)
        {
            if (!_settingFormInitialized)
            {
                InitSettingEditor();
            }

            _settingForm.ShowTrackItemProbabilities(_track, true);
            _settingForm.Visible = true;
        }

        private void InitSettingEditor()
        {
            if (_settingForm == null)
            {
                _settingForm = new SettingEditorForm();
                _settingForm.Owner = ParentForm;
                _settingForm.ColorSelected += TileColorSelected;
                _settingForm.Theme = _track.Theme;
            }

            _settingForm.Init();
            _settingFormInitialized = true;
        }

        private void ReInitSettingEditor()
        {
            if (!_settingFormInitialized)
            {
                return;
            }

            if (!_settingForm.Visible)
            {
                // Reinit the setting editor next time it's shown
                _settingFormInitialized = false;
            }
            else
            {
                // Reinit the setting editor now
                _settingForm.Init();
            }
        }

        private void MenuBarCodecRequested(object sender, EventArgs e)
        {
            if (!_codecFormInitialized)
            {
                InitCodecForm();
            }

            _codecForm.Visible = !_codecForm.Visible;
        }

        private void InitCodecForm()
        {
            if (_codecForm == null)
            {
                _codecForm = new CodecForm();
                _codecForm.Owner = ParentForm;
            }

            _codecForm.Init();
            _codecFormInitialized = true;
        }

        private void ReInitCodecForm()
        {
            if (!_codecFormInitialized)
            {
                return;
            }

            if (!_codecForm.Visible)
            {
                // Reinit the codec form next time it's shown
                _codecFormInitialized = false;
            }
            else
            {
                // Reinit the codec form now
                _codecForm.Init();
            }
        }
        #endregion MenuBar

        #region TrackDisplay events
        private void TrackDisplayPaint(object sender, PaintEventArgs e)
        {
            switch (_editionMode)
            {
                case EditionMode.Tileset:
                    _drawer.DrawTrackTileset(e, TileSelectionRectangle, _buttonsPressed == MouseButtons.Right, tilesetControl.BucketMode);
                    break;

                case EditionMode.Overlay:
                    _drawer.DrawTrackOverlay(e, _hoveredOverlayTile, overlayControl.SelectedTile, overlayControl.SelectedPattern, _selectedOverlayPatternLocation);
                    break;

                case EditionMode.Start:
                    _drawer.DrawTrackStart(e);
                    break;

                case EditionMode.Objects:
                    _drawer.DrawTrackObjects(e, _hoveredObject, objectsControl.FrontAreasView);
                    break;

                case EditionMode.AI:
                    _drawer.DrawTrackAI(e, _hoveredAIElem, aiControl.SelectedElement, _aiAction == AIAction.DragTarget);
                    break;
            }
        }

        /// <summary>
        /// Fully invalidates the track display panel.
        /// </summary>
        private void InvalidateWholeTrackDisplay()
        {
            if (Platform.IsWindows)
            {
                _dirtyRegion.MakeInfinite();
            }
            else
            {
                // HACK: Work around the fact Mono does not properly support Region.MakeInfinite()
                _dirtyRegion.Union(trackDisplay.DisplayRectangle);
            }

            InvalidateTrackDisplay();
        }

        /// <summary>
        /// Partially invalidates the track display panel, depending on the current context.
        /// </summary>
        private void InvalidateTrackDisplay()
        {
            InvalidateTrackDisplay(GetFullDirtyRegion());
        }

        /// <summary>
        /// Partially invalidates the track display panel, using the passed region.
        /// </summary>
        /// <param name="region"></param>
        private void InvalidateTrackDisplay(DrawRegion region)
        {
            region.Union(GetFullDirtyRegion());
            InvalidateTrackDisplaySub(region);
        }

        private void InvalidateTrackDisplaySub(DrawRegion region)
        {
            trackDisplay.Invalidate(region);
        }

        private DrawRegion GetFullDirtyRegion()
        {
            DrawRegion region = GetCurrentDirtyRegion();
            DrawRegion regionTemp = region.Clone();

            region.Union(_dirtyRegion);

            _dirtyRegion.Dispose();
            _dirtyRegion = regionTemp;

            return region;
        }

        private DrawRegion GetCurrentDirtyRegion()
        {
            switch (_editionMode)
            {
                case EditionMode.Tileset:
                    return _drawer.GetTrackTilesetRegion(TileSelectionRectangle);

                case EditionMode.Overlay:
                    return _drawer.GetTrackOverlayRegion(_hoveredOverlayTile, overlayControl.SelectedTile, overlayControl.SelectedPattern, _selectedOverlayPatternLocation);

                case EditionMode.Start:
                    return _drawer.GetTrackStartRegion();

                case EditionMode.Objects:
                    return _drawer.GetTrackObjectsRegion(_hoveredObject);

                case EditionMode.AI:
                    return _drawer.GetTrackAIRegion(_hoveredAIElem, aiControl.SelectedElement);

                default:
                    throw new InvalidOperationException();
            }
        }

        private void TrackDisplayVScrollBarMouseMove(object sender, MouseEventArgs e)
        {
            if (Form.ActiveForm != null) // Application focused
            {
                trackDisplayVScrollBar.Focus(); // Lets you use the mouse wheel to scroll
            }
        }

        private void TrackDisplayVScrollBarMouseLeave(object sender, EventArgs e)
        {
            RemoveFocus();
        }

        private void TrackDisplayVScrollBarValueChanged(object sender, EventArgs e)
        {
            _scrollPosition.Y = trackDisplayVScrollBar.Value;
            trackDisplay.ScrollPositionY = _scrollPosition.Y;
            _drawer.ScrollPosition = _scrollPosition;
            RepaintAfterScrollingIfNeeded();
        }

        private void TrackDisplayVScrollBarScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                _repaintAfterScrolling = true;
            }
        }

        private void TrackDisplayHScrollBarValueChanged(object sender, EventArgs e)
        {
            _scrollPosition.X = trackDisplayHScrollBar.Value;
            trackDisplay.ScrollPositionX = _scrollPosition.X;
            _drawer.ScrollPosition = _scrollPosition;
            RepaintAfterScrollingIfNeeded();
        }

        private void TrackDisplayHScrollBarScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                _repaintAfterScrolling = true;
            }
        }

        private void SetHorizontalScrollingValue(int x)
        {
            SetScrollingValue(trackDisplayHScrollBar, x);
        }

        private void SetVerticalScrollingValue(int y)
        {
            SetScrollingValue(trackDisplayVScrollBar, y);
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
            if (_repaintAfterScrolling)
            {
                InvalidateWholeTrackDisplay();
                _repaintAfterScrolling = false;
            }
        }

        private void TrackDisplaySizeChanged(object sender, EventArgs e)
        {
            UpdateScrollBars();
            InvalidateWholeTrackDisplay();
        }

        private void TrackDisplayKeyDown(object sender, KeyEventArgs e)
        {
            if (_buttonsPressed != MouseButtons.None)
            {
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                if (_editionMode == EditionMode.Overlay)
                {
                    if (overlayControl.SelectedTile != null)
                    {
                        DeleteOverlayTile();
                    }
                }
                else if (_editionMode == EditionMode.AI)
                {
                    if (aiControl.SelectedElement != null)
                    {
                        DeleteAIElement();
                    }
                }
            }
            else if (ModifierKeys == Keys.Control)
            {
                if (_editionMode == EditionMode.Tileset)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.D1: tilesetControl.SetTheme(0); break;
                        case Keys.D2: tilesetControl.SetTheme(1); break;
                        case Keys.D3: tilesetControl.SetTheme(2); break;
                        case Keys.D4: tilesetControl.SetTheme(3); break;
                        case Keys.D5: tilesetControl.SetTheme(4); break;
                        case Keys.D6: tilesetControl.SetTheme(5); break;
                        case Keys.D7: tilesetControl.SetTheme(6); break;
                        case Keys.D8: tilesetControl.SetTheme(7); break;
                    }
                }
                else if (_editionMode == EditionMode.AI && aiControl.SelectedElement != null)
                {
                    if (e.KeyCode == Keys.D)
                    {
                        CloneAIElement();
                    }
                    else if (e.KeyCode == Keys.M)
                    {
                        aiControl.SelectedElement.AreaShape = aiControl.SelectedElement.AreaShape.Next();
                    }
                }
            }
            else if (_editionMode == EditionMode.Tileset)
            {
                switch (e.KeyCode)
                {
                    case Keys.B:
                        tilesetControl.SelectPenTool();
                        InitTilesetAction();
                        break;

                    case Keys.G:
                        tilesetControl.SelectPaintBucketTool();
                        InitTilesetAction();
                        break;
                }
            }
        }

        private void TrackDisplayMouseMove(object sender, MouseEventArgs e)
        {
            if (Form.ActiveForm != null) // Application focused
            {
                trackDisplay.Focus(); // Lets you use the mouse wheel to scroll
            }

            Point tilePositionBefore = TilePosition;
            SetPosition(e.Location);

            if (tilePositionBefore == TilePosition) // If the cursor has not moved to another tile
            {
                if (_editionMode == EditionMode.Start &&
                    _buttonsPressed != MouseButtons.Middle)
                {
                    // The only mode that needs pixel precision,
                    // as opposed to tile precision
                    if (InitStartAction())
                    {
                        InvalidateTrackDisplay();
                    }
                }
            }
            else
            {
                if (_buttonsPressed == MouseButtons.Middle)
                {
                    ScrollTrack();
                }
                else
                {
                    InitEditionModeAction();
                }
            }
        }

        private void SetPosition(Point location)
        {
            int x = location.X;
            int y = location.Y;
            int zoomedTileSize = (int)(Tile.Size * Zoom);
            int zoomedScrollPositionX = _scrollPosition.X * zoomedTileSize;
            int zoomedScrollPositionY = _scrollPosition.Y * zoomedTileSize;
            int zoomedTrackWidth = _track.Map.Width * zoomedTileSize;
            int zoomedTrackHeight = _track.Map.Height * zoomedTileSize;
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

            _pixelPosition = new Point(x, y);
        }

        private void ScrollTrack()
        {
            int xBefore = _scrollPosition.X;
            int yBefore = _scrollPosition.Y;

            SetHorizontalScrollingValue(_anchorPoint.X - TilePosition.X);
            SetVerticalScrollingValue(_anchorPoint.Y - TilePosition.Y);

            if (xBefore != _scrollPosition.X ||
                yBefore != _scrollPosition.Y)
            {
                InvalidateWholeTrackDisplay();
            }
        }

        private void RecalculateTileClipboard()
        {
            Point hoveredTilePosition = AbsoluteTilePosition;

            int x = Math.Min(_anchorPoint.X, hoveredTilePosition.X);
            int y = Math.Min(_anchorPoint.Y, hoveredTilePosition.Y);
            int width = Math.Abs(hoveredTilePosition.X - _anchorPoint.X) + 1;
            int height = Math.Abs(hoveredTilePosition.Y - _anchorPoint.Y) + 1;

            _tileClipboard.Rectangle = new Rectangle(x, y, width, height);
        }

        private void TrackDisplayMouseLeave(object sender, EventArgs e)
        {
            trackDisplay.Cursor = Cursors.Default;
            EndMouseAction(_buttonsPressed);

            RemoveFocus();

            ResetPosition();
            _hoveredOverlayTile = null;
            _selectedOverlayPatternLocation = OutOfBounds;
            _hoveredObject = null;
            _hoveredAIElem = null;

            InvalidateTrackDisplay();
        }

        private void TrackDisplayMouseDown(object sender, MouseEventArgs e)
        {
            // We only acknowledge the click if the cursor isn't out of bounds,
            // and if no mouse button is already pressed
            if (_pixelPosition == OutOfBounds ||
                _buttonsPressed != MouseButtons.None)
            {
                return;
            }

            if (e.Button == MouseButtons.Middle)
            {
                _buttonsPressed = MouseButtons.Middle;
                trackDisplay.Cursor = Cursors.SizeAll;
                _anchorPoint = AbsoluteTilePosition;
                InvalidateTrackDisplay();
            }
            else if (_editionMode == EditionMode.Tileset)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _buttonsPressed = MouseButtons.Left;

                        if (!tilesetControl.BucketMode)
                        {
                            UndoRedoBuffer.BeginAdd();
                            LayTiles();
                            InvalidateTrackDisplay();
                        }
                        else
                        {
                            if (FillTiles())
                            {
                                InvalidateTrackDisplay();
                            }
                        }
                        break;

                    case MouseButtons.Right:
                        _buttonsPressed = MouseButtons.Right;

                        _anchorPoint = AbsoluteTilePosition;
                        _tileClipboard.Rectangle = new Rectangle(_anchorPoint.X, _anchorPoint.Y, 1, 1);

                        InvalidateTrackDisplay();
                        break;
                }
            }
            else if (_editionMode == EditionMode.Overlay)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (overlayControl.SelectedPattern == null)
                        {
                            overlayControl.SelectedTile = _hoveredOverlayTile;

                            if (overlayControl.SelectedTile != null)
                            {
                                _buttonsPressed = MouseButtons.Left;
                                Point hoveredTilePosition = AbsoluteTilePosition;
                                _anchorPoint = new Point(hoveredTilePosition.X - overlayControl.SelectedTile.X,
                                                         hoveredTilePosition.Y - overlayControl.SelectedTile.Y);
                            }
                        }
                        else
                        {
                            OverlayTile overlayTile = new OverlayTile(overlayControl.SelectedPattern, _selectedOverlayPatternLocation);
                            _track.OverlayTiles.Add(overlayTile);
                        }
                        break;

                    case MouseButtons.Right:
                        if (_hoveredOverlayTile == null)
                        {
                            overlayControl.SelectedPattern = null;
                            InitOverlayAction();
                        }
                        else
                        {
                            overlayControl.SelectedPattern = _hoveredOverlayTile.Pattern;
                            SetSelectedOverlayPatternLocation();
                            _hoveredOverlayTile = null;
                            trackDisplay.Cursor = Cursors.Default;
                        }
                        break;
                }

                InvalidateTrackDisplay();
            }
            else if (_editionMode == EditionMode.Start)
            {
                if (e.Button != MouseButtons.Left ||
                    _startAction == StartAction.None)
                {
                    return;
                }

                _buttonsPressed = MouseButtons.Left;
                Point absPixelPos = AbsolutePixelPosition;

                if (_track is GPTrack gpTrack)
                {
                    if (_startAction == StartAction.DragStartPosition)
                    {
                        _anchorPoint = new Point(absPixelPos.X - gpTrack.StartPosition.X,
                                                 absPixelPos.Y - gpTrack.StartPosition.Y);
                    }
                    else
                    {
                        // ie: StartAction.DragLapLine, ResizeLapLine or DragAll
                        _anchorPoint = new Point(absPixelPos.X - gpTrack.LapLine.X,
                                                 absPixelPos.Y - gpTrack.LapLine.Y);
                    }
                }
                else
                {
                    BattleTrack bTrack = (BattleTrack)_track;

                    if (_startAction == StartAction.DragStartPosition)
                    {
                        _anchorPoint = new Point(absPixelPos.X - bTrack.StartPositionP1.X,
                                                 absPixelPos.Y - bTrack.StartPositionP1.Y);
                    }
                    else
                    {
                        _anchorPoint = new Point(absPixelPos.X - bTrack.StartPositionP2.X,
                                                 absPixelPos.Y - bTrack.StartPositionP2.Y);
                    }
                }
            }
            else if (_editionMode == EditionMode.Objects)
            {
                if (_hoveredObject == null)
                {
                    return;
                }

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _buttonsPressed = MouseButtons.Left;
                        InvalidateTrackDisplay();
                        break;

                    case MouseButtons.Right:
                        if (!(_hoveredObject is TrackObjectMatchRace))
                        {
                            break;
                        }

                        TrackObjectMatchRace hoveredObjectMatchRace = (TrackObjectMatchRace)_hoveredObject;

                        if (hoveredObjectMatchRace.Direction == TrackObjectDirection.Horizontal)
                        {
                            hoveredObjectMatchRace.Direction = TrackObjectDirection.Vertical;
                        }
                        else if (hoveredObjectMatchRace.Direction == TrackObjectDirection.Vertical)
                        {
                            hoveredObjectMatchRace.Direction = TrackObjectDirection.None;
                        }
                        else // hoveredObjectMatchRace.Direction == Direction.None
                        {
                            hoveredObjectMatchRace.Direction = TrackObjectDirection.Horizontal;
                        }

                        InvalidateTrackDisplay();
                        break;
                }
            }
            else if (_editionMode == EditionMode.AI)
            {
                if (_hoveredAIElem == null)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        aiControl.SelectedElement = null;
                        InvalidateTrackDisplay();
                    }
                    return;
                }

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _buttonsPressed = MouseButtons.Left;
                        aiControl.SelectedElement = _hoveredAIElem;
                        InvalidateTrackDisplay();
                        break;

                    case MouseButtons.Right:
                        if (_hoveredAIElem.Speed < 3)
                        {
                            _hoveredAIElem.Speed++;
                        }
                        else
                        {
                            _hoveredAIElem.Speed = 0;
                        }

                        aiControl.SelectedElement = _hoveredAIElem;
                        InvalidateTrackDisplay();
                        break;
                }
            }
        }

        private void TrackDisplayMouseUp(object sender, MouseEventArgs e)
        {
            EndMouseAction(e.Button);
        }

        private void EndMouseAction(MouseButtons button)
        {
            // When the user releases a mouse button, we ensure this button is the same
            // as the one that was initially held down before validating the action.
            switch (button)
            {
                case MouseButtons.Middle:
                    if (_buttonsPressed != MouseButtons.Middle)
                    {
                        break;
                    }

                    _buttonsPressed = MouseButtons.None;

                    if (_track is BattleTrack && _editionMode == EditionMode.Objects)
                    {
                        // For other modes, the cursor will be reset
                        // by the call to the InitEditionModeAction method below.
                        trackDisplay.Cursor = Cursors.Default;
                    }

                    InitEditionModeAction();
                    break;

                case MouseButtons.Left:
                    if (_buttonsPressed != MouseButtons.Left)
                    {
                        break;
                    }

                    _buttonsPressed = MouseButtons.None;

                    if (_editionMode == EditionMode.Tileset)
                    {
                        UndoRedoBuffer.EndAdd();
                        ToggleUndoRedo();
                    }

                    break;

                case MouseButtons.Right:
                    if (_buttonsPressed != MouseButtons.Right)
                    {
                        break;
                    }

                    _buttonsPressed = MouseButtons.None;

                    if (_editionMode == EditionMode.Tileset)
                    {
                        OnRightMouseButtonRelease();
                    }
                    break;
            }
        }

        private void TrackDisplayMouseWheel(object sender, MouseEventArgs e)
        {
            if (!trackDisplay.Focused)
            {
                return;
            }

            if (ModifierKeys == Keys.Control)
            {
                MouseWheelZoom(e);
            }
            else if (_buttonsPressed != MouseButtons.Middle)
            {
                MouseWheelScroll(e);
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
            if (ModifierKeys != Keys.Shift)
            {
                int y = _scrollPosition.Y - delta;
                before = _scrollPosition.Y;
                SetVerticalScrollingValue(y);
                after = _scrollPosition.Y;
            }
            else
            {
                int x = _scrollPosition.X - delta;
                before = _scrollPosition.X;
                SetHorizontalScrollingValue(x);
                after = _scrollPosition.X;
            }

            if (before != after)
            {
                InitEditionModeAction(true);
            }
        }

        private void TrackDisplayMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_editionMode == EditionMode.AI &&
                e.Button == MouseButtons.Left &&
                _hoveredAIElem == null)
            {
                AddAIElement(AbsoluteTilePosition);
            }
        }
        #endregion TrackDisplay events

        #region TrackDisplay methods
        private void InvalidateTrack()
        {
            _drawer.CreateCache();
            InvalidateWholeTrackDisplay();
        }

        private void ResetPosition()
        {
            _pixelPosition = OutOfBounds;
            menuBar.ResetCoordinates();
        }

        /// <summary>
        /// Updates the horizontal and vertical scroll bar properties
        /// depending on the track display panel size and zoom level.
        /// </summary>
        private void UpdateScrollBars()
        {
            GetOffScreenTileCounts(out int offScreenTileCountX, out int offScreenTileCountY);

            // Recalculate the maximum value of the horizontal scroll bar
            UpdateScrollBar(trackDisplayHScrollBar, offScreenTileCountX);

            // Recalculate the maximum value of the vertical scroll bar
            UpdateScrollBar(trackDisplayVScrollBar, offScreenTileCountY);
        }

        /// <summary>
        /// Gets the number of track tiles that are off screen (not visible), horizontally and vertically.
        /// </summary>
        /// <param name="offScreenTileCountX">The number of horizontally off-screen tiles (from 0 to 128).</param>
        /// <param name="offScreenTileCountY">The number of vertically off-screen tiles (from 0 to 128).</param>
        private void GetOffScreenTileCounts(out int offScreenTileCountX, out int offScreenTileCountY)
        {
            offScreenTileCountX = GetOffScreenTileCount(trackDisplay.Width);
            offScreenTileCountY = GetOffScreenTileCount(trackDisplay.Height);
            int offScreenTileCountXWithScrollBar = GetOffScreenTileCount(Math.Max(0, trackDisplay.Width - trackDisplayVScrollBar.Width));
            int offScreenTileCountYWithScrollBar = GetOffScreenTileCount(Math.Max(0, trackDisplay.Height - trackDisplayHScrollBar.Height));

            bool? horizontalScrollBarNeeded = IsScrollBarNeeded(offScreenTileCountX, offScreenTileCountXWithScrollBar);
            bool? verticalScrollBarNeeded = IsScrollBarNeeded(offScreenTileCountY, offScreenTileCountYWithScrollBar);

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
            return _track.Map.Width - GetOnScreenTileCount(panelSize); // Map.Width = Map.Height
        }

        /// <summary>
        /// Gets the number of track tiles that are on screen (visible).
        /// </summary>
        /// <param name="panelSize">The width or height of the track display panel.</param>
        /// <returns>The number of on-screen tiles (from 0 to 128).</returns>
        private int GetOnScreenTileCount(int panelSize)
        {
            return Math.Min(_track.Map.Width, (int)((panelSize) / (Tile.Size * Zoom)));
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

                if (scrollBar.Value > offScreenTileCount)
                {
                    // Reposition the track content properly (avoiding to show off-track black)
                    // when resizing the window and the panel is scrolled to the bottom and/or right limit.
                    scrollBar.Value = offScreenTileCount;
                }

                int onScreenTileCount = _track.Map.Width - offScreenTileCount; // Map.Width = Map.Height
                scrollBar.Maximum = offScreenTileCount + (onScreenTileCount - 1);
                scrollBar.LargeChange = onScreenTileCount;
                // Adding the equivalent of LargeChange - 1 to the Maximum because
                // it's not possible for users to reach the scroll bar maximum.
                // "The maximum value that can be reached through user interaction is equal to
                // 1 plus the Maximum property value minus the LargeChange property value."
                // See: http://msdn.microsoft.com/en-us/library/system.windows.forms.scrollbar.maximum.aspx
            }
        }

        private void UpdateTileClipboard()
        {
            _drawer.UpdateTileClipboardCache(_tileClipboard.Rectangle);
        }
        #endregion TrackDisplay methods

        #region TrackTreeView
        private void TrackTreeViewSelectedTrackChanged(object sender, EventArgs e)
        {
            ResetScrollingPosition();
            SetTrack();
            SetUndoRedo();
            InvalidateWholeTrackDisplay();
        }

        private void ResetScrollingPosition()
        {
            trackDisplayHScrollBar.Value = 0;
            trackDisplayVScrollBar.Value = 0;
        }

        private void SetTrack()
        {
            if (_track != null)
            {
                _track.PropertyChanged -= track_PropertyChanged;
                _track.OverlayTiles.ElementRemoved -= track_OverlayTiles_ElementRemoved;
                _track.OverlayTiles.ElementsCleared -= track_OverlayTiles_ElementsCleared;
                _track.AI.PropertyChanged -= track_AI_PropertyChanged;
                _track.AI.ElementAdded -= track_AI_ElementAdded;
                _track.AI.ElementRemoved -= track_AI_ElementRemoved;
                _track.AI.ElementsCleared -= track_AI_ElementsCleared;

                if (_track is GPTrack oldGPTrack)
                {
                    oldGPTrack.StartPosition.PropertyChanged -= gpTrack_StartPosition_PropertyChanged;
                    oldGPTrack.Objects.PropertyChanged -= gpTrack_Objects_PropertyChanged;
                }
            }

            _track = trackTreeView.SelectedTrack;

            _track.PropertyChanged += track_PropertyChanged;
            _track.OverlayTiles.ElementRemoved += track_OverlayTiles_ElementRemoved;
            _track.OverlayTiles.ElementsCleared += track_OverlayTiles_ElementsCleared;
            _track.AI.PropertyChanged += track_AI_PropertyChanged;
            _track.AI.ElementAdded += track_AI_ElementAdded;
            _track.AI.ElementRemoved += track_AI_ElementRemoved;
            _track.AI.ElementsCleared += track_AI_ElementsCleared;

            GPTrack gpTrack = _track as GPTrack;
            if (gpTrack != null)
            {
                gpTrack.StartPosition.PropertyChanged += gpTrack_StartPosition_PropertyChanged;
                gpTrack.Objects.PropertyChanged += gpTrack_Objects_PropertyChanged;
            }

            trackDisplay.Track = _track;

            tilesetControl.Track = _track;
            _hoveredOverlayTile = null;
            overlayControl.Track = _track;
            startControl.Track = _track;
            objectsControl.Track = gpTrack;
            _hoveredAIElem = null;
            aiControl.Track = _track;

            _drawer.LoadTrack(_track);
        }

        private void track_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.Track.Theme)
            {
                InvalidateTrack();
            }
        }

        private void track_OverlayTiles_ElementRemoved(object sender, EventArgs<OverlayTile> e)
        {
            if (_hoveredOverlayTile == e.Value)
            {
                _hoveredOverlayTile = null;
            }

            InitOverlayAction();
            InvalidateTrackDisplay();
        }

        private void track_OverlayTiles_ElementsCleared(object sender, EventArgs e)
        {
            InvalidateWholeTrackDisplay();
        }

        private void gpTrack_StartPosition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.GPStartPosition.SecondRowOffset)
            {
                InvalidateTrackDisplay();
            }
        }

        private void gpTrack_Objects_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case PropertyNames.TrackObjectProperties.Tileset:
                case PropertyNames.TrackObjectProperties.Routine:
                case PropertyNames.TrackObjectProperties.Palette:
                    InvalidateWholeTrackDisplay();
                    break;

                case PropertyNames.TrackObjectAreas.FrontView:
                    if (objectsControl.FrontAreasView)
                    {
                        InvalidateWholeTrackDisplay();
                    }
                    break;

                case PropertyNames.TrackObjectAreas.RearView:
                    if (!objectsControl.FrontAreasView)
                    {
                        InvalidateWholeTrackDisplay();
                    }
                    break;
            }
        }

        private void track_AI_PropertyChanged(object sender, EventArgs e)
        {
            InvalidateTrackDisplay();
        }

        private void track_AI_ElementAdded(object sender, EventArgs<TrackAIElement> e)
        {
            aiControl.SelectedElement = e.Value;
            InitAIAction();
            InvalidateTrackDisplay();
        }

        private void track_AI_ElementRemoved(object sender, EventArgs<TrackAIElement> e)
        {
            if (_hoveredAIElem == e.Value)
            {
                _hoveredAIElem = null;
            }

            if (aiControl.SelectedElement == e.Value)
            {
                aiControl.SelectedElement = null;
            }

            InitAIAction();
            InvalidateTrackDisplay();
        }

        private void track_AI_ElementsCleared(object sender, EventArgs e)
        {
            aiControl.SelectedElement = null;
            InvalidateWholeTrackDisplay();
        }
        #endregion TrackTreeView

        #region EditionMode tabs
        /// <summary>
        /// Init an action depending on the current edition mode.
        /// </summary>
        private void InitEditionModeAction()
        {
            InitEditionModeAction(false);
        }

        /// <summary>
        /// Init an action depending on the current edition mode.
        /// </summary>
        /// <param name="forceFullRepaint">If true: trigger a full repaint after the action.
        /// If false: trigger a partial repaint if the action resulted in a change on the track.</param>
        private void InitEditionModeAction(bool forceFullRepaint)
        {
            bool repaintNeeded;

            if (Context.ColorPickerMode)
            {
                repaintNeeded = false;
            }
            else
            {
                switch (_editionMode)
                {
                    case EditionMode.Tileset:
                        repaintNeeded = InitTilesetAction();
                        break;

                    case EditionMode.Overlay:
                        repaintNeeded = InitOverlayAction();
                        break;

                    case EditionMode.Start:
                        repaintNeeded = InitStartAction();
                        break;

                    case EditionMode.Objects:
                        repaintNeeded = InitObjectAction();
                        break;

                    case EditionMode.AI:
                        repaintNeeded = InitAIAction();
                        break;

                    default:
                        repaintNeeded = false;
                        break;
                }
            }

            if (repaintNeeded || forceFullRepaint)
            {
                if (forceFullRepaint)
                {
                    InvalidateWholeTrackDisplay();
                }
                else
                {
                    InvalidateTrackDisplay();
                }
            }

            menuBar.UpdateCoordinates(AbsoluteTilePosition);
        }

        private void SetEditionMode()
        {
            if (tilesetTabPage.Visible)
            {
                _editionMode = EditionMode.Tileset;
            }
            else if (overlayTabPage.Visible)
            {
                _editionMode = EditionMode.Overlay;
            }
            else if (startTabPage.Visible)
            {
                _editionMode = EditionMode.Start;
            }
            else if (objectsTabPage.Visible)
            {
                _editionMode = EditionMode.Objects;
            }
            else // if (this.aiTabPage.Visible)
            {
                _editionMode = EditionMode.AI;
            }

            trackDisplay.EditionMode = _editionMode;
            SetUndoRedo();
        }

        private void ModeTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            SetEditionMode();
            ResizeModeTabControl();
            InvalidateWholeTrackDisplay();
        }

        private void ModeTabControlClientSizeChanged(object sender, EventArgs e)
        {
            int widthBefore = modeTabControl.Width;
            ResizeModeTabControl();
            int difference = modeTabControl.Width - widthBefore;

            if (difference != 0)
            {
                // Properly reposition the modeTabControl
                modeTabControl.Left -= difference;
            }
        }

        /// <summary>
        /// Adapts the modeTabControl width depending on whether its vertical scroll bar is visible.
        /// </summary>
        private void ResizeModeTabControl()
        {
            modeTabControl.Width =
                modeTabControl.SelectedTab.VerticalScroll.Visible ?
                144 + SystemInformation.VerticalScrollBarWidth : 144;
        }
        #endregion EditionMode tabs


        #region EditionMode.Tileset
        private bool InitTilesetAction()
        {
            if (tilesetControl.BucketMode)
            {
                trackDisplay.Cursor = EpicCursors.BucketCursor;
            }
            else
            {
                trackDisplay.Cursor = EpicCursors.PencilCursor;

                switch (_buttonsPressed)
                {
                    case MouseButtons.Left:
                        LayTiles();
                        break;

                    case MouseButtons.Right:
                        RecalculateTileClipboard();
                        break;
                }
            }

            return true; // Repaint always needed in tileset mode
        }

        private void LayTiles()
        {
            LayTiles(AbsoluteTilePosition);
        }

        private void LayTiles(Point location)
        {
            Size affectedSurface = GetTruncatedRectangle();
            AddUndoChange(location.X, location.Y, affectedSurface.Width, affectedSurface.Height);
            _track.Map.SetTiles(location, _tileClipboard);
            _drawer.UpdateCacheOnTileChange(location);
        }

        private bool FillTiles()
        {
            return FillTiles(AbsoluteTilePosition);
        }

        private bool FillTiles(Point location)
        {
            TileChange change = GetPaintBucketChange(location, _tileClipboard.FirstTile);

            if (change == null)
            {
                return false;
            }

            UndoRedoBuffer.BeginAdd();
            AddUndoChange(change.X, change.Y, change.Width, change.Height);
            UndoRedoBuffer.EndAdd();

            _track.Map.SetTiles(new Point(change.X, change.Y), change);
            _drawer.UpdateCache(change.Rectangle);

            _dirtyRegion.Union(new RectangleF(
                (change.X - _scrollPosition.X) * Tile.Size * Zoom,
                (change.Y - _scrollPosition.Y) * Tile.Size * Zoom,
                change.Width * Tile.Size * Zoom,
                change.Height * Tile.Size * Zoom));

            return true;
        }

        private TileChange GetPaintBucketChange(Point location, byte tile)
        {
            byte targetTile = _track.Map[location.X, location.Y];

            if (targetTile == tile)
            {
                return null;
            }

            TrackMap tempBuffer = new TrackMap(_track.Map.GetBytes());
            int minX = TrackMap.Size;
            int minY = TrackMap.Size;
            int maxX = 0;
            int maxY = 0;

            Queue<Point> locations = new Queue<Point>();
            locations.Enqueue(location);

            while (locations.Count > 0)
            {
                Point loc = locations.Dequeue();
                int x = loc.X;
                int y = loc.Y;
                int left = x;
                int right = x;

                do { left--; }
                while (left >= 0 && tempBuffer[left, y] == targetTile);
                left++;

                do { right++; }
                while (right < TrackMap.Size && tempBuffer[right, y] == targetTile);
                right--;

                minX = Math.Min(minX, left);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, right);
                maxY = Math.Max(maxY, y);

                for (int xIter = left; xIter <= right; xIter++)
                {
                    tempBuffer[xIter, y] = tile;

                    if (y > 0 && tempBuffer[xIter, y - 1] == targetTile)
                    {
                        locations.Enqueue(new Point(xIter, y - 1));
                    }

                    if (y < TrackMap.Size - 1 && tempBuffer[xIter, y + 1] == targetTile)
                    {
                        locations.Enqueue(new Point(xIter, y + 1));
                    }
                }
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            return new TileChange(minX, minY, width, height, tempBuffer);
        }

        private void AddUndoChange(int x, int y, int width, int height)
        {
            TileChange tileChange = new TileChange(x, y, width, height, _track.Map);
            UndoRedoBuffer.Add(tileChange);
        }

        private Size GetTruncatedRectangle()
        {
            Size rectangleToDisplay = _tileClipboard.Size;
            if ((_scrollPosition.X + TilePosition.X + rectangleToDisplay.Width) >= _track.Map.Width)
            {
                int subFromWidth = _scrollPosition.X + TilePosition.X + rectangleToDisplay.Width - _track.Map.Width;
                rectangleToDisplay.Width -= subFromWidth;
            }
            if ((_scrollPosition.Y + TilePosition.Y + rectangleToDisplay.Height) >= _track.Map.Height)
            {
                int subFromHeight = _scrollPosition.Y + TilePosition.Y + rectangleToDisplay.Height - _track.Map.Height;
                rectangleToDisplay.Height -= subFromHeight;
            }
            return rectangleToDisplay;
        }

        private void OnRightMouseButtonRelease()
        {
            _tileClipboard.Fill(_track.Map);
            tilesetControl.SelectedTile = _tileClipboard.FirstTile;
            InvalidateTrackDisplay();
        }

        private void TilesetControlSelectedThemeChanged(object sender, EventArgs e)
        {
            if (_settingFormInitialized)
            {
                _settingForm.Theme = _track.Theme;
            }
        }

        private void TilesetControlSelectedTileChanged(object sender, EventArgs e)
        {
            _tileClipboard.Fill(tilesetControl.SelectedTile);
        }

        private void TilesetControlTrackMapChanged(object sender, EventArgs e)
        {
            UndoRedoBuffer.Clear();
            menuBar.UndoEnabled = false;
            menuBar.RedoEnabled = false;

            InvalidateTrack();
        }

        private void TilesetControlTileChanged(object sender, EventArgs<byte> e)
        {
            _drawer.UpdateCache(e.Value);
            UpdateTileClipboard();
            tilesetControl.UpdateTileset();
            overlayControl.UpdateTileset();
            InvalidateWholeTrackDisplay();
            trackDisplay.Update();
        }

        private void TilesetControlTilesetChanged(object sender, EventArgs e)
        {
            InvalidateTrack();
            UpdateTileClipboard();
            overlayControl.UpdateTileset();
        }
        #endregion EditionMode.Tileset

        #region EditionMode.Overlay
        private bool InitOverlayAction()
        {
            Point hoveredTilePosition = AbsoluteTilePosition;

            if (_buttonsPressed == MouseButtons.Left)
            {
                // Drag overlay tile
                overlayControl.SelectedTile.Location =
                    new Point(hoveredTilePosition.X - _anchorPoint.X,
                              hoveredTilePosition.Y - _anchorPoint.Y);
                return true;
            }

            if (overlayControl.SelectedPattern == null)
            {
                // Try to hover overlay tile
                foreach (OverlayTile overlayTile in _track.OverlayTiles)
                {
                    if (overlayTile.IntersectsWith(hoveredTilePosition))
                    {
                        trackDisplay.Cursor = Cursors.Hand;

                        if (_hoveredOverlayTile == overlayTile)
                        {
                            return false;
                        }

                        _hoveredOverlayTile = overlayTile;
                        return true;
                    }
                }

                trackDisplay.Cursor = Cursors.Default;

                if (_hoveredOverlayTile == null)
                {
                    return false;
                }

                _hoveredOverlayTile = null;
                return true;
            }

            // Move selected tile pattern
            trackDisplay.Cursor = Cursors.Default;
            Point originalPatternLocation = _selectedOverlayPatternLocation;
            SetSelectedOverlayPatternLocation();
            // Return whether the location has changed
            return originalPatternLocation.X != _selectedOverlayPatternLocation.X ||
                   originalPatternLocation.Y != _selectedOverlayPatternLocation.Y;
        }

        private void DeleteOverlayTile()
        {
            _track.OverlayTiles.Remove(overlayControl.SelectedTile);
        }

        private void OverlayControlRepaintRequested(object sender, EventArgs e)
        {
            InvalidateTrackDisplay();
        }

        private void SetSelectedOverlayPatternLocation()
        {
            OverlayTilePattern pattern = overlayControl.SelectedPattern;

            Point tilePosition = TilePosition;
            if (tilePosition == OutOfBounds)
            {
                // The mouse cursor isn't over the track map
                _selectedOverlayPatternLocation = tilePosition;
            }
            else
            {
                Point hoveredTilePosition = AbsoluteTilePosition;
                int x = hoveredTilePosition.X - pattern.Width / 2;
                int y = hoveredTilePosition.Y - pattern.Height / 2;

                if (x < 0)
                {
                    x = 0;
                }
                else if (x + pattern.Width > _track.Map.Width)
                {
                    x = _track.Map.Width - pattern.Width;
                }

                if (y < 0)
                {
                    y = 0;
                }
                else if (y + pattern.Height > _track.Map.Height)
                {
                    y = _track.Map.Height - pattern.Height;
                }

                _selectedOverlayPatternLocation = new Point(x, y);
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

            return _track is GPTrack ?
                InitGPStartAction() :
                InitBattleStartAction();
        }

        private bool InitGPStartAction()
        {
            GPTrack gpTrack = (GPTrack)_track;
            Point absPixelPos = AbsolutePixelPosition;

            if (_buttonsPressed == MouseButtons.Left)
            {
                bool dataChanged = false;

                switch (_startAction)
                {
                    case StartAction.DragLapLine:
                        // Move lap line
                        int step = startControl.Precision;
                        Point destination = new Point(absPixelPos.X - _anchorPoint.X,
                                                      ((absPixelPos.Y - _anchorPoint.Y) / step) * step);

                        int xBefore = gpTrack.LapLine.X;
                        int yBefore = gpTrack.LapLine.Y;

                        gpTrack.LapLine.Location = destination;

                        if (gpTrack.LapLine.X != xBefore || gpTrack.LapLine.Y != yBefore)
                        {
                            dataChanged = true;
                        }
                        break;

                    case StartAction.ResizeLapLine:
                        // Resize lap line
                        int lengthBefore = gpTrack.LapLine.Length;

                        gpTrack.LapLine.Resize(_resizeHandle, absPixelPos.X);

                        if (gpTrack.LapLine.Length != lengthBefore)
                        {
                            dataChanged = true;
                        }
                        break;

                    case StartAction.DragStartPosition:
                        // Move driver starting position
                        step = startControl.Precision;
                        destination = new Point(((absPixelPos.X - _anchorPoint.X) / step) * step,
                                                ((absPixelPos.Y - _anchorPoint.Y) / step) * step);

                        xBefore = gpTrack.StartPosition.X;
                        yBefore = gpTrack.StartPosition.Y;

                        gpTrack.StartPosition.Location = destination;

                        if (gpTrack.StartPosition.X != xBefore || gpTrack.StartPosition.Y != yBefore)
                        {
                            dataChanged = true;
                        }
                        break;

                    case StartAction.DragAll:
                        // Move both the lap line and the driver starting position
                        step = startControl.Precision;
                        destination = new Point(absPixelPos.X - _anchorPoint.X,
                                                ((absPixelPos.Y - _anchorPoint.Y) / step) * step);

                        xBefore = gpTrack.LapLine.X;
                        yBefore = gpTrack.LapLine.Y;

                        gpTrack.LapLine.Location = destination;

                        int xDifference = gpTrack.LapLine.X - xBefore;
                        int yDifference = gpTrack.LapLine.Y - yBefore;

                        if (xDifference != 0 || yDifference != 0)
                        {
                            destination = new Point(gpTrack.StartPosition.X + xDifference,
                                                    gpTrack.StartPosition.Y + yDifference);

                            gpTrack.StartPosition.Location = destination;
                            dataChanged = true;
                        }
                        break;
                }

                return dataChanged;
            }

            if (gpTrack.LapLine.IntersectsWith(absPixelPos))
            {
                _resizeHandle = gpTrack.LapLine.GetResizeHandle(absPixelPos);

                if (_resizeHandle == ResizeHandle.None)
                {
                    _startAction = startControl.LapLineAndDriverPositionsBound ?
                        StartAction.DragAll : StartAction.DragLapLine;
                    trackDisplay.Cursor = Cursors.SizeAll;
                }
                else
                {
                    _startAction = StartAction.ResizeLapLine;
                    trackDisplay.Cursor = Cursors.SizeWE;
                }
            }
            else if (gpTrack.StartPosition.IntersectsWith(absPixelPos))
            {
                _startAction = startControl.LapLineAndDriverPositionsBound ?
                    StartAction.DragAll : StartAction.DragStartPosition;
                trackDisplay.Cursor = Cursors.SizeAll;
            }
            else
            {
                _startAction = StartAction.None;
                trackDisplay.Cursor = Cursors.Default;
            }

            return false;
        }

        private bool InitBattleStartAction()
        {
            BattleTrack bTrack = (BattleTrack)_track;

            if (_buttonsPressed == MouseButtons.Left)
            {
                BattleStartPosition position = _startAction == StartAction.DragStartPosition ?
                    bTrack.StartPositionP1 : bTrack.StartPositionP2;

                return InitBattleStartActionSub(position);
            }

            Point absPixelPos = AbsolutePixelPosition;

            if (bTrack.StartPositionP1.IntersectsWith(absPixelPos))
            {
                _startAction = StartAction.DragStartPosition;
                trackDisplay.Cursor = Cursors.Hand;
            }
            else if (bTrack.StartPositionP2.IntersectsWith(absPixelPos))
            {
                _startAction = StartAction.DragStartPosition2;
                trackDisplay.Cursor = Cursors.Hand;
            }
            else
            {
                _startAction = StartAction.None;
                trackDisplay.Cursor = Cursors.Default;
            }

            return false;
        }

        private bool InitBattleStartActionSub(BattleStartPosition position)
        {
            Point absPixelPos = AbsolutePixelPosition;

            int step = startControl.Precision;
            Point destination = new Point(((absPixelPos.X - _anchorPoint.X) / step) * step,
                                         ((absPixelPos.Y - _anchorPoint.Y) / step) * step);

            int xBefore = position.X;
            int yBefore = position.Y;

            position.Location = destination;

            return position.X != xBefore || position.Y != yBefore;
        }
        #endregion EditionMode.Start

        #region EditionMode.Objects
        private bool InitObjectAction()
        {
            if (!(_track is GPTrack gpTrack))
            {
                return false;
            }

            Point hoveredTilePosition = AbsoluteTilePosition;

            if (_buttonsPressed == MouseButtons.Left)
            {
                // Drag object
                _hoveredObject.Location = hoveredTilePosition;
                return true;
            }

            // Try to hover object
            if (gpTrack.Objects.Routine == TrackObjectType.Pillar)
            {
                // Not supported yet
                trackDisplay.Cursor = Cursors.Default;
                return false;
            }

            foreach (TrackObject trackObject in gpTrack.Objects)
            {
                if (trackObject.X == hoveredTilePosition.X &&
                    trackObject.Y == hoveredTilePosition.Y)
                {
                    _hoveredObject = trackObject;
                    trackDisplay.Cursor = Cursors.Hand;
                    return true;
                }
            }

            trackDisplay.Cursor = Cursors.Default;

            if (_hoveredObject != null)
            {
                _hoveredObject = null;
                return true;
            }

            return false;
        }

        private void ObjectsControlViewChanged(object sender, EventArgs e)
        {
            InvalidateWholeTrackDisplay();
        }
        #endregion EditionMode.Objects

        #region EditionMode.AI
        private bool InitAIAction()
        {
            Point hoveredTilePosition = AbsoluteTilePosition;

            if (_buttonsPressed == MouseButtons.Left)
            {
                // Drag or resize AI element
                bool dataChanged = false;

                if (_aiAction == AIAction.DragTarget)
                {
                    // Drag AI target
                    _hoveredAIElem.Target = hoveredTilePosition;
                    dataChanged = true;
                }
                else if (_aiAction == AIAction.DragArea)
                {
                    // Drag AI area
                    int xBefore = _hoveredAIElem.Area.X;
                    int yBefore = _hoveredAIElem.Area.Y;

                    _hoveredAIElem.Location =
                        new Point(hoveredTilePosition.X - _anchorPoint.X,
                                  hoveredTilePosition.Y - _anchorPoint.Y);

                    if (xBefore != _hoveredAIElem.Area.X ||
                        yBefore != _hoveredAIElem.Area.Y)
                    {
                        dataChanged = true;
                    }
                }
                else if (_aiAction == AIAction.ResizeArea)
                {
                    // Resize AI area
                    int widthBefore = _hoveredAIElem.Area.Width;
                    int heightBefore = _hoveredAIElem.Area.Height;

                    _hoveredAIElem.Resize(_resizeHandle,
                                              hoveredTilePosition.X,
                                              hoveredTilePosition.Y);

                    int widthAfter = _hoveredAIElem.Area.Width;
                    int heightAfter = _hoveredAIElem.Area.Height;

                    if (widthBefore != widthAfter || heightBefore != heightAfter)
                    {
                        dataChanged = true;
                    }
                }

                return dataChanged;
            }

            // Try to hover AI target
            // Priority to selected element
            if (aiControl.SelectedElement != null &&
                aiControl.SelectedElement.Target.X == hoveredTilePosition.X &&
                aiControl.SelectedElement.Target.Y == hoveredTilePosition.Y)
            {
                // Hover AI target
                _hoveredAIElem = aiControl.SelectedElement;
                _aiAction = AIAction.DragTarget;
                trackDisplay.Cursor = Cursors.Hand;
                return true;
            }

            foreach (TrackAIElement trackAIElem in _track.AI)
            {
                if (trackAIElem.Target.X == hoveredTilePosition.X &&
                    trackAIElem.Target.Y == hoveredTilePosition.Y)
                {
                    // Hover AI target
                    _hoveredAIElem = trackAIElem;
                    _aiAction = AIAction.DragTarget;
                    trackDisplay.Cursor = Cursors.Hand;
                    return true;
                }
            }

            // Try to hover AI area
            // Priority to selected element
            if (aiControl.SelectedElement != null &&
                TryToHoverAIArea(aiControl.SelectedElement, hoveredTilePosition))
            {
                // If an element is already selected, and that it's hovered,
                // don't try to hover anything else
                return false;
            }

            if (_hoveredAIElem != null &&
                TryToHoverAIArea(_hoveredAIElem, hoveredTilePosition))
            {
                // If an element is already hovered,
                // don't try to hover anything else
                return false;
            }

            foreach (TrackAIElement trackAIElem in _track.AI)
            {
                if (TryToHoverAIArea(trackAIElem, hoveredTilePosition))
                {
                    return true;
                }
            }

            trackDisplay.Cursor = Cursors.Default;

            if (_hoveredAIElem == null)
            {
                return false;
            }

            _hoveredAIElem = null;
            return true;
        }

        private bool TryToHoverAIArea(TrackAIElement trackAIElem, Point hoveredTilePosition)
        {
            if (trackAIElem.IntersectsWith(hoveredTilePosition))
            {
                // Hover AI area
                _hoveredAIElem = trackAIElem;

                if (_hoveredAIElem != aiControl.SelectedElement)
                {
                    _aiAction = AIAction.DragArea;
                    trackDisplay.Cursor = Cursors.SizeAll;
                    SetAIElementAnchorPoint(hoveredTilePosition);
                }
                else
                {
                    _resizeHandle = aiControl.SelectedElement.GetResizeHandle(hoveredTilePosition);

                    if (_resizeHandle == ResizeHandle.None)
                    {
                        _aiAction = AIAction.DragArea;
                        trackDisplay.Cursor = Cursors.SizeAll;
                        SetAIElementAnchorPoint(hoveredTilePosition);
                    }
                    else
                    {
                        _aiAction = AIAction.ResizeArea;

                        switch (_resizeHandle)
                        {
                            case ResizeHandle.TopLeft:
                                trackDisplay.Cursor = Cursors.SizeNWSE;
                                break;

                            case ResizeHandle.Top:
                                trackDisplay.Cursor = Cursors.SizeNS;
                                break;

                            case ResizeHandle.TopRight:
                                trackDisplay.Cursor = Cursors.SizeNESW;
                                break;

                            case ResizeHandle.Right:
                                trackDisplay.Cursor = Cursors.SizeWE;
                                break;

                            case ResizeHandle.BottomRight:
                                trackDisplay.Cursor = Cursors.SizeNWSE;
                                break;

                            case ResizeHandle.Bottom:
                                trackDisplay.Cursor = Cursors.SizeNS;
                                break;

                            case ResizeHandle.BottomLeft:
                                trackDisplay.Cursor = Cursors.SizeNESW;
                                break;

                            case ResizeHandle.Left:
                                trackDisplay.Cursor = Cursors.SizeWE;
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
            _anchorPoint = new Point(hoveredTilePosition.X - _hoveredAIElem.Area.X,
                                         hoveredTilePosition.Y - _hoveredAIElem.Area.Y);
        }

        private void AddAIElement(Point location)
        {
            _track.AI.Add(new TrackAIElement(location));
        }

        private void DeleteAIElement()
        {
            _track.AI.Remove(aiControl.SelectedElement);
        }

        private void CloneAIElement()
        {
            TrackAIElement aiElement = aiControl.SelectedElement;
            TrackAIElement newAIElem = aiElement.Clone();

            // Shift the cloned element position, so it's not directly over the source element
            newAIElem.Location = new Point(aiElement.Location.X + TrackAIElement.Precision,
                                           aiElement.Location.Y + TrackAIElement.Precision);

            // Ensure the cloned element index is right after the source element
            int newAIElementIndex = _track.AI.GetElementIndex(aiElement) + 1;

            _track.AI.Insert(newAIElem, newAIElementIndex);
        }

        private void AIControlAddElementRequested(object sender, EventArgs e)
        {
            AddAIElement(AbsoluteCenterTileLocation);
        }
        #endregion EditionMode.AI
    }

    internal enum EditionMode
    {
        Tileset,
        Overlay,
        Start,
        Objects,
        AI
    }
}