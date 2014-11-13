using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Be.Windows.Forms;

namespace PIE
{
    class PIEProjectManager
    {
        public PIEFileManager FileManager { get; private set; }
        public PIESliceManager SliceManager { get; private set; }
        public string FilePath { get { return PieInfo.FilePath; } }
        public string ProjectPath { get; private set; }
        private OpenFileDialog ProjectOpenFileDialog;
        private SaveFileDialog ProjectSaveFileDialog;
        public TreeView ProjectTreeView { get; private set; }
        public HexBox DisplayHexBox { get; private set; }
        private PIEInfo PieInfo;

        public bool ChangedMind
        {
            get
            {
                return PieInfo.FileBytes != null && !(FileManager.CloseFile() && CloseProject());
            }
        }

        public PIEProjectManager(PIEInfo pieInfo)
        {
            PieInfo = pieInfo;
            ProjectTreeView = PieInfo.ProjectTreeView;
            DisplayHexBox = PieInfo.DisplayHexBox;
            SliceManager = new PIESliceManager(PieInfo);
            FileManager = new PIEFileManager(SliceManager, PieInfo);
            ProjectOpenFileDialog = new OpenFileDialog();
            ProjectSaveFileDialog = new SaveFileDialog();
        }

        public void OpenProject(string projectPath)
        {
            if (ChangedMind)
            {
                return;
            }
            ProjectPath = projectPath;
            openProject();
        }

        public void OpenProject()
        {
            if (ChangedMind)
            {
                return;
            }
            ProjectPath = getFilename();
            if (!string.IsNullOrEmpty(ProjectPath))
            {
                openProject();
            }
        }

