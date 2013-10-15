using System;
using System.Collections;
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
using System.Xml;
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
        //the character indicating the file/slice has changes
        static char[] changed = new char[] { '*' };
        //indicates if the size of the file has changed
        bool lengthChanged;

        public PIEForm()
        {
            InitializeComponent();
            projectTreeView.TreeViewNodeSorter = new ProjectTreeSort();
        }

        void changeEnable(bool enableValue)
        {
            isSelecting = enableValue;
            hexContextMenuStrip.Items["sliceHexToolStripMenuItem"].Enabled = enableValue;
            //change enable of copy and cut
            hexContextMenuStrip.Items["cutHexToolStripMenuItem"].Enabled = enableValue;
            hexContextMenuStrip.Items["copyHexToolStripMenuItem"].Enabled = enableValue;
            hexContextMenuStrip.Items["deleteHexToolStripMenuItem"].Enabled = enableValue;
            standardToolStrip.Items["cutToolStripButton"].Enabled = enableValue;
            standardToolStrip.Items["copyToolStripButton"].Enabled = enableValue;
            editToolStripMenuItem.DropDownItems["cutToolStripMenuItem"].Enabled = enableValue;
            editToolStripMenuItem.DropDownItems["copyToolStripMenuItem"].Enabled = enableValue;
            editToolStripMenuItem.DropDownItems["deleteToolStripMenuItem"].Enabled = enableValue;
            if (displayHexBox.CanPaste())
                enablePaste();
        }

        private bool closeProject()
        {
            if (anyChanges(projectTreeView.Nodes[0]))
            {
                DialogResult result = MessageBox.Show("Apply changes before closing?", "PIE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    saveAllChanges();
                    saveFile(true);
                    displayHexBox.ByteProvider = null;
                    fileBytes.Dispose();
                }
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        private bool anyChanges(TreeNode current)
        {
            foreach (TreeNode t in current.Nodes)
            {
                if (!anyChanges(t))
                    continue;
                else
                    return true;
            }
            return (current.Tag as Data).isChanged;
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
            if (projectTreeView.SelectedNode != null)
            {
                if (activeData != null)
                {
                    activeData.dataByteProvider.Changed -= dataByteProvider_Changed;
                    activeData.Hide();
                }
                currentTreeNode = projectTreeView.SelectedNode;
                activeData = currentTreeNode.Tag as Data;
            }
            if (activeData.display == null)
                activeData.display = displayHexBox;
            activeData.Display();
            activeData.FillAddresses(startAddrToolStripComboBox);
            activeData.dataByteProvider.Changed += new EventHandler(dataByteProvider_Changed);
            sliceToolStripStatusLabel.Text = currentTreeNode.Text + ":  " + activeData.start.ToString("X") + " - " + activeData.end.ToString("X");
        }

        void dataByteProvider_Changed(object sender, EventArgs e)
        {
            if (!currentTreeNode.Text.EndsWith("*"))
                currentTreeNode.Text += "*";
        }

        private void enableItems()
        {
            editToolStripMenuItem.DropDownItems["findToolStripMenuItem"].Enabled = true;
            editToolStripMenuItem.DropDownItems["selectAllToolStripMenuItem"].Enabled = true;
            editToolStripMenuItem.DropDownItems["findNextToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["saveToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["saveAsToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["reloadToolStripMenuItem"].Enabled = true;
            fileToolStripMenuItem.DropDownItems["saveAllToolStripMenuItem"].Enabled = true;
            standardToolStrip.Items["saveToolStripButton"].Enabled = true;
            hexContextMenuStrip.Items["selectAllHexToolStripMenuItem"].Enabled = true;
            startAddrToolStripComboBox.Enabled = true;
            gotoToolStripTextBox.Enabled = true;
        }

        private void enablePaste()
        {
            editToolStripMenuItem.DropDownItems["pasteToolStripMenuItem"].Enabled = true;
            editToolStripMenuItem.DropDownItems["pasteOverToolStripMenuItem"].Enabled = true;
            hexContextMenuStrip.Items["pasteHexToolStripMenuItem"].Enabled = true;
            hexContextMenuStrip.Items["pasteOverHexToolStripMenuItem"].Enabled = true;
            standardToolStrip.Items["pasteToolStripButton"].Enabled = true;
            standardToolStrip.Items["pasteOverToolStripButton"].Enabled = true;
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

                fileBytes.LengthChanged += new EventHandler(fileBytes_LengthChanged);
                fileBytes.Changed += new EventHandler(fileBytes_Changed);
                fileName = Path.GetFileNameWithoutExtension(filePath);
                this.Text = "PIE - " + fileName;
                initializeProjectTree(fileName);
                hexContextMenuStrip.Enabled = true;
                displayHexBox.ByteProvider = fileBytes;
                enableItems();
                displayData();
            }
        }

        void fileBytes_Changed(object sender, EventArgs e)
        {
            if (!this.Text.EndsWith("*"))
                this.Text += "*";
        }

        void fileBytes_LengthChanged(object sender, EventArgs e)
        {
            lengthChanged = true;
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
            previous = currentTreeNode.Tag as Data;
            view = new Data(previous, start, size);
            if (Data.IsTaken(currentTreeNode, view))
            {
                MessageBox.Show("Unable to create slice: " + Properties.Resources.overlapString);
                return;
            }
            slice = new TreeNode();
            slice.Name = uniqueID.ToString();
            slice.Text = start.ToString("X") + "-" + (start + size - 1).ToString("X");
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
            currentPosition = (displayHexBox.CurrentLine - 1) * displayHexBox.BytesPerLine + (displayHexBox.CurrentPositionInLine - 1);
            statusStrip.Items["positionToolStripStatusLabel"].Text = currentPosition.ToString("X");
            if (isSelecting)
                statusStrip.Items["positionToolStripStatusLabel"].Text += "-" + (currentPosition + displayHexBox.SelectionLength - 1).ToString("X");
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
            if (currentTreeNode.Parent == null)
                saveFile(false);
            else
                saveChanges(false);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void saveFile(bool isClosing)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (lengthChanged)
                    (projectTreeView.Nodes[0].Tag as Data).Resize(projectTreeView.Nodes[0], 0, fileBytes.Length - 1);
                saveChanges(isClosing);
                this.Cursor = Cursors.Arrow;
                this.Text = this.Text.TrimEnd(changed);
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
            search.Owner = this;
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

        private void saveChanges(bool isClosing)
        {
            activeData.Save();
            if (!isClosing)
            {
                bool hadChanges = fileBytes.HasChanges();
                propagateDown(currentTreeNode);
                currentTreeNode.Text = currentTreeNode.Text.TrimEnd(changed);
                if (!hadChanges)
                    this.Text = this.Text.TrimEnd(changed);
            }
        }

        private void saveAllChanges()
        {
            saveAllChanges(projectTreeView.Nodes[0]);
        }

        private void saveAllChanges(TreeNode current)
        {
            foreach (TreeNode t in current.Nodes)
            {
                if (t.Nodes.Count > 0)
                    saveAllChanges(t);
                (t.Tag as Data).Save(false);
                t.Text.TrimEnd(changed);
            }
            this.Text = this.Text.TrimEnd(changed);
        }

        private void propagateDown(TreeNode current)
        {
            foreach (TreeNode n in current.Nodes)
            {
                if (n.Nodes.Count > 0)
                    propagateDown(n);
                (n.Tag as Data).invalidate();
                n.Text = n.Text.TrimEnd(changed);
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayData();
        }

        private void deleteSliceToolStripMenuItem_Click(object sender, EventArgs e)
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
                    (projectTreeView.SelectedNode.Parent.Tag as Data).Display();
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
            sliceToolStripMenuItem.Enabled = true;
            if (e.Node.Parent == null)
                projectContextMenuStrip.Items["resizeToolStripMenuItem"].Enabled = false;
            else
                projectContextMenuStrip.Items["resizeToolStripMenuItem"].Enabled = true;
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
            if (activeData.dataByteProvider.HasChanges() &&
                MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (currentTreeNode.Parent == null)
                {
                    fileBytes.Dispose();
                    fileBytes = new DynamicFileByteProvider(filePath);
                    activeData.updateMainSlice(fileBytes);
                    displayHexBox.ByteProvider = fileBytes;
                    this.Text = this.Text.TrimEnd(changed);
                }
                else
                {
                    activeData.invalidate();
                    activeData.Display();
                }
                currentTreeNode.Text = currentTreeNode.Text.TrimEnd(changed);
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
            sliceForm.Show(this);
        }

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
            long offset = long.Parse(startAddrToolStripComboBox.SelectedItem as string, NumberStyles.HexNumber);
            activeData.ChangeOffset(offset);
        }

        private void startAddrToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                long potentialAddress;
                try
                {
                    potentialAddress = long.Parse(startAddrToolStripComboBox.Text, NumberStyles.HexNumber);
                    activeData.ChangeOffset(potentialAddress);
                    if (!startAddrToolStripComboBox.Items.Contains(startAddrToolStripComboBox.Text))
                        startAddrToolStripComboBox.Items.Add(startAddrToolStripComboBox.Text);
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
                    //if (currentPosition + displayHexBox.VerticalByteCount * displayHexBox.BytesPerLine < displayHexBox.ByteProvider.Length - 1)
                    //    displayHexBox.ScrollByteIntoView(gotoAddress + displayHexBox.VerticalByteCount * displayHexBox.BytesPerLine);
                    //else
                    //    displayHexBox.ScrollByteIntoView(displayHexBox.ByteProvider.Length - 1);
                    //displayHexBox.Select(gotoAddress, 1);
                }
                catch (Exception ex)
                {
                    gotoToolStripTextBox.Text = ex.Message;
                }
            }
        }

        /*checks the Data object passed in against all nodes of the same tier
         *if the new Data object's data range is found overlapping with any existing objects,
         *it returns false.  Otherwise the method returns true.
         */
        public static bool dataIsTaken(TreeNode toSlice, Data toCheck)
        {
            return dataIsTaken(toSlice, toCheck.start, toCheck.end);
        }

        public static bool dataIsTaken(TreeNode beingSliced, long start, long end)
        {
            TreeNodeCollection currentTier = beingSliced.Nodes;
            Data currentData;

            foreach (TreeNode d in currentTier)
            {
                currentData = d.Tag as Data;

                //if they don't overlap, continue
                if (end < currentData.start || start > currentData.end)
                    continue;
                else
                    return true;
            }
            return false;
        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResizeForm resizeForm = new ResizeForm(projectTreeView.SelectedNode);
            resizeForm.Show(this);
            if (projectTreeView.SelectedNode == currentTreeNode)
            {
                (currentTreeNode.Tag as Data).Display();
                activeData.FillAddresses(startAddrToolStripComboBox);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activeData.Delete();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(displayHexBox);
            optionForm.Show(this);
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAllChanges();
        }

        private void saveProject()
        {
            //open a new xmltextwriter "[file name].pie"
            //start an attribute ("PIEForm")
            //save filePath, idIndex, currentFileName and end attribute
            //start attribute (projectTreeView.Nodes[0].Name)
            //for each node, save the name and text (recursive call in a different function)
            //serialize the Tag as Data
            //end attribute
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TreeNode selectedTreeNode = projectTreeView.SelectedNode;
            Data selectedData = selectedTreeNode.Tag as Data;
            if (selectedData.dataByteProvider.HasChanges() &&
                MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            if (selectedTreeNode.Parent == null)
            {
                fileBytes.Dispose();
                fileBytes = new DynamicFileByteProvider(filePath);
                selectedData.updateMainSlice(fileBytes);
                displayHexBox.ByteProvider = fileBytes;
                this.Text = this.Text.TrimEnd(changed);
            }
            else
                selectedData.invalidate();
            if (selectedTreeNode == currentTreeNode)
                selectedData.Display();
            selectedTreeNode.Text = selectedTreeNode.Text.TrimEnd(changed);
        }

        private void projectTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            sliceToolStripStatusLabel.Text = currentTreeNode.Text + ":  " + activeData.start.ToString("X") + " - " + activeData.end.ToString("X");
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloneForm cloneForm = new CloneForm(projectTreeView.SelectedNode);
            cloneForm.Show(this);
        }
    }

    public class ProjectTreeSort : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            Data a = (x as TreeNode).Tag as Data;
            Data b = (y as TreeNode).Tag as Data;
            if (a.start < b.start)
                return -1;
            else if (a.start > b.start)
                return 1;
            else
                return 0;
        }
    }
}
