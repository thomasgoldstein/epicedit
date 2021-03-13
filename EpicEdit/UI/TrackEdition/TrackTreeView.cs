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

using EpicEdit.Rom.Tracks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Displays a hierarchical collection of tracks.
    /// </summary>
    internal partial class TrackTreeView : UserControl
    {
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> SelectedTrackChanged;

        private Dictionary<TrackGroup, TreeNode> _trackGroupDictionary;
        private Dictionary<Track, TreeNode> _trackDictionary;

        /// <summary>
        /// The track being dragged in the track list (for reordering).
        /// </summary>
        private TreeNode _draggedTrack;

        /// <summary>
        /// The target track of the <see cref="_draggedTrack"/>.
        /// </summary>
        private TreeNode _draggedTrackTarget;

        public TrackTreeView()
        {
            InitializeComponent();
        }

        public void InitOnFirstRomLoad()
        {
            _trackGroupDictionary = new Dictionary<TrackGroup, TreeNode>();
            _trackDictionary = new Dictionary<Track, TreeNode>();
            InitOnRomLoad();

            // Attach the AfterSelect event handler method here
            // to avoid an extra repaint on ROM loading
            SetSelectedTrack();
            treeView.AfterSelect += TreeViewAfterSelect;
        }

        public void InitOnRomLoad()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            foreach (TrackGroup trackGroup in Context.Game.TrackGroups)
            {
                TreeNode trackGroupNode = new TreeNode(trackGroup.Name);
                trackGroupNode.ForeColor = SystemColors.WindowText;

                // Makes it so group nodes don't appear highlighted when clicked
                trackGroupNode.BackColor = treeView.BackColor;

                foreach (Track track in trackGroup)
                {
                    TreeNode trackNode = new TreeNode();
                    trackGroupNode.Nodes.Add(trackNode);
                    track.PropertyChanged += track_PropertyChanged;
                }

                treeView.Nodes.Add(trackGroupNode);
                trackGroup.PropertyChanged += trackGroup_PropertyChanged;
            }

            UpdateTrackNames();

            treeView.ExpandAll();
            treeView.SelectedNode = treeView.Nodes[0].Nodes[0];
            treeView.EndUpdate();
        }

        private void UpdateTrackNames()
        {
            _trackGroupDictionary.Clear();
            _trackDictionary.Clear();

            for (int i = 0; i < Context.Game.TrackGroups.Count; i++)
            {
                TrackGroup trackGroup = Context.Game.TrackGroups[i];
                TreeNode trackGroupNode = treeView.Nodes[i];
                _trackGroupDictionary.Add(trackGroup, trackGroupNode);

                TreeNodeCollection trackNodes = trackGroupNode.Nodes;

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    Track track = trackGroup[j];
                    TreeNode trackNode = trackNodes[j];

                    _trackDictionary.Add(track, trackNode);
                    trackNode.Text = GetTrackText(track);
                }
            }
        }

        private void trackGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is TrackGroup trackGroup))
            {
                return;
            }

            TreeNode treeNode = _trackGroupDictionary[trackGroup];
            treeNode.Text = trackGroup.Name;
        }

        private void track_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Track track = sender as Track;
            TreeNode treeNode = _trackDictionary[track];
            string trackText = GetTrackText(track);

            if (treeNode.Text != trackText)
            {
                // NOTE: Supposedly a redundant condition check before assignment,
                // but without it, the TrackTreeView flickers (gets invalidated)
                // even when the text is the same as before.
                treeNode.Text = trackText;
            }
        }

        private static string GetTrackText(Track track)
        {
            return track.Name + (!track.Modified ? null : "*");
        }

        private void TreeViewBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void TreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Cancel = true;
            }
        }

        private void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            SetSelectedTrack();
            SelectedTrackChanged(this, EventArgs.Empty);
        }

        private void SetSelectedTrack()
        {
            int trackGroupId = treeView.SelectedNode.Parent.Index;
            int trackId = treeView.SelectedNode.Index;
            SelectedTrack = Context.Game.TrackGroups[trackGroupId][trackId];
        }

        /// <summary>
        /// The selected track.
        /// </summary>
        public Track SelectedTrack { get; private set; }

        public string SelectedTrackFileName => SelectedTrackId + "- " + SelectedTrack.Name;

        public int SelectedTrackId => treeView.SelectedNode.Parent.Index * GPTrack.CountPerGroup + treeView.SelectedNode.Index + 1;

        #region Track reordering
        private void TreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode hoveredNode = (TreeNode)e.Item;

            if (e.Button != MouseButtons.Left || hoveredNode.Level == 0)
            {
                return;
            }

            _draggedTrack = _draggedTrackTarget = hoveredNode;
            treeView.SelectedNode = _draggedTrack;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void TreeViewDragEnter(object sender, DragEventArgs e)
        {
            // For the drag and drop operation to be valid,
            // make sure the dragged object is a TreeNode, and that it's coming from this very control
            if (!e.Data.GetDataPresent(typeof(TreeNode)) ||
                ((TreeNode)e.Data.GetData(typeof(TreeNode))).TreeView != treeView)
            {
                _draggedTrack = _draggedTrackTarget = null;
            }
        }

        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            TreeNode hoveredNode = treeView.GetNodeAt(targetPoint);

            if (hoveredNode.Level == 0)
            {
                ClearPreviousHighlight();
                e.Effect = DragDropEffects.None;
                return;
            }

            TreeNode nodeGroup = hoveredNode.Parent;

            if ((nodeGroup.Index < GPTrack.GroupCount && _draggedTrack.Parent.Index < GPTrack.GroupCount) ||
                (nodeGroup.Index == GPTrack.GroupCount && _draggedTrack.Parent.Index == GPTrack.GroupCount))
            {
                e.Effect = DragDropEffects.Move;

                if (hoveredNode.BackColor != SystemColors.Highlight)
                {
                    ClearPreviousHighlight();
                    _draggedTrackTarget = hoveredNode;
                    HighlightNode();
                }
            }
            else
            {
                ClearPreviousHighlight();
                e.Effect = DragDropEffects.None;
            }
        }

        private void TreeViewDragDrop(object sender, DragEventArgs e)
        {
            ClearPreviousHighlight();

            int sourceTrackGroupId = _draggedTrack.Parent.Index;
            int sourceTrackId = _draggedTrack.Index;

            int destinationTrackGroupId = _draggedTrackTarget.Parent.Index;
            int destinationTrackId = _draggedTrackTarget.Index;

            Context.Game.ReorderTracks(sourceTrackGroupId, sourceTrackId, destinationTrackGroupId, destinationTrackId);

            treeView.BeginUpdate();
            UpdateTrackNames();
            treeView.EndUpdate();

            treeView.AfterSelect -= TreeViewAfterSelect;
            treeView.SelectedNode = treeView.Nodes[destinationTrackGroupId].Nodes[destinationTrackId];
            treeView.AfterSelect += TreeViewAfterSelect;

            _draggedTrack = _draggedTrackTarget = null;
        }

        private void HighlightNode()
        {
            _draggedTrackTarget.ForeColor = SystemColors.HighlightText;
            _draggedTrackTarget.BackColor = SystemColors.Highlight;
        }

        private void ClearPreviousHighlight()
        {
            _draggedTrackTarget.ForeColor = Color.Empty;
            _draggedTrackTarget.BackColor = Color.Empty;
        }
        #endregion Track reordering
    }
}
