using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Be.Windows.Forms;
using PIE.Slices;

namespace PIE
{
    public class PIEInfo
    {
        private string _FileName;
        //generates a unique ID
        public ulong UniqueID { get { return IdIndex++; } }
        //the unique ID
        public ulong IdIndex { get; set; }
        public TreeView ProjectTreeView { get; private set; }
        public TreeNode CurrentTreeNode { get; set; }
        public Slice ActiveSlice { get; set; }
        public bool ProjectChanged { get; set; }
        public string FilePath { get; set; }
        public DynamicFileByteProvider FileBytes { get; set; }
        public PIEForm PieForm { get; private set; }
        public static char[] Changed = new char[] { '*' };

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_FileName))
                {
                    _FileName = Path.GetFileNameWithoutExtension(FilePath ?? string.Empty);
                }
                return _FileName;
            }
        }

        public string SliceRange
        {
            get
            {
                return string.Format("{0}:  {1} - {2}",
                    CurrentTreeNode.Text,
                    ActiveSlice.Start.ToString("X"),
                    ActiveSlice.End.ToString("X"));
            }
        }

        public PIEInfo(TreeView projectTreeview, PIEForm pieForm)
        {
            ProjectTreeView = projectTreeview;
            PieForm = pieForm;
        }

        public PIEInfo(PIEInfo pieInfo)
        {
            _FileName = pieInfo._FileName;
            IdIndex = pieInfo.IdIndex;
            ProjectTreeView = pieInfo.ProjectTreeView;
            CurrentTreeNode = pieInfo.CurrentTreeNode;
            ActiveSlice = pieInfo.ActiveSlice;
            ProjectChanged = pieInfo.ProjectChanged;
            FilePath = pieInfo.FilePath;
            FileBytes = pieInfo.FileBytes;
            PieForm = pieInfo.PieForm;
        }

        public void DeserializeIdIndex(XmlReader xr)
        {
            IdIndex = ulong.Parse(xr.ReadElementContentAsString());
        }

        public void SerializeIdIndex(XmlWriter xw)
        {
            xw.WriteElementString("idIndex", IdIndex.ToString());
        }

        public void SetActiveSlice()
        {
            CurrentTreeNode = ProjectTreeView.SelectedNode;
            ActiveSlice = CurrentTreeNode.Tag as Slice;
            PieForm.ViewController.Model = ActiveSlice;
        }

        public void ReloadFileBytes()
        {
            FileBytes.Dispose();
            FileBytes = new DynamicFileByteProvider(FilePath);
        }
    }
}
