using System;
using System.IO;
using System.Windows.Forms;

namespace PIE
{
    class PIESliceManager
    {
        private TreeView ProjectTreeView;
        private PIEInfo PieInfo;

        public PIESliceManager(PIEInfo pieInfo)
        {
            PieInfo = pieInfo;
            ProjectTreeView = pieInfo.ProjectTreeView;
        }

        //creates a new slice from the selected bytes
        public void CreateSlice(long start, long size)
        {
            var currentTreeNode = PieInfo.CurrentTreeNode;
            var activeSlice = PieInfo.ActiveSlice;
            Slice previous = currentTreeNode.Tag as Slice;
            Slice view = new Slice(previous, start, size);

            if (Slice.IsTaken(currentTreeNode, view))
            {
                MessageBox.Show("Unable to create slice: " + Properties.Resources.overlapString);
                return;
            }
            TreeNode slice = new TreeNode();
            slice.Name = PieInfo.UniqueID.ToString();
            //the initial name is the data range
            slice.Text = string.Format("{0}-{1}", (start + activeSlice.Offset).ToString("X"),
                (start + size + activeSlice.Offset - 1).ToString("X"));
            slice.Tag = view;
            currentTreeNode.Nodes.Add(slice);

            if (!currentTreeNode.IsExpanded)
                currentTreeNode.Expand();

            PieInfo.ProjectChanged = true;
        }


