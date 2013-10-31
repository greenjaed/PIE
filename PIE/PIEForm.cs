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
        Slice activeData;
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
        //the character indicating the file/slice has changes
        static char[] changed = new char[] { '*' };
        //indicates if the size of the file has changed
        bool lengthChanged;

        public PIEForm()
        {
            InitializeComponent();
            projectTreeView.TreeViewNodeSorter = new ProjectTreeSort();
            if (File.Exists(Application.StartupPath + Properties.Resources.configFileString))
                loadSettings();
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

        private bool closeFile()
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
            return (current.Tag as Slice).isChanged;
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
                activeData = currentTreeNode.Tag as Slice;
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
            fileToolStripMenuItem.DropDownItems["saveProjectToolStripMenuItem"].Enabled = true;
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
            Slice rootData;

            rootNode = new TreeNode(fileName);
            rootNode.Name = uniqueID.ToString();
            rootData = new Slice(fileBytes);
            rootNode.Tag = rootData;
            if (projectTreeView.Nodes.Count > 0)
                projectTreeView.Nodes.Clear();
            projectTreeView.Nodes.Add(rootNode);
            activeData = rootData;
            currentTreeNode = rootNode;
        }

        private void loadSettings()
        {
            FontConverter fc = new FontConverter();
            ColorConverter cc = new ColorConverter();
            string line;

            try
            {
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

        private void openFile()
        {

            //if a project is already open and the user chooses not to close it, cancel the open
            if (fileBytes != null && !closeFile())
                return;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                String fileName;

                try
                {
                    if (File.Exists(filePath))
                    {
                        if (fileBytes != null)
                            fileBytes.Dispose();
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
                using (XmlReader pr = XmlReader.Create(openFileDialog1.FileName, rs))
                {
                    pr.ReadToFollowing("PIEForm");
                    pr.Read();
                    filePath = pr.ReadElementContentAsString();
                    fileBytes = new DynamicFileByteProvider(filePath);
                    //pr.Read();
                    idIndex = int.Parse(pr.ReadElementContentAsString());
                    pr.ReadToFollowing("Node");
                    pr.Read();
                    root = new TreeNode(pr.ReadElementContentAsString());
                    root.Text = pr.ReadElementContentAsString();
                    whole = Slice.Deserialize(pr);
                    whole.display = displayHexBox;
                    whole.updateMainSlice(fileBytes);
                    root.Tag = whole;
                    projectTreeView.Nodes.Add(root);
                    if (pr.ReadToDescendant("Node"))
                        loadNodes(root, pr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadNodes(TreeNode current, XmlReader xr)
        {

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
            openFileDialog1.Title = "Select a Project";
            openFileDialog1.Filter = "PIE files (*.pie)|*.pie|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                return;
            openProject();

        }

        private void saveFile(bool isClosing)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if (lengthChanged)
                    (projectTreeView.Nodes[0].Tag as Slice).Resize(projectTreeView.Nodes[0], 0, fileBytes.Length - 1);
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
                e.Cancel = !closeFile();
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
                (t.Tag as Slice).Save(false);
                t.Text.TrimEnd(changed);
            }
            this.Text = this.Text.TrimEnd(changed);
        }

        //when reloading or saving, clears all sub slices of data
        private void propagateDown(TreeNode current)
        {
            foreach (TreeNode n in current.Nodes)
            {
                if (n.Nodes.Count > 0)
                    propagateDown(n);
                (n.Tag as Slice).Invalidate();
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
                    (projectTreeView.SelectedNode.Parent.Tag as Slice).Display();
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

        //reloads the file only
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileBytes.HasChanges() &&
                MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                fileBytes.Dispose();
                fileBytes = new DynamicFileByteProvider(filePath);
                (projectTreeView.Nodes[0].Tag as Slice).updateMainSlice(fileBytes);
                propagateDown(projectTreeView.Nodes[0]);
                projectTreeView.Nodes[0].Text = projectTreeView.Nodes[0].Text.TrimEnd(changed);
                this.Text = this.Text.TrimEnd(changed);
                activeData.Display();
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
            if (projectTreeView.SelectedNode == currentTreeNode)
            {
                (currentTreeNode.Tag as Slice).Display();
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
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.NewLineOnAttributes = true;
            writerSettings.CloseOutput = true;

            try
            {
                saveFileDialog1.InitialDirectory = Path.GetDirectoryName(filePath);
                saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(filePath) + ".pie";
                if (saveFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                    return;

                using (XmlWriter projectFile = XmlWriter.Create(saveFileDialog1.FileName, writerSettings))
                {
                    projectFile.WriteStartDocument();
                    projectFile.WriteStartElement("PIEProject");
                        projectFile.WriteStartElement("PIEForm");
                            projectFile.WriteElementString("filePath", filePath);
                            projectFile.WriteElementString("idIndex", idIndex.ToString());
                        projectFile.WriteEndElement();
                        saveNodes(projectTreeView.Nodes[0], projectFile);
                    projectFile.WriteEndElement();
                    projectFile.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void saveNodes(TreeNode current, XmlWriter writer)
        {
            writer.WriteStartElement("Node");
                writer.WriteElementString("Name", current.Name);
                writer.WriteElementString("Text", current.Text);
                (current.Tag as Slice).Serialize(writer);
                foreach (TreeNode t in current.Nodes)
                    saveNodes(t, writer);
            writer.WriteEndElement();
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
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
                this.Text = this.Text.TrimEnd(changed);
            }
            else
                selectedData.Invalidate();
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

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            saveProject();
            this.Cursor = Cursors.Arrow;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select a Project File";
            openFileDialog1.Filter = "";
            openFile();
        }
    }

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
