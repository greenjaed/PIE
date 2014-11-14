using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Be.Windows.Forms;

namespace PIE
{
    public partial class PIEForm : Form
    {
        //indicates whether data is being selected
        public bool IsSelecting { get; private set; }
        //searches for data
        private FindForm SearchForm;
        //the start and end of a slice selection
        private TreeNode SelectionStart, SelectionEnd;

        public ToolStripItem PositionDisplay
        {
            get
            {
                return statusStrip.Items["positionToolStripStatusLabel"];
            }
        }

        public ToolStripStatusLabel InsertRemove
        {
            get
            {
                return insertToolStripStatusLabel;
            }
        }

        private PIEProjectManager ProjectManager;
        private PIEFileManager FileManager;
        private PIESliceManager SliceManager;
        public ISliceController ViewController { get; private set; }
        public PIEInfo PieInfo { get; private set; }

        string Title
        {
            get
            {
                return "PIE" + (string.IsNullOrEmpty(PieInfo.FileName) ? string.Empty : " - " + PieInfo.FileName);
            }
        }

        public PIEForm()
        {
            InitializeComponent();
            projectTreeView.TreeViewNodeSorter = new ProjectTreeSort();
            PieInfo = new PIEInfo(projectTreeView, displayHexBox, this);
            ProjectManager = new PIEProjectManager(PieInfo);
            //ProjectManager = new PIEProjectManager(this, projectTreeView, displayHexBox);
            FileManager = ProjectManager.FileManager;
            SliceManager = ProjectManager.SliceManager;
            ViewController = new HexBoxController(this, displayHexBox);
        }

