using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public partial class PIEForm : Form
    {
        //the opened file
        DynamicFileByteProvider fileBytes;
        //the filepath of the opened file
        String filePath;
        //the current data being worked on
        Data activeData;
        //the treenode containing the active data
        TreeNode currentTreeNode;
        //indicates whether data is being selected
        bool isSelecting;
        //the current position in the data
        long currentPosition;
        //generates a unique ID
        public int uniqueID { get { return idIndex++; } }
        //the unique ID
        private int idIndex;
        //searches for data
        FindForm search;
        //the name of the opened file
        string currentFileName;

        public PIEForm()
        {
            InitializeComponent();
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
            if (displayHexBox.CanPaste())
                enablePaste();
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

        private void copy()
        {
            activeData.Copy();
            //enablePaste();
        }

        private void cut()
        {
            activeData.Cut();
            //enablePaste();
        }

        private void displayData()
        {
            if (activeData != null && currentTreeNode.Tag != activeData)
                activeData.Hide();
            currentTreeNode = projectTreeView.SelectedNode;
            activeData = currentTreeNode.Tag as Data;
            activeData.Display(displayPanel.Controls);
            activeData.fillAddresses(startAddrToolStripComboBox);
        }

        private void enableItems()
        {
            editToolStripMenuItem.DropDownItems["findToolStripMenuItem"].Enabled = true;
            editToolStripMenuItem.DropDownItems["selectAllToolStripMenuItem"].Enabled = true;
            editToolStripMenuItem.DropDownItems["findNextToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["saveToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["saveAsToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["reloadToolStripMenuItem"].Enabled = true;
            standardToolStrip.Items["saveToolStripButton"].Enabled = true;
            hexContextMenuStrip.Items["selectAllHexToolStripMenuItem"].Enabled = true;
            startAddrToolStripComboBox.Enabled = true;
            gotoToolStripTextBox.Enabled = true;
        }

        private void enablePaste()
        {
            editToolStripMenuItem.DropDownItems["pasteToolStripMenuItem"].Enabled = true;
            hexContextMenuStrip.Items["pasteHexToolStripMenuItem"].Enabled = true;
            standardToolStrip.Items["pasteToolStripButton"].Enabled = true;
            standardToolStrip.Items["pasteOverToolStripButton"].Enabled = true;
            editToolStripMenuItem.DropDownItems["pasteOverToolStripMenuItem"].Enabled = true;
        }

        private void initializeProjectTree(string fileName)
        {
            TreeNode rootNode;
            Data rootData;

            rootNode = new TreeNode(fileName);
            rootNode.Name = uniqueID.ToString();
            rootData = new Data(fileBytes);
            rootNode.Tag = rootData;
            if (projectTreeView.Nodes.Count > 0)
                projectTreeView.Nodes.Clear();
            projectTreeView.Nodes.Add(rootNode);
            activeData = rootData;
            currentTreeNode = rootNode;
        }

        private void openFile()
        {

            //if a project is already open and the user chooses not to close it, cancel the open
            if (fileBytes != null && !closeProject())
                return;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                String fileName;

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
                initializeProjectTree(fileName);
                activeData.fillAddresses(startAddrToolStripComboBox);
                hexContextMenuStrip.Enabled = true;
                activeData.Display(displayPanel.Controls);
                displayHexBox.ByteProvider = fileBytes;
                enableItems();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void displayHexBox_SelectionLengthChanged(object sender, EventArgs e)
        {
                changeEnable(displayHexBox.CanCopy());
                updatePosition();
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
            slice = new TreeNode();
            slice.Name = uniqueID.ToString();
            slice.Text = start.ToString("X") + "-" + (start + size).ToString("X");
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

        private void PIEForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fileBytes != null)
                e.Cancel = !closeProject();
        }

        private void performSearch()
        {
            search = new FindForm(displayHexBox);
            search.Location = this.DesktopLocation;
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
                if (n.Nodes.Count > 0)
                    saveChanges(n);
                (n.Tag as Data).save();
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
            deleteNode();
        }

        private void deleteNode()
        {
            if (projectTreeView.SelectedNode != projectTreeView.Nodes[0])
            {
                //if the node has sub-nodes and the user doesn't want to remove them all, cancel
                if (projectTreeView.SelectedNode.Nodes.Count > 0 &&
                    MessageBox.Show("All sub-slices will be deleted.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                        == DialogResult.Cancel)
                    return;
                if (projectTreeView.SelectedNode == currentTreeNode)
                {
                    activeData.Hide();
                    (projectTreeView.SelectedNode.Parent.Tag as Data).Display(displayPanel.Controls);
                    currentTreeNode = projectTreeView.SelectedNode.Parent;
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

        private void displayHexBox_Copied(object sender, EventArgs e)
        {
            enablePaste();
        }

        private void PIEForm_Activated(object sender, EventArgs e)
        {
            if (displayHexBox.CanPaste())
                enablePaste();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileBytes.HasChanges() && MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                fileBytes.Dispose();
                fileBytes = new DynamicFileByteProvider(filePath);
                initializeProjectTree(Path.GetFileNameWithoutExtension(filePath));
                displayHexBox.ByteProvider = fileBytes;
            }
        }

        private void pasteOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject da = Clipboard.GetDataObject();
            long size = 0;
            long start = displayHexBox.SelectionLength > 0 ? displayHexBox.SelectionStart : currentPosition;

            if (da.GetDataPresent("BinaryData"))
            {
                MemoryStream ms = (MemoryStream)da.GetData("BinaryData");
                size = ms.Length;
            }
            else if (da.GetDataPresent(typeof(string)))
            {
                string buffer = (string)da.GetData(typeof(string));
                size = buffer.Length;
            }

            if (size > 0)
            {
                fileBytes.DeleteBytes(start, size);
                paste();
            }
        }

        private void sliceToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SliceForm sliceForm = new SliceForm(projectTreeView.SelectedNode);
            sliceForm.Owner = this;
            sliceForm.Location = this.DesktopLocation;
            sliceForm.Show();
        }

        //the following drag and drop code was taken from:
        //http://social.msdn.microsoft.com/Forums/windows/en-US/2654f7ac-b58f-477a-9b42-3d2afe43c6e8/moving-a-treenode-in-a-treeview
        //start

        private void projectTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            projectTreeView.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void projectTreeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            TreeNode source = (TreeNode)e.Data.GetData(typeof(TreeNode));
            Point location = projectTreeView.PointToClient(new Point(e.X, e.Y));
            TreeNode destination = projectTreeView.GetNodeAt(location);
            if (destination == null)
                return;
            for (TreeNode parent = destination.Parent; parent != null; parent = parent.Parent)
                if (parent == source)
                    return;
            destination.Expand();
            e.Effect = DragDropEffects.Move;    
        }

        private void projectTreeView_DragDrop(object sender, DragEventArgs e)
        {
            Point location = projectTreeView.PointToClient(new Point(e.X, e.Y));
            TreeNode destination = projectTreeView.GetNodeAt(location);
            TreeNode source = (TreeNode)e.Data.GetData(typeof(TreeNode));
            int prevIndex = source.Index;

            if (destination == source)
                return;
            //only reorganization within the same level will be allowed at present
            //might modify to so only root-level drops disallowed
            if (destination.Parent != source.Parent)
                return;
            if (source.Parent == null)
                source.TreeView.Nodes.Remove(source);
            else
                source.Parent.Nodes.Remove(source);
            if (destination.Parent == null)
            {
                if (prevIndex == 0 && destination.Index == 0)
                    destination.TreeView.Nodes.Insert(1, source);
                else
                    destination.TreeView.Nodes.Insert(destination.Index, source);
            }
            else
            {
                if (prevIndex == 0 && destination.Index == 0)
                    destination.Parent.Nodes.Insert(1, source);
                else
                    destination.Parent.Nodes.Insert(destination.Index, source);
            }
        }
        //end

        private void projectTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (projectTreeView.SelectedNode != null)
            {
                if (e.KeyCode == Keys.Enter)
                    displayData();
                else if (e.KeyCode == Keys.Delete)
                    deleteNode();
            }
        }

        private void startAddrToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayHexBox.LineInfoOffset = long.Parse(startAddrToolStripComboBox.SelectedItem as string, NumberStyles.HexNumber);
        }

        private void startAddrToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                long potentialAddress;
                try
                {
                    potentialAddress = long.Parse(startAddrToolStripComboBox.Text, NumberStyles.HexNumber);
                    if (activeData.customStart == 0)
                        startAddrToolStripComboBox.Items.Add(startAddrToolStripComboBox.Text);
                    activeData.customStart = potentialAddress;
                    displayHexBox.LineInfoOffset = potentialAddress;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    startAddrToolStripComboBox.Text = "";
                }
            }
        }

        private void gotoToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                long gotoAddress;
                try
                {
                    gotoAddress = long.Parse(gotoToolStripTextBox.Text, NumberStyles.HexNumber);
                    if (gotoAddress < displayHexBox.LineInfoOffset)
                        gotoAddress = displayHexBox.LineInfoOffset;
                    if (gotoAddress > displayHexBox.LineInfoOffset + activeData.size)
                        gotoAddress = activeData.size - 1;
                    displayHexBox.ScrollByteIntoView(gotoAddress);
                }
                catch (Exception ex)
                {
                    gotoToolStripTextBox.Text = ex.Message;
                }
            }
        }
    }
}
