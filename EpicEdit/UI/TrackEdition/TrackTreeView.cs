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

        private Dictionary<TrackGroup, TreeNode> trackGroupDictionary;
        private Dictionary<Track, TreeNode> trackDictionary;

        /// <summary>
        /// The selected track.
        /// </summary>
        private Track selectedTrack;

        /// <summary>
        /// The track being dragged in the track list (for reordering).
        /// </summary>
        private TreeNode draggedTrack;

        /// <summary>
        /// The target track of the <see cref="draggedTrack"/>.
        /// </summary>
        private TreeNode draggedTrackTarget;

        public TrackTreeView()
        {
            this.InitializeComponent();
        }

        public void InitOnFirstRomLoad()
        {
            this.trackGroupDictionary = new Dictionary<TrackGroup, TreeNode>();
            this.trackDictionary = new Dictionary<Track, TreeNode>();
            this.InitOnRomLoad();

            // Attach the AfterSelect event handler method here
            // to avoid an extra repaint on ROM loading
            this.SetSelectedTrack();
            this.treeView.AfterSelect += this.TreeViewAfterSelect;
        }

        public void InitOnRomLoad()
        {
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();

            foreach (TrackGroup trackGroup in Context.Game.TrackGroups)
            {
                TreeNode trackGroupNode = new TreeNode(trackGroup.Name);
                trackGroupNode.ForeColor = SystemColors.WindowText;

                // Makes it so group nodes don't appear highlighted when clicked
                trackGroupNode.BackColor = this.treeView.BackColor;

                foreach (Track track in trackGroup)
                {
                    TreeNode trackNode = new TreeNode();
                    trackGroupNode.Nodes.Add(trackNode);
                    track.PropertyChanged += this.track_PropertyChanged;
                }

                this.treeView.Nodes.Add(trackGroupNode);
                trackGroup.PropertyChanged += this.trackGroup_PropertyChanged;
            }

            this.UpdateTrackNames();

            this.treeView.ExpandAll();
            this.treeView.SelectedNode = this.treeView.Nodes[0].Nodes[0];
            this.treeView.EndUpdate();
        }

        private void UpdateTrackNames()
        {
            this.trackGroupDictionary.Clear();
            this.trackDictionary.Clear();

            for (int i = 0; i < Context.Game.TrackGroups.Count; i++)
            {
                TrackGroup trackGroup = Context.Game.TrackGroups[i];
                TreeNode trackGroupNode = this.treeView.Nodes[i];
                this.trackGroupDictionary.Add(trackGroup, trackGroupNode);

                TreeNodeCollection trackNodes = trackGroupNode.Nodes;

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    Track track = trackGroup[j];
                    TreeNode trackNode = trackNodes[j];

                    this.trackDictionary.Add(track, trackNode);
                    trackNode.Text = TrackTreeView.GetTrackText(track);
                }
            }
        }

        private void trackGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is TrackGroup trackGroup))
            {
                return;
            }

            TreeNode treeNode = this.trackGroupDictionary[trackGroup];
            treeNode.Text = trackGroup.Name;
        }

        private void track_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Track track = sender as Track;
            TreeNode treeNode = this.trackDictionary[track];
            string trackText = TrackTreeView.GetTrackText(track);

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
            return track.Name + (!track.Modified ? null : "*"); ;
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
            this.SetSelectedTrack();
            this.SelectedTrackChanged(this, EventArgs.Empty);
        }

        private void SetSelectedTrack()
        {
            int trackGroupId = this.treeView.SelectedNode.Parent.Index;
            int trackId = this.treeView.SelectedNode.Index;
            this.selectedTrack = Context.Game.TrackGroups[trackGroupId][trackId];
        }

        public Track SelectedTrack => this.selectedTrack;

        public string SelectedTrackFileName => this.SelectedTrackId + "- " + this.selectedTrack.Name;

        public int SelectedTrackId => this.treeView.SelectedNode.Parent.Index * GPTrack.CountPerGroup + this.treeView.SelectedNode.Index + 1;

        #region Track reordering
        private void TreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode hoveredNode = (TreeNode)e.Item;

            if (e.Button != MouseButtons.Left || hoveredNode.Level == 0)
            {
                return;
            }

            this.draggedTrack = this.draggedTrackTarget = hoveredNode;
            this.treeView.SelectedNode = this.draggedTrack;
            this.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void TreeViewDragEnter(object sender, DragEventArgs e)
        {
            // For the drag and drop operation to be valid,
            // make sure the dragged object is a TreeNode, and that it's coming from this very control
            if (!e.Data.GetDataPresent(typeof(TreeNode)) ||
                ((TreeNode)e.Data.GetData(typeof(TreeNode))).TreeView != this.treeView)
            {
                this.draggedTrack = this.draggedTrackTarget = null;
            }
        }

        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            Point targetPoint = this.PointToClient(new Point(e.X, e.Y));
            TreeNode hoveredNode = this.treeView.GetNodeAt(targetPoint);

            if (hoveredNode.Level == 0)
            {
                this.ClearPreviousHighlight();
                e.Effect = DragDropEffects.None;
                return;
            }

            TreeNode nodeGroup = hoveredNode.Parent;

            if ((nodeGroup.Index < GPTrack.GroupCount && this.draggedTrack.Parent.Index < GPTrack.GroupCount) ||
                (nodeGroup.Index == GPTrack.GroupCount && this.draggedTrack.Parent.Index == GPTrack.GroupCount))
            {
                e.Effect = DragDropEffects.Move;

                if (hoveredNode.BackColor != SystemColors.Highlight)
                {
                    this.ClearPreviousHighlight();
                    this.draggedTrackTarget = hoveredNode;
                    this.HighlightNode();
                }
            }
            else
            {
                this.ClearPreviousHighlight();
                e.Effect = DragDropEffects.None;
            }
        }

        private void TreeViewDragDrop(object sender, DragEventArgs e)
        {
            this.ClearPreviousHighlight();

            int sourceTrackGroupId = this.draggedTrack.Parent.Index;
            int sourceTrackId = this.draggedTrack.Index;

            int destinationTrackGroupId = this.draggedTrackTarget.Parent.Index;
            int destinationTrackId = this.draggedTrackTarget.Index;

            Context.Game.ReorderTracks(sourceTrackGroupId, sourceTrackId, destinationTrackGroupId, destinationTrackId);

            this.treeView.BeginUpdate();
            this.UpdateTrackNames();
            this.treeView.EndUpdate();

            this.treeView.AfterSelect -= this.TreeViewAfterSelect;
            this.treeView.SelectedNode = this.treeView.Nodes[destinationTrackGroupId].Nodes[destinationTrackId];
            this.treeView.AfterSelect += this.TreeViewAfterSelect;

            this.draggedTrack = this.draggedTrackTarget = null;
        }

        private void HighlightNode()
        {
            this.draggedTrackTarget.ForeColor = SystemColors.HighlightText;
            this.draggedTrackTarget.BackColor = SystemColors.Highlight;
        }

        private void ClearPreviousHighlight()
        {
            this.draggedTrackTarget.ForeColor = Color.Empty;
            this.draggedTrackTarget.BackColor = Color.Empty;
        }
        #endregion Track reordering
    }
}
