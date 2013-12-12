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
        //the path of the project
        String projectPath;
        //the current data being worked on
        Slice activeSlice;
        //the treenode containing the active data
        TreeNode currentTreeNode;
        //indicates whether data is being selected
        bool isSelecting;
        //indicates when the project has changed
        bool projectChanged;
        //the current position in the data
        long currentPosition;
        //generates a unique ID
        public int uniqueID { get { return idIndex++; } }
        //the unique ID
        private int idIndex;
        //searches for data
        FindForm search;
        //the character indicating the file/slice has changes
        static char[] changed = new char[] { '*' };
        //indicates if the size of the file has changed
        bool lengthChanged;

        public PIEForm()
        {
            InitializeComponent();
            projectTreeView.TreeViewNodeSorter = new ProjectTreeSort();
            //if a configuration file exists, load settings from it
            if (File.Exists(Application.StartupPath + Properties.Resources.configFileString))
                loadSettings();
        }

        //enables/disables a bunch of controls relating to cut/copy/paste, delete and slicing
        //enableValue is the value the controls are set to
        private void toggleEnable(bool toggle)
        {
            isSelecting = toggle;
            hexContextMenuStrip.Items["sliceHexToolStripMenuItem"].Enabled = toggle;
            sliceToolStripButton.Enabled = toggle;
            //change enable of copy and cut
            hexContextMenuStrip.Items["cutHexToolStripMenuItem"].Enabled = toggle;
            hexContextMenuStrip.Items["copyHexToolStripMenuItem"].Enabled = toggle;
            hexContextMenuStrip.Items["deleteHexToolStripMenuItem"].Enabled = toggle;
            standardToolStrip.Items["cutToolStripButton"].Enabled = toggle;
            standardToolStrip.Items["copyToolStripButton"].Enabled = toggle;
            editToolStripMenuItem.DropDownItems["cutToolStripMenuItem"].Enabled = toggle;
            editToolStripMenuItem.DropDownItems["copyToolStripMenuItem"].Enabled = toggle;
            editToolStripMenuItem.DropDownItems["deleteToolStripMenuItem"].Enabled = toggle;
            if (displayHexBox.CanPaste())
                togglePaste(true);
        }

        //closes the file associated with the project
        //if the file was successfully closed, it returns true
        //if the action was canceled, it returns falses
        private bool closeFile()
        {
            if (anyChanges(projectTreeView.Nodes[0]))
            {
                DialogResult result = MessageBox.Show("Apply changes to file before closing?", "PIE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    saveAllChanges();
                    saveFile(true);
                    displayHexBox.ByteProvider = null;
                    fileBytes.Dispose();
                }
                else if (result == DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        //closes the project file
        //returns true if the project was successfully closed
        //returns false if the action was canceled
        private bool closeProject()
        {
            if (projectChanged)
            {
                DialogResult result = MessageBox.Show("Apply changes to project before closing?", "PIE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                    return saveProject();
                else if (result == DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        //checks recursively to see if any slices have unsaved changes
        //returns true if there are unsaved changes
        //returns false if unchanged

        private bool anyChanges(TreeNode current)
        {
            foreach (TreeNode t in current.Nodes)
            {
                if (!anyChanges(t))
                    continue;
                else
                    return true;
            }
            return (current.Tag as Slice).isChanged;
        }

        private void copy()
        {
            activeSlice.Copy();
        }

        private void cut()
        {
            activeSlice.Cut();
        }

        //displays a slice

        private void displaySlice()
        {
            //if this is not the first node to be selected, clear the loaded slice and load the new one
            if (projectTreeView.SelectedNode != null)
            {
                if (activeSlice != null)
                {
                    activeSlice.dataByteProvider.Changed -= dataByteProvider_Changed;
                    activeSlice.Hide();
                }
                currentTreeNode = projectTreeView.SelectedNode;
                activeSlice = currentTreeNode.Tag as Slice;
            }
            if (activeSlice.display == null)
                activeSlice.display = displayHexBox;
            activeSlice.Display();
            activeSlice.FillAddresses(startAddrToolStripComboBox);
            activeSlice.dataByteProvider.Changed += new EventHandler(dataByteProvider_Changed);
            //show the slice name and the data range
            sliceToolStripStatusLabel.Text = currentTreeNode.Text + ":  " + activeSlice.start.ToString("X") + " - " + activeSlice.end.ToString("X");
            //if the current slice is not the entire file, enable import/export
            fileToolStripMenuItem.DropDownItems["exportToolStripMenuItem"].Enabled =
                fileToolStripMenuItem.DropDownItems["importToolStripMenuItem"].Enabled = !(currentTreeNode.Parent == null);
            toggleEnable(false);
            updatePosition();
        }

        void dataByteProvider_Changed(object sender, EventArgs e)
        {
            if (!currentTreeNode.Text.EndsWith("*"))
                currentTreeNode.Text += "*";
        }

        //when a project is opened/closed, enable/disable all related controls
        //the listed controls are set to toggle
        private void toggleControls(bool toggle)
        {
            editToolStripMenuItem.DropDownItems["findToolStripMenuItem"].Enabled = toggle;
            editToolStripMenuItem.DropDownItems["selectAllToolStripMenuItem"].Enabled = toggle;
            editToolStripMenuItem.DropDownItems["findNextToolStripMenuItem"].Enabled = toggle;
            fileToolStripMenuItem.DropDownItems["saveToolStripMenuItem"].Enabled = toggle;
            fileToolStripMenuItem.DropDownItems["saveProjectToolStripMenuItem"].Enabled = toggle;
            fileToolStripMenuItem.DropDownItems["reloadToolStripMenuItem"].Enabled = toggle;
            fileToolStripMenuItem.DropDownItems["saveAllToolStripMenuItem"].Enabled = toggle;
            fileToolStripMenuItem.DropDownItems["closeProjectToolStripMenuItem"].Enabled = toggle;
            standardToolStrip.Items["saveToolStripButton"].Enabled = toggle;
            hexContextMenuStrip.Items["selectAllHexToolStripMenuItem"].Enabled = toggle;
            startAddrToolStripComboBox.Enabled = toggle;
            gotoToolStripTextBox.Enabled = toggle;
        }

        //enables/disables pasting
        //value set by toggle value
        private void togglePaste(bool toggle)
        {
            editToolStripMenuItem.DropDownItems["pasteToolStripMenuItem"].Enabled = toggle;
            editToolStripMenuItem.DropDownItems["pasteOverToolStripMenuItem"].Enabled = toggle;
            hexContextMenuStrip.Items["pasteHexToolStripMenuItem"].Enabled = toggle;
            hexContextMenuStrip.Items["pasteOverHexToolStripMenuItem"].Enabled = toggle;
            standardToolStrip.Items["pasteToolStripButton"].Enabled = toggle;
            standardToolStrip.Items["pasteOverToolStripButton"].Enabled = toggle;
        }

        //initializes projecTreeview with an initial node and sets activeSlice and currentTreeNode
        private void initializeProjectTree(string fileName)
        {
            TreeNode rootNode;
            Slice rootData;

            rootNode = new TreeNode(fileName);
            rootNode.Name = uniqueID.ToString();
            rootData = new Slice(fileBytes);
            rootNode.Tag = rootData;
            if (projectTreeView.Nodes.Count > 0)
                projectTreeView.Nodes.Clear();
            projectTreeView.Nodes.Add(rootNode);
            activeSlice = rootData;
            currentTreeNode = rootNode;
        }

        //loads settings from file
        private void loadSettings()
        {
            FontConverter fc = new FontConverter();
            ColorConverter cc = new ColorConverter();
            string line;

            try
            {
                //currently, settings are order dependent.  should consider making them order independent
                using (StreamReader settingsFile = new StreamReader(Application.StartupPath + Properties.Resources.configFileString))
                {
                    settingsFile.ReadLine();
                    line = settingsFile.ReadLine();
                    displayHexBox.ForeColor = (Color) cc.ConvertFromString(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.Font = (Font)fc.ConvertFromString(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.BackColor = (Color)cc.ConvertFromString(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.StringViewVisible = bool.Parse(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.LineInfoVisible = bool.Parse(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.HexCasing = (HexCasing)Enum.Parse(typeof(HexCasing), line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.BytesPerLine = int.Parse(line.Substring(line.IndexOf('=') + 1));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //opens the file related to the project
        //newProject determines whether the project exists or is being created
        private void openFile(bool newProject)
        {
            string fileName;

            //if a project is already open and the user chooses not to close it, cancel the open
            if (fileBytes != null && !closeFile())
                return;

            if (newProject)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                else
                    filePath = openFileDialog1.FileName;
            }

            try
            {
                if (File.Exists(filePath))
                {
                    if (fileBytes != null)
                        fileBytes.Dispose();
                    fileBytes = new DynamicFileByteProvider(filePath);
                    fileBytes.LengthChanged += new EventHandler(fileBytes_LengthChanged);
                    fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (newProject)
                        initializeProjectTree(fileName);
                    else if (projectTreeView.Nodes.Count > 0)
                        projectTreeView.Nodes.Clear();
                    hexContextMenuStrip.Enabled = true;
                    displayHexBox.ByteProvider = fileBytes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: could not open file: " + ex.Message);
                return;
            }
        }

        //opens a project file
        private void openProject()
        {
            Slice whole;
            TreeNode root;
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.CloseInput = true;
            rs.IgnoreComments = true;
            rs.IgnoreWhitespace = true;

            try
            {
                using (XmlReader xr = XmlReader.Create(projectPath, rs))
                {
                    xr.ReadToFollowing("PIEForm");
                    xr.Read();
                    //get filePath
                    filePath = xr.ReadElementContentAsString();

                    //if the file is not where it should be, check the project file directory
                    if (!File.Exists(filePath))
                    {
                        string tempPath = Path.GetDirectoryName(openFileDialog1.FileName) + "\\" + Path.GetFileName(filePath);
                        if (!File.Exists(tempPath))
                            throw new FileNotFoundException("Unable to locate " + filePath);
                        else
                            filePath = tempPath;
                    }
                    
                    openFile(false);

                    //initialize idIndex
                    idIndex = int.Parse(xr.ReadElementContentAsString());
                    xr.ReadToFollowing("Node");
                    xr.Read();
                    //initialize the main slice
                    root = new TreeNode(xr.ReadElementContentAsString());
                    root.Text = xr.ReadElementContentAsString();
                    whole = Slice.Deserialize(xr);
                    whole.display = displayHexBox;
                    whole.updateMainSlice(fileBytes);
                    root.Tag = whole;
                    projectTreeView.Nodes.Add(root);
                    activeSlice = whole;
                    currentTreeNode = root;
                    //if the root has children, initialize them
                    if (xr.IsStartElement())
                        loadNodes(root, xr);
                    this.Text = "PIE - " + Path.GetFileNameWithoutExtension(projectPath);
                    displaySlice();
                    toggleControls(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //recursively initializes and adds nodes into current using the passed in XmlReader
        private void loadNodes(TreeNode current, XmlReader xr)
        {
            Slice slice;
            TreeNode node;
            //load all children
            do
            {
                //load the TreeNode information
                xr.Read();
                node = new TreeNode(xr.ReadElementContentAsString());
                node.Text = xr.ReadElementContentAsString();
                slice = Slice.Deserialize(xr);
                node.Tag = new Slice(slice, current.Tag as Slice);
                current.Nodes.Add(node);
                //if this TreeNode has children, recurse
                if (xr.IsStartElement())
                    loadNodes(node, xr);
            } while (xr.ReadToNextSibling("Node"));
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
                toggleEnable(displayHexBox.CanCopy());
                updatePosition();
        }

        private void sliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sliceData();
            showProjectChanged();
        }

        //creates a new slice from the selected bytes
        private void sliceData()
        {
            long start = displayHexBox.SelectionStart;
            long size = displayHexBox.SelectionLength;
            Slice previous;
            Slice view;
            TreeNode slice;
            previous = currentTreeNode.Tag as Slice;
            view = new Slice(previous, start, size);
            if (Slice.IsTaken(currentTreeNode, view))
            {
                MessageBox.Show("Unable to create slice: " + Properties.Resources.overlapString);
                return;
            }
            slice = new TreeNode();
            slice.Name = uniqueID.ToString();
            //the initial name is the data range
            slice.Text = (start + activeSlice.lastStart).ToString("X") + "-" + (start + size + activeSlice.lastStart - 1).ToString("X");
            slice.Tag = view;
            currentTreeNode.Nodes.Add(slice);

            if (!currentTreeNode.IsExpanded)
                currentTreeNode.Expand();

        }

        //adds an asterisk to the end of the project name to show unsaved changes
        private void showProjectChanged()
        {
            projectChanged = true;
            if (!this.Text.EndsWith("*"))
                this.Text += "*";
        }

        private void displayHexBox_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            updatePosition();
        }

        //shows the current cursor position for displayHexBox
        //if the user is selecting bytes, shows the range of bytes
        private void updatePosition()
        {
            if (activeSlice != null)
            {
                currentPosition = (displayHexBox.CurrentLine - 1) * displayHexBox.BytesPerLine + (displayHexBox.CurrentPositionInLine - 1) + activeSlice.lastStart;
                statusStrip.Items["positionToolStripStatusLabel"].Text = currentPosition.ToString("X");
                if (isSelecting)
                    statusStrip.Items["positionToolStripStatusLabel"].Text += "-" + (currentPosition + displayHexBox.SelectionLength - 1).ToString("X");
            }
        }

        private void displayHexBox_CurrentLineChanged(object sender, EventArgs e)
        {
            updatePosition();
        }

        private void paste()
        {
            activeSlice.Paste();
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
            //if a project is already opened and the user doesn't close it, return
            if (fileBytes != null && !(closeFile() && closeProject()))
                return;
            openFileDialog1.Title = "Select a Project";
            openFileDialog1.Filter = "PIE files (*.pie)|*.pie|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                return;
            projectPath = openFileDialog1.FileName;
            openProject();
            if (fileBytes != null)
                this.Text = "PIE - " + Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
        }

        //saves the files
        //isClosing indicates whether the file is being closed
        private void saveFile(bool isClosing)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                //if the file length has changed, update any slices
                if (lengthChanged)
                    (projectTreeView.Nodes[0].Tag as Slice).Resize(projectTreeView.Nodes[0], 0, fileBytes.Length - 1);
                saveChanges(isClosing);
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
            {
                if (!(closeFile() && closeProject()))
                    e.Cancel = true;
            }
        }

        //searches the file
        private void performSearch()
        {
            search = new FindForm(displayHexBox);
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

        //saves the changes for the current slice
        //if the file isn't being closed, update any sub-slices
        private void saveChanges(bool isClosing)
        {
            activeSlice.Save();
            if (!isClosing)
            {
                propagateSave(currentTreeNode);
                currentTreeNode.Text = currentTreeNode.Text.TrimEnd(changed);
            }
        }

        private void saveAllChanges()
        {
            saveAllChanges(projectTreeView.Nodes[0]);
        }

        //saves all changes
        private void saveAllChanges(TreeNode current)
        {
            foreach (TreeNode t in current.Nodes)
            {
                if (t.Nodes.Count > 0)
                    saveAllChanges(t);
                (t.Tag as Slice).Save(false);
                t.Text.TrimEnd(changed);
            }
            this.Text = this.Text.TrimEnd(changed);
        }

        //when reloading or saving, clears all sub-slices of data
        //when displayed again, the slice will reflect changes
        private void propagateSave(TreeNode current)
        {
            foreach (TreeNode n in current.Nodes)
            {
                if (n.Nodes.Count > 0)
                    propagateSave(n);
                (n.Tag as Slice).Invalidate();
                n.Text = n.Text.TrimEnd(changed);
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displaySlice();
        }

        private void deleteSliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteSlice();
        }

        //deletes a slice from projectTreeView
        private void deleteSlice()
        {
            //if the slice is not the entire file
            if (projectTreeView.SelectedNode != projectTreeView.Nodes[0])
            {
                //if the slice is sliced and the user doesn't want to remove them all, cancel
                if (projectTreeView.SelectedNode.Nodes.Count > 0 &&
                    MessageBox.Show("All sub-slices will be deleted.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                        == DialogResult.Cancel)
                    return;
                if (projectTreeView.SelectedNode == currentTreeNode)
                {
                    activeSlice.Hide();
                    (projectTreeView.SelectedNode.Parent.Tag as Slice).Display();
                    currentTreeNode = projectTreeView.SelectedNode.Parent;
                    activeSlice = currentTreeNode.Tag as Slice;
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
            //if the slice is not the entire file, enable resizing and cloning
            if (e.Node.Parent == null)
            {
                sliceToolStripMenuItem.DropDownItems["resizeToolStripMenuItem1"].Enabled = false;
                sliceToolStripMenuItem.DropDownItems["cloneToolStripMenuItem1"].Enabled = false;
                projectContextMenuStrip.Items["resizeToolStripMenuItem"].Enabled = false;
                projectContextMenuStrip.Items["cloneToolStripMenuItem"].Enabled = false;
            }
            else
            {
                sliceToolStripMenuItem.DropDownItems["resizeToolStripMenuItem1"].Enabled = true;
                sliceToolStripMenuItem.DropDownItems["cloneToolStripMenuItem1"].Enabled = true;
                projectContextMenuStrip.Items["resizeToolStripMenuItem"].Enabled = true;
                projectContextMenuStrip.Items["cloneToolStripMenuItem"].Enabled = true;
            }
        }

        private void projectTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            displaySlice();
        }

        private void displayHexBox_Copied(object sender, EventArgs e)
        {
            togglePaste(true);
        }

        private void PIEForm_Activated(object sender, EventArgs e)
        {
            if (displayHexBox.CanPaste())
                togglePaste(true);
        }

        //reloads the file only
        private void reloadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileBytes.HasChanges() &&
                MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                fileBytes.Dispose();
                fileBytes = new DynamicFileByteProvider(filePath);
                (projectTreeView.Nodes[0].Tag as Slice).updateMainSlice(fileBytes);
                propagateSave(projectTreeView.Nodes[0]);
                projectTreeView.Nodes[0].Text = projectTreeView.Nodes[0].Text.TrimEnd(changed);
                activeSlice.Display();
            }
        }

        //overwrites the bytes instead of inserting them
        private void pasteOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject da = Clipboard.GetDataObject();
            long size = 0;
            long start = displayHexBox.SelectionLength > 0 ? displayHexBox.SelectionStart : currentPosition;

            //get the size of the data to paste
            //if it's copied hex:
            if (da.GetDataPresent("BinaryData"))
            {
                MemoryStream ms = (MemoryStream)da.GetData("BinaryData");
                size = ms.Length;
            }
            //if it's copied text:
            else if (da.GetDataPresent(typeof(string)))
            {
                string buffer = (string)da.GetData(typeof(string));
                size = buffer.Length;
            }

            //if there is data to paste, delete the bytes and insert the new ones
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
            if (sliceForm.changed)
                showProjectChanged();
        }

        private void projectTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (projectTreeView.SelectedNode != null)
            {
                if (e.KeyCode == Keys.Enter)
                    displaySlice();
                else if (e.KeyCode == Keys.Delete)
                    deleteSlice();
            }
        }

        private void startAddrToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            long offset = long.Parse(startAddrToolStripComboBox.SelectedItem as string, NumberStyles.HexNumber);
            activeSlice.ChangeOffset(offset);
            updateSliceInfo();
            updatePosition();
        }

        private void startAddrToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                long potentialAddress;
                try
                {
                    potentialAddress = long.Parse(startAddrToolStripComboBox.Text, NumberStyles.HexNumber);
                    activeSlice.ChangeOffset(potentialAddress);
                    if (!startAddrToolStripComboBox.Items.Contains(startAddrToolStripComboBox.Text))
                        startAddrToolStripComboBox.Items.Add(startAddrToolStripComboBox.Text);
                    updateSliceInfo();
                    updatePosition();
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
                    if (gotoAddress > displayHexBox.LineInfoOffset + activeSlice.size)
                        gotoAddress = activeSlice.size - 1;
                    displayHexBox.ScrollByteIntoView(gotoAddress);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    startAddrToolStripComboBox.Text = "";
                }
            }
        }

        /* Deprecated
         * checks the Data object passed in against all nodes of the same tier
         *if the new Data object's data range is found overlapping with any existing objects,
         *it returns false.  Otherwise the method returns true.
         */
        public static bool dataIsTaken(TreeNode toSlice, Slice toCheck)
        {
            return dataIsTaken(toSlice, toCheck.start, toCheck.end);
        }

        public static bool dataIsTaken(TreeNode beingSliced, long start, long end)
        {
            TreeNodeCollection currentTier = beingSliced.Nodes;
            Slice currentData;

            foreach (TreeNode d in currentTier)
            {
                currentData = d.Tag as Slice;

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
            if (resizeForm.changed)
                showProjectChanged();
            if (resizeForm.node == currentTreeNode)
            {
                (currentTreeNode.Tag as Slice).Display();
                activeSlice.FillAddresses(startAddrToolStripComboBox);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activeSlice.Delete();
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

        //saves the project
        //returns true if the project was saved
        //returns false if it was not
        private bool saveProject()
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.NewLineOnAttributes = true;
            writerSettings.CloseOutput = true;

            try
            {
                saveFileDialog1.InitialDirectory = Path.GetDirectoryName(filePath);
                saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(filePath) + ".pie";

                if (!File.Exists(projectPath))
                {
                    if (saveFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                        return false;
                    else
                        projectPath = saveFileDialog1.FileName;
                }

                using (XmlWriter projectFile = XmlWriter.Create(projectPath, writerSettings))
                {
                    projectFile.WriteStartDocument();
                    projectFile.WriteStartElement("PIEProject");
                        projectFile.WriteStartElement("PIEForm");
                            projectFile.WriteElementString("filePath", filePath);
                            projectFile.WriteElementString("idIndex", idIndex.ToString());
                        projectFile.WriteEndElement();
                        saveSlices(projectTreeView.Nodes[0], projectFile);
                    projectFile.WriteEndElement();
                    projectFile.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            
            return true;
        }

        //saves project slices
        private void saveSlices(TreeNode current, XmlWriter writer)
        {
            writer.WriteStartElement("Node");
                writer.WriteElementString("Name", current.Name);
                writer.WriteElementString("Text", current.Text);
                (current.Tag as Slice).Serialize(writer);
                foreach (TreeNode t in current.Nodes)
                    saveSlices(t, writer);
            writer.WriteEndElement();
        }

        //reloads the current slice. this could be the entire file
        private void reloadSliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedTreeNode = projectTreeView.SelectedNode;
            Slice selectedData = selectedTreeNode.Tag as Slice;
            if (selectedData.dataByteProvider.HasChanges() &&
                MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            if (selectedTreeNode.Parent == null)
            {
                fileBytes.Dispose();
                fileBytes = new DynamicFileByteProvider(filePath);
                selectedData.updateMainSlice(fileBytes);
                displayHexBox.ByteProvider = fileBytes;
            }
            else
                selectedData.Invalidate();
            if (selectedTreeNode == currentTreeNode)
                selectedData.Display();
            selectedTreeNode.Text = selectedTreeNode.Text.TrimEnd(changed);
        }

        private void projectTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            updateSliceInfo();
            showProjectChanged();
        }

        private void updateSliceInfo()
        {
            sliceToolStripStatusLabel.Text = currentTreeNode.Text + ":  " + (activeSlice.start + activeSlice.lastStart).ToString("X") +
                " - " + (activeSlice.end + activeSlice.lastStart).ToString("X");
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloneForm cloneForm = new CloneForm(projectTreeView.SelectedNode);
            cloneForm.Show(this);
            if (cloneForm.changed)
                showProjectChanged();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (saveProject())
                this.Text.TrimEnd(changed);
            this.Cursor = Cursors.Arrow;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select a Project File";
            openFileDialog1.Filter = "";
            openFile(true);
            if (fileBytes != null)
            {
                this.Text = "PIE - new project";
                displaySlice();
                showProjectChanged();
                toggleControls(true);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PIEAboutBox about = new PIEAboutBox();
            about.Show(this);
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(closeFile() && closeProject()))
                return;
            displayHexBox.ByteProvider = null;
            fileBytes = null;
            projectTreeView.Nodes.Clear();
            this.Text = "PIE";
            toggleControls(false);
            togglePaste(false);
            sliceToolStripMenuItem.Enabled = false;
            sliceToolStripStatusLabel.Text = "";
            positionToolStripStatusLabel.Text = "";
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportSlice();
        }

        //exports a slice
        private void exportSlice()
        {
            saveFileDialog1.FileName = currentTreeNode.Text;
            saveFileDialog1.InitialDirectory = filePath;
            saveFileDialog1.Filter = "bin | *.bin | All files | *.*";

            if (saveFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                return;
            try
            {
                activeSlice.Export(saveFileDialog1.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importSlice();
        }

        //imports a slice
        private void importSlice()
        {
            openFileDialog1.InitialDirectory = filePath;
            openFileDialog1.Filter = "";

            if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                return;
            try
            {
                if (File.Exists(openFileDialog1.FileName))
                {
                    activeSlice.Import(openFileDialog1.FileName);
                    displayHexBox.ByteProvider = activeSlice.dataByteProvider;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotesForm infoForm = new NotesForm(projectTreeView.SelectedNode.Tag as Slice);
            infoForm.Show(this);
        }
    }

    //sorts the nodes in projectTreeView by the start value of the contained slice
    public class ProjectTreeSort : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            Slice a = (x as TreeNode).Tag as Slice;
            Slice b = (y as TreeNode).Tag as Slice;
            if (a.start < b.start)
                return -1;
            else if (a.start > b.start)
                return 1;
            else
                return 0;
        }
    }
}