        private void openProject()
        {
            Slice wholeSlice;
            TreeNode rootNode;
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.CloseInput = true;
            rs.IgnoreComments = true;
            rs.IgnoreWhitespace = true;

            try
            {
                using (XmlReader xr = XmlReader.Create(ProjectPath, rs))
                {
                    xr.ReadToFollowing("PIEForm");
                    xr.Read();

                    string filePath = xr.ReadElementContentAsString();

                    if (!File.Exists(filePath))
                    {
                        string pathToCheck = Path.GetDirectoryName(ProjectPath) + "\\" + Path.GetFileName(filePath);
                        if (!File.Exists(pathToCheck))
                            throw new FileNotFoundException("Unable to locate " + filePath);
                        else
                            filePath = pathToCheck;
                    }

                    FileManager.OpenFile(filePath);

                    if (PieInfo.FileBytes != null)
                    {
                        PieInfo.ProjectChanged = true;
                        updateMRU(ProjectPath);
                        //recentProjectsToolStripMenuItem.Enabled = true;
                        PieInfo.DeserializeIdIndex(xr);
                        xr.ReadToFollowing("Node");
                        xr.Read();
                        rootNode = new TreeNode(xr.ReadElementContentAsString());
                        rootNode.Text = xr.ReadElementContentAsString();
                        wholeSlice = Slice.Deserialize(XmlDictionaryReader.CreateDictionaryReader(xr));
                        initializeProject(wholeSlice, rootNode);
                        if (xr.IsStartElement())
                        {
                            loadNodes(rootNode, XmlDictionaryReader.CreateDictionaryReader(xr));
                        }
                        //this.Text = "PIE - " + Path.GetFileNameWithoutExtension(projectPath);
                        SliceManager.DisplaySlice();
                        //toggleControls(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadNodes(TreeNode current, XmlDictionaryReader xdr)
        {
            Slice slice;
            TreeNode node;
            //load all children
            do
            {
                //load the TreeNode information
                xdr.Read();
                node = new TreeNode(xdr.ReadElementContentAsString());
                node.Text = xdr.ReadElementContentAsString();
                slice = Slice.Deserialize(xdr);
                slice.Parent = current.Tag as Slice;
                node.Tag = slice;
                current.Nodes.Add(node);
                //if this TreeNode has children, recurse
                if (xdr.IsStartElement())
                {
                    loadNodes(node, xdr);
                }
            } while (xdr.ReadToNextSibling("Node"));
        }

        private void initializeProject(Slice wholeSlice, TreeNode rootNode)
        {
            wholeSlice.SetMainSlice(PieInfo.FileBytes);
            rootNode.Tag = wholeSlice;
            ProjectTreeView.Nodes.Add(rootNode);
            PieInfo.ActiveSlice = wholeSlice;
            PieInfo.CurrentTreeNode = rootNode;
        }

        private void updateMRU(string projectPath)
        {
            
            if (Properties.Settings.Default.mru.Contains(projectPath))
            {
                //removing and adding project file puts it at the top of the list
                Properties.Settings.Default.mru.Remove(projectPath);
            }
            else
            {
                PieInfo.PieForm.addRecent(projectPath);
            }
            Properties.Settings.Default.mru.Add(projectPath);
            Properties.Settings.Default.Save();
        }

        private string getFilename()
        {

            ProjectOpenFileDialog.Title = "Select a Project";
            ProjectOpenFileDialog.Filter = "PIE files (*.pie)|*.pie|All files (*.*)|*.*";
            if (ProjectOpenFileDialog.ShowDialog(PieInfo.PieForm) == DialogResult.Cancel)
            {
                return string.Empty;
            }
            return ProjectOpenFileDialog.FileName;
        }

        public void Save()
        {
            if (PieInfo.CurrentTreeNode.Parent == null)
            {
                FileManager.SaveFile(false);
            }
            else
            {
                SliceManager.SaveChangedSlices(false);
            }
        }

        public bool SaveAll()
        {
            SliceManager.SaveAllSlices();
            return SaveProject();
        }

        public void InitializeProjectTree()
        {
            TreeNode rootNode;
            Slice rootData;

            rootNode = new TreeNode(PieInfo.FileName);
            rootNode.Name = PieInfo.UniqueID.ToString();
            rootData = new Slice(PieInfo.FileBytes);
            rootNode.Tag = rootData;
            if (ProjectTreeView.Nodes.Count > 0)
            {
                ProjectTreeView.Nodes.Clear();
            }
            ProjectTreeView.Nodes.Add(rootNode);
            PieInfo.ActiveSlice = rootData;
            PieInfo.CurrentTreeNode = rootNode;
        }

        public bool CloseProject()
        {
            if (PieInfo.ProjectChanged)
            {
                DialogResult result = MessageBox.Show("Apply changes to project before closing?",
                                                      "PIE", MessageBoxButtons.YesNoCancel,
                                                      MessageBoxIcon.Question);

                if ((result == DialogResult.Yes && SaveProject()) || result == DialogResult.No)
                {
                    PieInfo.PieForm.ViewController.Hide();
                    FileManager.ClearFileBytes();
                    ProjectTreeView.Nodes.Clear();
                    return true;
                }
                return false;
            }
            return true;
        }

        public bool SaveProject()
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.NewLineOnAttributes = true;
            writerSettings.CloseOutput = true;

            try
            {
                ProjectSaveFileDialog.InitialDirectory = Path.GetDirectoryName(FilePath);
                ProjectSaveFileDialog.FileName = PieInfo.FileName + ".pie";

                if (!File.Exists(ProjectPath))
                {
                    if (ProjectSaveFileDialog.ShowDialog(PieInfo.PieForm) == DialogResult.Cancel)
                    {
                        return false;
                    }
                    else
                    {
                        ProjectPath = ProjectSaveFileDialog.FileName;
                    }
                }

                using (XmlWriter projectFile = XmlWriter.Create(ProjectPath, writerSettings))
                {
                    projectFile.WriteStartDocument();
                    projectFile.WriteStartElement("PIEProject");
                    projectFile.WriteStartElement("PIEForm");
                    projectFile.WriteElementString("FilePath", FilePath);
                    PieInfo.SerializeIdIndex(projectFile);
                    projectFile.WriteEndElement();
                    serializeSlices(ProjectTreeView.Nodes[0], XmlDictionaryWriter.CreateDictionaryWriter(projectFile));
                    projectFile.WriteEndElement();
                    projectFile.WriteEndDocument();
                }

                if (!Properties.Settings.Default.mru.Contains(ProjectPath))
                {
                    Properties.Settings.Default.mru.Add(ProjectPath);
                    if (Properties.Settings.Default.mru.Count > 10)
                    {
                        Properties.Settings.Default.mru.RemoveAt(10);
                    }
                    Properties.Settings.Default.Save();
                    PieInfo.PieForm.addRecent(ProjectPath);
                    //recentProjectsToolStripMenuItem.Enabled = true;
                }

                PieInfo.ProjectChanged = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private static void serializeSlices(TreeNode current, XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Node");
            writer.WriteElementString("Name", current.Name);
            writer.WriteElementString("Text", current.Text);
            (current.Tag as Slice).Serialize(writer);
            foreach (TreeNode t in current.Nodes)
            {
                serializeSlices(t, writer);
            }
            writer.WriteEndElement();
        }

        public void SwitchToTable()
        {
            PieInfo.PieForm.ChangeController();
            if (!(PieInfo.ActiveSlice is TableSlice))
            {
                TableSlice table;
                if (ProjectTreeView.Focused)
                {
                    table = new TableSlice(ProjectTreeView.SelectedNode.Tag as Slice);
                }
                else
                {
                    table = new TableSlice(PieInfo.ActiveSlice);
                }
                if (table.EditColumns())
                {
                    PieInfo.CurrentTreeNode.Tag = PieInfo.ActiveSlice = table;
                    PieInfo.ProjectChanged = true;
                    //showProjectChanged();
                    Cursor.Current = Cursors.WaitCursor;
                    PieInfo.PieForm.ViewController.Model = table;
                    PieInfo.PieForm.ViewController.Display();
                    Cursor.Current = Cursors.Default;
                }
            }
            else
            {
                PieInfo.PieForm.ViewController.Display();
            }
        }
    }
}
