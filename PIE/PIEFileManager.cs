using System;
using System.IO;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    class PIEFileManager
    {
        private PIESliceManager SliceManager;
        private PIEInfo PieInfo;
        private TreeView ProjectTreeView;
        public bool LengthChanged { get; private set; }

        public PIEFileManager(PIESliceManager sliceManager, PIEInfo pieInfo)
        {
            SliceManager = sliceManager;
            PieInfo = pieInfo;
            ProjectTreeView = pieInfo.ProjectTreeView;
        }

        public void SetSliceManager(PIESliceManager sliceManager)
        {
            SliceManager = sliceManager;
        }

        public void ReloadFileBytes()
        {
            PieInfo.FileBytes.Dispose();
            PieInfo.FileBytes = new DynamicFileByteProvider(PieInfo.FilePath);
        }

        public void ClearFileBytes()
        {
            PieInfo.FileBytes.Dispose();
            PieInfo.FileBytes = null;
        }

        public void ReloadFile()
        {
            if (PieInfo.FileBytes.HasChanges() &&
                MessageBox.Show("Revert all changes?", "PIE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                PieInfo.ReloadFileBytes();
                (ProjectTreeView.Nodes[0].Tag as Slice).SetMainSlice(PieInfo.FileBytes);
                PIESliceManager.PropagateSliceChanges(ProjectTreeView.Nodes[0]);
                ProjectTreeView.Nodes[0].Text = ProjectTreeView.Nodes[0].Text.TrimEnd(PIEForm.Changed);
                PieInfo.PieForm.ViewController.Display();
            }
        }

        public bool CloseFile()
        {
            if (PIESliceManager.AnySlicesChanged(ProjectTreeView.Nodes[0]))
            {
                DialogResult result = MessageBox.Show("Apply changes to file before closing?", "PIE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SliceManager.SaveAllSlices();
                    SaveFile(true);
                    PieInfo.DisplayHexBox.ByteProvider = null;
                    PieInfo.FileBytes.Dispose();
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        public bool OpenFile()
        {
            var FileOpenDialog = new OpenFileDialog();
            FileOpenDialog.Title = "Select a Project File";
            return OpenFile(true);
        }

        public bool OpenFile(string filePath)
        {
            PieInfo.FilePath = filePath;
            return OpenFile(false);
        }

        public bool OpenFile(bool newProject)
        {
            try
            {
                if (PieInfo.FileBytes != null && !CloseFile())
                {
                    return false;
                }

                if (newProject)
                {
                    var FileOpenFileDialog = new OpenFileDialog();
                    if (FileOpenFileDialog.ShowDialog() == DialogResult.Cancel)
                    {
                        return false;
                    }
                    else
                        PieInfo.FilePath = FileOpenFileDialog.FileName;
                }

                if (File.Exists(PieInfo.FilePath))
                {
                    var projectTreeView = PieInfo.ProjectTreeView;
                    if (PieInfo.FileBytes != null)
                    {
                        PieInfo.FileBytes.Dispose();
                    }
                    PieInfo.FileBytes = new DynamicFileByteProvider(PieInfo.FilePath);
                    PieInfo.FileBytes.LengthChanged += new EventHandler(FileBytes_LengthChanged);

                    if (projectTreeView.Nodes.Count > 0)
                    {
                        projectTreeView.Nodes.Clear();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        public void SaveFile(bool isClosing)
        {
            try
            {
                //this.Cursor = Cursors.WaitCursor;
                //if the file length has changed, update any slices
                if (LengthChanged)
                {
                    (ProjectTreeView.Nodes[0].Tag as Slice).Resize(ProjectTreeView.Nodes[0], 0, PieInfo.FileBytes.Length - 1);
                }
                SliceManager.SaveChangedSlices(isClosing);
                //this.Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not save changes.  Original message: " + ex.Message);
            }
        }

        void FileBytes_LengthChanged(object sender, EventArgs e)
        {
            LengthChanged = true;
        }
    }
}
