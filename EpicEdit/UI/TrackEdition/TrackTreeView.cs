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
using System.Windows.Forms;

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Displays a hierarchical collection of tracks.
    /// </summary>
    internal partial class TrackTreeView : UserControl
    {
        [Browsable(true)]
        public event EventHandler<EventArgs> SelectedTrackChanged;

        private Dictionary<SuffixedTextItem, TreeNode> nodeDictionary;

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
            this.nodeDictionary = new Dictionary<SuffixedTextItem, TreeNode>();
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
                    track.SuffixedNameItem.PropertyChanged += this.SuffixedNameItem_PropertyChanged;
                }

                this.treeView.Nodes.Add(trackGroupNode);
                trackGroup.SuffixedNameItem.PropertyChanged += this.SuffixedNameItem_PropertyChanged;
            }

            this.UpdateTrackNames();

            this.treeView.ExpandAll();
            this.treeView.SelectedNode = this.treeView.Nodes[0].Nodes[0];
            this.treeView.EndUpdate();
        }

        private void UpdateTrackNames()
        {
            this.nodeDictionary.Clear();

            for (int i = 0; i < Context.Game.TrackGroups.Count; i++)
            {
                TrackGroup trackGroup = Context.Game.TrackGroups[i];
                TreeNode trackGroupNode = this.treeView.Nodes[i];
                this.nodeDictionary.Add(trackGroup.SuffixedNameItem, trackGroupNode);

                TreeNodeCollection trackNodes = trackGroupNode.Nodes;

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    Track track = trackGroup[j];
                    TreeNode trackNode = trackNodes[j];

                    this.nodeDictionary.Add(track.SuffixedNameItem, trackNode);
                    trackNode.Text = track.Name;

                    if (track.Modified)
                    {
                        TrackTreeView.AddModifiedHint(trackNode);
                    }
                }
            }
        }

        private void SuffixedNameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SuffixedTextItem textItem = sender as SuffixedTextItem;
            TreeNode treeNode = this.nodeDictionary[textItem];
            treeNode.Text = textItem.Value;
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

        public Track SelectedTrack
        {
            get { return this.selectedTrack; }
        }

        public string SelectedTrackFileName
        {
            get
            {
                int trackId = this.treeView.SelectedNode.Parent.Index * GPTrack.CountPerGroup + this.treeView.SelectedNode.Index + 1;
                string trackFileName = trackId + "- " + this.selectedTrack.Name;
                return trackFileName;
            }
        }

        public void MarkTrackAsChanged()
        {
            if (!this.selectedTrack.Modified)
            {
                this.selectedTrack.Modified = true;
                TrackTreeView.AddModifiedHint(this.treeView.SelectedNode);
            }
        }

        private static void AddModifiedHint(TreeNode node)
        {
            node.Text += "*";
        }

        public void RemoveModifiedHints()
        {
            this.treeView.BeginUpdate();

            foreach (TreeNode trackGroupNode in this.treeView.Nodes)
            {
                TreeNodeCollection trackNodes = trackGroupNode.Nodes;

                foreach (TreeNode trackNode in trackNodes)
                {
                    if (trackNode.Text.Length == 0)
                    {
                        continue;
                    }

                    int lastCharIndex = trackNode.Text.Length - 1;
                    if (trackNode.Text[lastCharIndex] == '*')
                    {
                        trackNode.Text = trackNode.Text.Substring(0, lastCharIndex);
                    }
                }
            }

            this.treeView.EndUpdate();
        }

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

            if ((nodeGroup.Index < 4 && this.draggedTrack.Parent.Index < 4) ||
                (nodeGroup.Index == 4 && this.draggedTrack.Parent.Index == 4))
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