        public void ExportSlice()
        {
            SaveFileDialog ExportSliceSaveFileDialog = new SaveFileDialog();
            ExportSliceSaveFileDialog.FileName = PieInfo.CurrentTreeNode.Text;
            ExportSliceSaveFileDialog.InitialDirectory = PieInfo.FilePath;
            ExportSliceSaveFileDialog.Filter = "bin | *.bin | All files | *.*";

            if (ExportSliceSaveFileDialog.ShowDialog(PieInfo.PieForm) == DialogResult.Cancel)
                return;
            try
            {
                PieInfo.ActiveSlice.Export(ExportSliceSaveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //imports a slice
        public void ImportSlice()
        {
            OpenFileDialog ImportSliceOpenFileDialog = new OpenFileDialog();
            ImportSliceOpenFileDialog.InitialDirectory = PieInfo.FilePath;
            ImportSliceOpenFileDialog.Filter = "";

            if (ImportSliceOpenFileDialog.ShowDialog(PieInfo.PieForm) == DialogResult.Cancel)
                return;
            try
            {
                var activeSlice = PieInfo.ActiveSlice;
                if (File.Exists(ImportSliceOpenFileDialog.FileName))
                {
                    activeSlice.Import(ImportSliceOpenFileDialog.FileName);
                    PieInfo.DisplayHexBox.ByteProvider = activeSlice.Data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteSlice(TreeNode selectionStart, TreeNode selectionEnd)
        {
            if (selectionEnd == selectionStart)
                deleteSlice(true);
            else
                deleteSlices(selectionStart, selectionEnd);
        }

        private void deleteSlice(bool singleSliceSelected)
        {
            //if the slice is not the entire file
            if (ProjectTreeView.SelectedNode != ProjectTreeView.Nodes[0])
            {
                if (singleSliceSelected)
                {
                    //if the slice is sliced and the user doesn't want to remove them all, cancel
                    if (ProjectTreeView.SelectedNode.Nodes.Count > 0 &&
                        MessageBox.Show("All sub-slices will be deleted.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                            == DialogResult.Cancel)
                        return;
                }
                else if (ProjectTreeView.SelectedNode.Nodes.Count > 0)
                {
                    var confirm = new YesNoAllForm("All sub-slices will be deleted.", "Warning");
                    if (confirm.Result == yesNoAllResult.NoAll)
                        return;
                    else if (confirm.Result != yesNoAllResult.YesAll)
                    {
                        confirm.ShowDialog(PieInfo.PieForm);
                        if (confirm.Result == yesNoAllResult.No || confirm.Result == yesNoAllResult.NoAll)
                            return;
                    }
                }
                if (ProjectTreeView.SelectedNode == PieInfo.CurrentTreeNode)
                {
                    var controller = PieInfo.PieForm.ViewController;
                    controller.Hide();
                    controller.Model = ProjectTreeView.SelectedNode.Parent.Tag as Slice;
                    controller.Display();
                    PieInfo.SetActiveSlice();
                }
                ProjectTreeView.SelectedNode.Remove();
                PieInfo.ProjectChanged = true;
            }
            else
                MessageBox.Show("Cannot delete base", "PIE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void deleteSlices(TreeNode selectionStart, TreeNode selectionEnd)
        {
            int start = Math.Min(selectionStart.Index, selectionEnd.Index);
            int end = Math.Max(selectionStart.Index, selectionEnd.Index);
            TreeNodeCollection nodes = selectionStart.Parent.Nodes;
            TreeNode parent = selectionStart.Parent;

            for (int i = start; i <= end; ++i)
            {
                ProjectTreeView.SelectedNode = nodes[i];
                deleteSlice(false);
            }

            if (nodes.Count > start)
                ProjectTreeView.SelectedNode = nodes[start];
            else if (nodes.Count > 0)
                ProjectTreeView.SelectedNode = nodes[nodes.Count - 1];
            else
                ProjectTreeView.SelectedNode = parent;

            selectionStart = selectionEnd = ProjectTreeView.SelectedNode;
            clearSelection(selectionStart);
        }

        private void clearSelection(TreeNode selectionStart)
        {
            if (selectionStart.Parent == null)
            {
                return;
            }
            foreach (TreeNode node in selectionStart.Parent.Nodes)
            {
                node.ForeColor = ProjectTreeView.ForeColor;
                node.BackColor = ProjectTreeView.BackColor;
            }
        }

        public void ReloadSlice()
        {
            TreeNode selectedTreeNode = ProjectTreeView.SelectedNode;
            Slice selectedData = selectedTreeNode.Tag as Slice;
            if (selectedData.Data.HasChanges() &&
                MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            if (selectedTreeNode.Parent == null)
            {
                PieInfo.ReloadFileBytes();
                selectedData.SetMainSlice(PieInfo.FileBytes);
                PieInfo.DisplayHexBox.ByteProvider = PieInfo.FileBytes;
            }
            else
            {
                selectedData.Invalidate();
            }
            if (selectedTreeNode == PieInfo.CurrentTreeNode)
            {
                var controller = PieInfo.PieForm.ViewController;
                controller.Model = selectedData;
                controller.Display();
            }
            selectedTreeNode.Text = selectedTreeNode.Text.TrimEnd(PIEForm.Changed);
        }

        public static bool AnySlicesChanged(TreeNode current)
        {
            foreach (TreeNode t in current.Nodes)
            {
                if (!AnySlicesChanged(t))
                    continue;
                else
                    return true;
            }
            return (current.Tag as Slice).IsChanged;
        }

        public void SaveAllSlices()
        {
            saveAllSlices(ProjectTreeView.Nodes[0]);
        }

        private static void saveAllSlices(TreeNode current)
        {
            foreach (TreeNode t in current.Nodes)
            {
                if (t.Nodes.Count > 0)
                    saveAllSlices(t);
                (t.Tag as Slice).Save(false);
                t.Text = t.Text.TrimEnd(PIEForm.Changed);
            }
        }

        public void SaveChangedSlices(bool isClosing)
        {
            var currentTreeNode = PieInfo.CurrentTreeNode;
            PieInfo.ActiveSlice.Save();
            if (!isClosing)
            {
                PropagateSliceChanges(currentTreeNode);
                currentTreeNode.Text = currentTreeNode.Text.TrimEnd(PIEForm.Changed);
            }
        }

        public static void PropagateSliceChanges(TreeNode current)
        {
            foreach (TreeNode n in current.Nodes)
            {
                if (n.Nodes.Count > 0)
                    PropagateSliceChanges(n);
                (n.Tag as Slice).Invalidate();
                n.Text = n.Text.TrimEnd(PIEForm.Changed);
            }
        }

        public void DisplaySlice()
        {
            var controller = PieInfo.PieForm.ViewController;
            var activeSlice = PieInfo.ActiveSlice;
            //if this is not the first node to be selected, clear the loaded slice and load the new one
            if (ProjectTreeView.SelectedNode != null)
            {
                if (activeSlice != null)
                {
                    activeSlice.Data.Changed -= dataByteProvider_Changed;
                    controller.Hide();
                }
                PieInfo.SetActiveSlice();
                activeSlice = PieInfo.ActiveSlice;
            }
            controller.Model = activeSlice;
            controller.Display();
            activeSlice.Data.Changed += new EventHandler(dataByteProvider_Changed);
        }

        void dataByteProvider_Changed(object sender, EventArgs e)
        {
            var currentTreeNode = PieInfo.CurrentTreeNode;
            if (!currentTreeNode.Text.EndsWith("*"))
                currentTreeNode.Text += "*";
        }

        public void MergeSlices(TreeNode selectionStart, TreeNode selectionEnd)
        {
            int start = Math.Min(selectionStart.Index, selectionEnd.Index);
            int end = Math.Max(selectionStart.Index, selectionEnd.Index);
            TreeNode mergedNode = new TreeNode(PieInfo.UniqueID.ToString("X"));
            TreeNode parent = selectionStart.Parent;
            TreeNode temp;
            TreeNodeCollection toMerge = selectionStart.Parent.Nodes;
            long sliceStart = (toMerge[start].Tag as Slice).Start;
            long size = (toMerge[end].Tag as Slice).End + 1 - sliceStart;

            Slice mergedSlice = new Slice(selectionStart.Parent.Tag as Slice, sliceStart, size);
            mergedNode.Tag = mergedSlice;
            mergedNode.Text = toMerge[start].Text + " - " + toMerge[end].Text;

            for (int i = start; i <= end; ++i)
            {
                temp = toMerge[start];
                (temp.Tag as Slice).Merge(mergedSlice, sliceStart);
                toMerge.Remove(temp);
                mergedNode.Nodes.Add(temp);
            }

            parent.Nodes.Add(mergedNode);
            ProjectTreeView.SelectedNode = mergedNode;
            PieInfo.ProjectChanged = true;
        }

        public void SplitSlice(TreeNode selectionStart, TreeNode selectionEnd)
        {
            TreeNode toSplit = ProjectTreeView.SelectedNode;
            TreeNode parent = toSplit.Parent;
            TreeNode temp;
            toSplit.Remove();

            while (toSplit.Nodes.Count > 0)
            {
                temp = toSplit.Nodes[0];
                //adjust parent, start, and end for each slice
                (temp.Tag as Slice).Split();
                toSplit.Nodes.Remove(temp);
                parent.Nodes.Add(temp);
            }

            ProjectTreeView.SelectedNode = selectionStart;
            selectionEnd = selectionStart;
            PieInfo.ProjectChanged = true;
        }

    }
}
