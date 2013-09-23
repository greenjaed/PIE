using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public partial class PIEForm : Form
    {
        DynamicFileByteProvider fileBytes;
        Data activeData;
        TreeNode currentTreeNode;
        bool isSelecting;
        long currentPosition;
        int idIndex;
        FindForm search;
        string currentFileName;

        public PIEForm()
        {
            InitializeComponent();
        }

        private void openFile()
        {

            //if a project is already open and the user chooses not to close it, cancel the open
            if (fileBytes != null && !closeProject())
                return;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String filePath = openFileDialog1.FileName;
                String fileName;
                TreeNode rootNode;
                Data rootData;

                try
                {
                    if (File.Exists(filePath))
                    {
                        if (currentFileName != null && currentFileName == filePath)
                            fileBytes.Dispose();
                        currentFileName = filePath;
                        fileBytes = new DynamicFileByteProvider(filePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: could not open file: " + ex.Message);
                }

                fileName = Path.GetFileNameWithoutExtension(filePath);
                this.Text = "PIE - " + fileName;
                rootNode = new TreeNode(idIndex.ToString());
                ++idIndex;
                rootNode.Text = fileName;
                rootData = new Data(fileBytes);
                rootData.display = displayHexBox;
                rootNode.Tag = rootData;
                if (projectTreeView.Nodes.Count > 0)
                    projectTreeView.Nodes.Clear();
                projectTreeView.Nodes.Add(rootNode);
                hexContextMenuStrip.Enabled = true;
                activeData = rootData;
                currentTreeNode = rootNode;
                displayHexBox.ByteProvider = fileBytes;
                enableItems();
            }
        }

        private void enableItems()
        {
            editToolStripMenuItem.DropDownItems["findToolStripMenuItem"].Enabled = true;
            editToolStripMenuItem.DropDownItems["selectAllToolStripMenuItem"].Enabled = true;
            editToolStripMenuItem.DropDownItems["findNextToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["saveToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["saveAsToolStripMenuItem"].Enabled = true;
            standardToolStrip.Items["saveToolStripButton"].Enabled = true;
            hexContextMenuStrip.Items["selectAllHexToolStripMenuItem"].Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool closeProject()
        {
            if (fileBytes.HasChanges())
            {
                DialogResult result = MessageBox.Show("Apply changes before closing?", "PIE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    saveFile();
                    displayHexBox.ByteProvider = null;
                    fileBytes.Dispose();
                }
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        private void displayHexBox_SelectionLengthChanged(object sender, EventArgs e)
        {
            if (displayHexBox.SelectionLength > 0)
            {
                changeEnable(true);
                updatePosition();
            }
            else
            {
                changeEnable(false);
                updatePosition();
            }

        }

        void changeEnable(bool enableValue)
        {
            isSelecting = enableValue;
            hexContextMenuStrip.Items["sliceHexToolStripMenuItem"].Enabled = enableValue;
            //change enable of copy and cut
            hexContextMenuStrip.Items["cutHexToolStripMenuItem"].Enabled = enableValue;
            hexContextMenuStrip.Items["copyHexToolStripMenuItem"].Enabled = enableValue;
            standardToolStrip.Items["cutToolStripButton"].Enabled = enableValue;
            standardToolStrip.Items["copyToolStripButton"].Enabled = enableValue;
            editToolStripMenuItem.DropDownItems["cutToolStripMenuItem"].Enabled = enableValue;
            editToolStripMenuItem.DropDownItems["copyToolStripMenuItem"].Enabled = enableValue;
        }

        private void sliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sliceData();
        }

        private void sliceData()
        {
            long start = displayHexBox.SelectionStart;
            long size = displayHexBox.SelectionLength;
            Data previous;
            Data view;
            TreeNode slice;
            previous = (currentTreeNode.Parent != null ? currentTreeNode.Parent.Tag : currentTreeNode.Tag) as Data;
            view = new Data(previous, start, size);
            slice = new TreeNode(idIndex.ToString());
            slice.Text = start.ToString("X") + "-" + (start + size).ToString("X");
            ++idIndex;
            slice.Tag = view;
            currentTreeNode.Nodes.Add(slice);

            if (!currentTreeNode.IsExpanded)
                currentTreeNode.Expand();

        }

        private void displayHexBox_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            updatePosition();
        }

        private void updatePosition()
        {
            currentPosition = displayHexBox.CurrentLine * 0x10 + displayHexBox.CurrentPositionInLine;
            statusStrip.Items["positionToolStripStatusLabel"].Text = currentPosition.ToString("X");
            if (isSelecting)
                statusStrip.Items["positionToolStripStatusLabel"].Text += "-" + (currentPosition + displayHexBox.SelectionLength).ToString("X");
        }

        private void displayHexBox_CurrentLineChanged(object sender, EventArgs e)
        {
            updatePosition();
        }

        private void displayData()
        {
            if (activeData != null && currentTreeNode.Tag != activeData)
                activeData.Hide();
            currentTreeNode = projectTreeView.SelectedNode;
            activeData = currentTreeNode.Tag as Data;
            activeData.Display(displayPanel.Controls);
        }

        private void cut()
        {
            activeData.Cut();
            enablePaste();
        }

        private void copy()
        {
            activeData.Copy();
            enablePaste();
        }

        private void enablePaste()
        {
            editToolStripMenuItem.DropDownItems["pasteToolStripMenuItem"].Enabled = true;
            hexContextMenuStrip.Items["pasteHexToolStripMenuItem"].Enabled = true;
            standardToolStrip.Items["pasteToolStripButton"].Enabled = true;
            standardToolStrip.Items["pasteOverToolStripButton"].Enabled = true;
            editToolStripMenuItem.DropDownItems["pasteOverToolStripMenuItem"].Enabled = true;
        }

        private void paste()
        {
            activeData.Paste();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cut();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paste();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void saveFile()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                saveChanges();
                fileBytes.ApplyChanges();
                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not save changes.  Original message: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fileBytes != null)
                e.Cancel = !closeProject();
        }

        private void performSearch()
        {
            search = new FindForm();
            search.Location = this.Location;
            search.searchMedium = displayHexBox;
            search.Show(this);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            performSearch();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (search == null)
                performSearch();
            else
                search.search();
        }

        private void saveChanges()
        {
            saveChanges(projectTreeView.Nodes[0]);
        }

        private void saveChanges(TreeNode current)
        {
            foreach (TreeNode n in current.Nodes)
            {
                Data toSave;
                if (n.Nodes.Count > 0)
                    saveChanges(n);
                toSave = n.Tag as Data;
                toSave.save();
            }
        }

        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hexFontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                displayHexBox.Font = hexFontDialog.Font;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayData();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectTreeView.SelectedNode != projectTreeView.Nodes[0])
            {
                //if the node has sub-nodes and the user doesn't want to remove them all, cancel
                if (projectTreeView.SelectedNode.Nodes.Count > 0 &&
                    MessageBox.Show("All sub-slices will be deleted.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                        == System.Windows.Forms.DialogResult.Cancel)
                    return;
                if (projectTreeView.SelectedNode == currentTreeNode)
                {
                    Data toShow;
                    toShow = projectTreeView.SelectedNode.Parent.Tag as Data;
                    activeData.Hide();
                    toShow.Display(displayPanel.Controls);
                }

                projectTreeView.SelectedNode.Remove();
            }
            else
                MessageBox.Show("Cannot delete base", "PIE", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayHexBox.SelectAll();
        }

        private void projectTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            projectTreeView.SelectedNode = e.Node;
            if (!projectContextMenuStrip.Enabled)
            {
                projectContextMenuStrip.Enabled = true;
                foreach (ToolStripItem t in projectContextMenuStrip.Items)
                    t.Enabled = true;
            }
        }

        private void projectTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            displayData();
        }

    }
}