        #region  ToolStripMenuItem_Click Event Handlers

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PIEAboutBox about = new PIEAboutBox();
            about.ShowDialog(this);
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloneForm cloneForm = new CloneForm(projectTreeView.SelectedNode);
            if (cloneForm.ShowDialog(this) == DialogResult.OK)
                showProjectChanged();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectManager.ChangedMind)
            {
                return;
            }
            displayHexBox.ByteProvider = null;
            this.Text = "PIE";
            toggleControls(false);
            TogglePaste(false);
            sliceToolStripMenuItem.Enabled = false;
            sliceToolStripStatusLabel.Text = string.Empty;
            positionToolStripStatusLabel.Text = string.Empty;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.Copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.Cut();
        }

        private void deleteSliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SliceManager.DeleteSlice(SelectionStart, SelectionEnd);
            showProjectChanged();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.Delete();
        }

        private void editColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((PieInfo.ActiveSlice as TableSlice).EditColumns())
            {
                showProjectChanged();
                Cursor.Current = Cursors.WaitCursor;
                ViewController.Display();
                Cursor.Current = Cursors.Default;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SliceManager.ExportSlice();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SearchForm == null)
            {
                performSearch();
            }
            else
            {
                SearchForm.search();
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            performSearch();
        }

        private void hexToolStripButton_Click(object sender, EventArgs e)
        {
            if (!(ViewController is HexBoxController))
            {
                ViewController = new HexBoxController(this, displayHexBox);
            }
            ViewController.Display();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SliceManager.ImportSlice();
        }

        //puts a group of slices all under the same slice
        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SliceManager.MergeSlices(SelectionStart, SelectionEnd);
            showProjectChanged();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileManager.OpenFile())
            {
                ProjectManager.InitializeProjectTree();
                displayHexBox.ByteProvider = PieInfo.FileBytes;
                this.Text = "PIE - new project";
                SliceManager.DisplaySlice();
                toggleControls(true);
            }
        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotesForm infoForm = new NotesForm(projectTreeView.SelectedNode.Tag as Slice);
            infoForm.Show(this);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.OpenProject();
            if (PieInfo.FileBytes != null)
            {
                this.Text = Title;
                projectTreeView.SelectedNode = projectTreeView.Nodes[0];
                recentProjectsToolStripMenuItem.Enabled = true;
                displaySliceInfo();
                toggleControls(true);
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm(displayHexBox);
            optionForm.ShowDialog(this);
        }

        //overwrites the bytes instead of inserting them
        private void pasteOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.PasteOver();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.Paste();
        }

        //reloads the file only
        private void reloadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileManager.ReloadFile();
        }

        //reloads the current slice. this could be the entire file
        private void reloadSliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SliceManager.ReloadSlice();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectTreeView.SelectedNode.BeginEdit();
        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResizeForm resizeForm = new ResizeForm(projectTreeView.SelectedNode);
            var currentTreeNode = PieInfo.CurrentTreeNode;
            if (resizeForm.ShowDialog(this) == DialogResult.OK)
            {
                showProjectChanged();
            }
            if (resizeForm.Node == currentTreeNode)
            {
                ViewController.Model = currentTreeNode.Tag as Slice;
                ViewController.Display();
                startAddrToolStripComboBox.Items.Clear();
                startAddrToolStripComboBox.Items.AddRange(PieInfo.ActiveSlice.Addresses());
            }
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (ProjectManager.SaveAll())
            {
                this.Text.TrimEnd(PIEInfo.Changed);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (ProjectManager.SaveProject())
            {
                this.Text = Title;
            }
            this.Cursor = Cursors.Arrow;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.Save();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewController.SelectAll();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displaySliceInfo();
        }

        private void sliceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SliceManager.CreateSlice(displayHexBox.SelectionStart, displayHexBox.SelectionLength);
            showProjectChanged();
        }

        private void sliceToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SliceForm sliceForm = new SliceForm(projectTreeView.SelectedNode);
            if (sliceForm.ShowDialog(this) == DialogResult.OK)
            {
                showProjectChanged();
            }
        }

        //removes the parent slice and moves the child slices up
        private void splitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SliceManager.SplitSlice(SelectionStart, SelectionEnd);
            showProjectChanged();
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Help.ShowHelp(this, "PIE help.chm");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region  ToolStripItem IEnumerables

        private IEnumerable<ToolStripItem> EditItems()
        {
            yield return hexContextMenuStrip.Items["sliceHexToolStripMenuItem"];
            yield return hexContextMenuStrip.Items["cutHexToolStripMenuItem"];
            yield return hexContextMenuStrip.Items["copyHexToolStripMenuItem"];
            yield return hexContextMenuStrip.Items["deleteHexToolStripMenuItem"];
            yield return standardToolStrip.Items["cutToolStripButton"];
            yield return standardToolStrip.Items["copyToolStripButton"];
            yield return editToolStripMenuItem.DropDownItems["cutToolStripMenuItem"];
            yield return editToolStripMenuItem.DropDownItems["copyToolStripMenuItem"];
            yield return editToolStripMenuItem.DropDownItems["deleteToolStripMenuItem"];
        }

        private IEnumerable<ToolStripItem> OpenedProjectItems()
        {
            yield return editToolStripMenuItem.DropDownItems["findToolStripMenuItem"];
            yield return editToolStripMenuItem.DropDownItems["selectAllToolStripMenuItem"];
            yield return editToolStripMenuItem.DropDownItems["findNextToolStripMenuItem"];
            yield return fileToolStripMenuItem.DropDownItems["saveToolStripMenuItem"];
            yield return fileToolStripMenuItem.DropDownItems["saveProjectToolStripMenuItem"];
            yield return fileToolStripMenuItem.DropDownItems["reloadToolStripMenuItem"];
            yield return fileToolStripMenuItem.DropDownItems["saveAllToolStripMenuItem"];
            yield return fileToolStripMenuItem.DropDownItems["closeProjectToolStripMenuItem"];
            yield return standardToolStrip.Items["saveToolStripButton"];
            yield return standardToolStrip.Items["saveProjectToolStripButton"];
            yield return standardToolStrip.Items["saveAllToolStripButton"];
            yield return hexContextMenuStrip.Items["selectAllHexToolStripMenuItem"];
        }

        private IEnumerable<ToolStripItem> PasteItems()
        {
            yield return editToolStripMenuItem.DropDownItems["pasteToolStripMenuItem"];
            yield return editToolStripMenuItem.DropDownItems["pasteOverToolStripMenuItem"];
            yield return hexContextMenuStrip.Items["pasteHexToolStripMenuItem"];
            yield return hexContextMenuStrip.Items["pasteOverHexToolStripMenuItem"];
            yield return standardToolStrip.Items["pasteToolStripButton"];
            yield return standardToolStrip.Items["pasteOverToolStripButton"];
        }

        #endregion

        //enables/disables a bunch of controls relating to cut/copy/paste, delete and slicing
        //enableValue is the value the controls are set to
        public void ToggleEnable(bool toggle)
        {
            IsSelecting = toggle;
            sliceToolStripButton.Enabled = toggle;
            foreach (var item in EditItems())
            {
                item.Enabled = toggle;
            }
            if (displayHexBox.CanPaste())
            {
                TogglePaste(true);
            }
        }

        //displays a slice
        private void displaySliceInfo()
        {
            startAddrToolStripComboBox.Items.Clear();
            startAddrToolStripComboBox.Items.AddRange(PieInfo.ActiveSlice.Addresses());
            //show the slice name and the data range
            sliceToolStripStatusLabel.Text = PieInfo.SliceRange;
            //if the current slice is not the entire file, enable import/export
            fileToolStripMenuItem.DropDownItems["exportToolStripMenuItem"].Enabled =
                fileToolStripMenuItem.DropDownItems["importToolStripMenuItem"].Enabled = !(PieInfo.CurrentTreeNode.Parent == null);
            ToggleEnable(false);
            ViewController.UpdatePosition();
        }

        //when a project is opened/closed, enable/disable all related controls
        //the listed controls are set to toggle
        private void toggleControls(bool toggle)
        {
            foreach (var item in OpenedProjectItems())
            {
                item.Enabled = toggle;
            }
            startAddrToolStripComboBox.Enabled = toggle;
            gotoToolStripTextBox.Enabled = toggle;
        }

        //enables/disables pasting
        //value set by toggle value
        public void TogglePaste(bool toggle)
        {
            foreach (var item in PasteItems())
            {
                item.Enabled = toggle;
            }
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
                    displayHexBox.InfoForeColor = (Color)cc.ConvertFromString(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.SelectionBackColor = (Color)cc.ConvertFromString(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.ShadowSelectionColor = (Color)cc.ConvertFromString(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.SelectionForeColor = (Color)cc.ConvertFromString(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.StringViewVisible = bool.Parse(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.LineInfoVisible = bool.Parse(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.HexCasing = (HexCasing)Enum.Parse(typeof(HexCasing), line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.BytesPerLine = int.Parse(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.GroupSeparatorVisible = bool.Parse(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.GroupSize = int.Parse(line.Substring(line.IndexOf('=') + 1));
                    line = settingsFile.ReadLine();
                    displayHexBox.ColumnInfoVisible = bool.Parse(line.Substring(line.IndexOf('=') + 1));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //adds an asterisk to the end of the project name to show unsaved changes
        private void showProjectChanged()
        {
            if (!this.Text.EndsWith("*"))
            {
                this.Text += "*";
            }
        }

        private void PIEForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ProjectManager.ChangedMind)
            {
                e.Cancel = true;
            }
        }

        //searches the file
        private void performSearch()
        {
            SearchForm = new FindForm(displayHexBox);
            SearchForm.Show(this);
        }

        //when reloading or saving, clears all sub-slices of data
        //when displayed again, the slice will reflect changes
        private void propagateSave(TreeNode current)
        {
            foreach (TreeNode n in current.Nodes)
            {
                if (n.Nodes.Count > 0)
                {
                    propagateSave(n);
                }
                (n.Tag as Slice).Invalidate();
                n.Text = n.Text.TrimEnd(PIEInfo.Changed);
            }
        }

        private void clearSelection()
        {
            if (SelectionStart == null || SelectionStart.Parent == null)
            {
                return;
            }
            foreach (TreeNode node in SelectionStart.Parent.Nodes)
            {
                node.ForeColor = projectTreeView.ForeColor;
                node.BackColor = projectTreeView.BackColor;
            }
        }

        private void toggleChildrenControls(bool toggle)
        {
            sliceToolStripMenuItem.DropDownItems["resizeToolStripMenuItem1"].Enabled = toggle;
            sliceToolStripMenuItem.DropDownItems["cloneToolStripMenuItem1"].Enabled = toggle;
            projectContextMenuStrip.Items["resizeToolStripMenuItem"].Enabled = toggle;
            projectContextMenuStrip.Items["cloneToolStripMenuItem"].Enabled = toggle;
            hexToolStripButton.Enabled = toggle;
            tableToolStripButton.Enabled = toggle;
            if (toggle)
            {
                projectContextMenuStrip.Items["splitToolStripMenuItem"].Visible =
                       sliceToolStripMenuItem.DropDownItems["splitToolStripMenuItem1"].Visible = projectTreeView.SelectedNode.Nodes.Count > 0;
                projectContextMenuStrip.Items["mergeToolStripMenuItem"].Visible =
                    sliceToolStripMenuItem.DropDownItems["mergeToolStripMenuItem1"].Visible = SelectionStart != SelectionEnd;
            }
            else
            {
                projectContextMenuStrip.Items["splitToolStripMenuItem"].Visible = false;
                sliceToolStripMenuItem.DropDownItems["splitToolStripMenuItem1"].Visible = false;
                projectContextMenuStrip.Items["mergeToolStripMenuItem"].Visible = false;
                sliceToolStripMenuItem.DropDownItems["mergeToolStripMenuItem1"].Visible = false;
            }

        }

        private void projectTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            //if the shift key is held and both nodes are in the same tier
            if (ModifierKeys == Keys.Shift && SelectionStart.Parent == node.Parent)
            {
                int start = Math.Min(node.Index, SelectionStart.Index);
                int end = Math.Max(node.Index, SelectionStart.Index);
                if (start != end)
                {
                    clearSelection();
                    SelectionEnd = node;
                    for (int i = start; i <= end; ++i)
                        highlightSelection(node.Parent.Nodes[i]);
                }
            }
            else if (e.Button != MouseButtons.Right)
            {
                if (SelectionStart != SelectionEnd)
                {
                    clearSelection();
                }
                SelectionStart = SelectionEnd = node;
            }
            projectTreeView.SelectedNode = e.Node;
            if (!projectContextMenuStrip.Enabled)
            {
                projectContextMenuStrip.Enabled = true;
                foreach (ToolStripItem t in projectContextMenuStrip.Items)
                {
                    t.Enabled = true;
                }
            }
            sliceToolStripMenuItem.Enabled = true;
            //if the slice is not the entire file, enable resizing and cloning
            toggleChildrenControls(e.Node.Parent == null ? false : true);
        }

        private void projectTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            SliceManager.DisplaySlice();
            displaySliceInfo();
        }

        private void PIEForm_Activated(object sender, EventArgs e)
        {
            if (displayHexBox.CanPaste())
            {
                TogglePaste(true);
            }
        }

        private bool selectionIsGrowing(TreeNode current)
        {
            return SelectionStart.Index >= SelectionEnd.Index && current.Index < SelectionEnd.Index ||
                SelectionStart.Index <= SelectionEnd.Index && current.Index > SelectionEnd.Index;
        }

        private void projectTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (projectTreeView.SelectedNode != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        displaySliceInfo();
                        break;
                    case Keys.Delete:
                        SliceManager.DeleteSlice(SelectionStart, SelectionEnd);
                        break;
                    //case Keys.Up:
                    //case Keys.Down:
                    //    if (ModifierKeys == Keys.Shift &&
                    //        projectTreeView.SelectedNode.Parent == selectionStart.Parent)
                    //    {
                    //        if (!selectionIsGrowing(projectTreeView.SelectedNode))
                    //        {
                    //            selectionEnd.BackColor = projectTreeView.BackColor;
                    //            selectionEnd.ForeColor = projectTreeView.ForeColor;
                    //        }
                    //        else if (projectTreeView.SelectedNode != selectionEnd)
                    //        {
                    //            if (selectionStart == selectionEnd)
                    //                highlightSelection(selectionStart);
                    //            selectionEnd = projectTreeView.SelectedNode;
                    //            highlightSelection(selectionEnd);
                    //        }
                    //        selectionEnd = projectTreeView.SelectedNode;
                    //    }
                    //    else
                    //    {
                    //        clearSelection();
                    //        selectionStart = selectionEnd = projectTreeView.SelectedNode;
                    //    }
                    //    e.Handled = true;
                    //    break;
                }
            }
        }

        private void highlightSelection(TreeNode toHighlight)
        {
            toHighlight.BackColor = SystemColors.Highlight;
            toHighlight.ForeColor = SystemColors.HighlightText;
        }

        private void startAddrToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PieInfo.ActiveSlice.Offset = long.Parse(startAddrToolStripComboBox.SelectedItem as string, NumberStyles.HexNumber);
            updateSliceInfo();
            ViewController.UpdatePosition();
        }

        private void startAddrToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                long potentialAddress;
                try
                {
                    potentialAddress = long.Parse(startAddrToolStripComboBox.Text, NumberStyles.HexNumber);
                    ViewController.Offset = potentialAddress;
                    if (!startAddrToolStripComboBox.Items.Contains(startAddrToolStripComboBox.Text))
                    {
                        startAddrToolStripComboBox.Items.Add(startAddrToolStripComboBox.Text);
                    }
                    updateSliceInfo();
                    ViewController.UpdatePosition();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    startAddrToolStripComboBox.Text = string.Empty;
                }
            }
        }

        private void gotoToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var activeSlice = PieInfo.ActiveSlice;
                long gotoAddress;
                try
                {
                    gotoAddress = long.Parse(gotoToolStripTextBox.Text, NumberStyles.HexNumber);
                    if (gotoAddress < displayHexBox.LineInfoOffset)
                    {
                        gotoAddress = displayHexBox.LineInfoOffset;
                    }
                    if (gotoAddress > displayHexBox.LineInfoOffset + activeSlice.Size)
                    {
                        gotoAddress = activeSlice.Size - 1;
                    }
                    ViewController.ScrollToAddress(gotoAddress);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    startAddrToolStripComboBox.Text = string.Empty;
                }
            }
        }

        //saves project slices
        private void saveSlices(TreeNode current, XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Node");
            writer.WriteElementString("Name", current.Name);
            writer.WriteElementString("Text", current.Text);
            (current.Tag as Slice).Serialize(writer);
            foreach (TreeNode t in current.Nodes)
            {
                saveSlices(t, writer);
            }
            writer.WriteEndElement();
        }

        private void projectTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            updateSliceInfo();
            if (!string.IsNullOrEmpty(e.Label) && PieInfo.CurrentTreeNode.Text != e.Label)
            {
                showProjectChanged();
            }
        }

        private void updateSliceInfo()
        {
            sliceToolStripStatusLabel.Text = PieInfo.SliceRange;
        }

        private void PIEForm_Load(object sender, EventArgs e)
        {
            //if a configuration file exists, load settings from it
            if (File.Exists(Application.StartupPath + Properties.Resources.configFileString))
            {
                loadSettings();
            }
            foreach (string s in Properties.Settings.Default.mru)
            {
                addRecent(s);
            }

            if (!recentProjectsToolStripMenuItem.HasDropDownItems)
            {
                recentProjectsToolStripMenuItem.Enabled = false;
            }
        }

        public void addRecent(string recent)
        {
            ToolStripMenuItem recentFile = new ToolStripMenuItem(recent);
            recentProjectsToolStripMenuItem.DropDownItems.Insert(0, recentFile);
        }

        private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ProjectManager.OpenProject(e.ClickedItem.Text);
            if (PieInfo.FileBytes != null)
            {
                this.Text = Title;
                displaySliceInfo();
                toggleControls(true);
            }
        }

        private void gotoToolStripTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void projectTreeView_Leave(object sender, EventArgs e)
        {
            clearSelection();
            SelectionStart = SelectionEnd = projectTreeView.SelectedNode;
        }

        private void tableToolStripButton_Click(object sender, EventArgs e)
        {
            ProjectManager.SwitchToTable();
            if (PieInfo.ProjectChanged)
            {
                showProjectChanged();
                tableToolStripMenuItem.Enabled = true;
            }
        }

        public void ChangeController()
        {
            //eventually, will need to pass in something to determine what to change it to.
        }
    }

    //sorts the nodes in projectTreeView by the start value of the contained slice
    public class ProjectTreeSort : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            Slice a = (x as TreeNode).Tag as Slice;
            Slice b = (y as TreeNode).Tag as Slice;
            if (a.Start < b.Start)
            {
                return -1;
            }
            else if (a.Start > b.Start)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
