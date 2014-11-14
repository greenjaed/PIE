using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace PIE
{
    public partial class CloneForm : Form
    {
        private long Start; //the starting address
        private long Copies; //the number of times the slice is cloned;
        private TreeNode Node; //the node to clone
        private TreeNode ParentNode; //the parent node
        private Slice NodeData; //the slice to clone
        private PIEInfo Info;

        public CloneForm()
        {
            InitializeComponent();
        }

        public CloneForm(TreeNode node)
        {
            InitializeComponent();
            this.Node = node;
            ParentNode = node.Parent;
            NodeData = node.Tag as Slice;
            Start = (ParentNode.Tag as Slice).Offset + NodeData.Start + NodeData.Size;
            startTextBox.Text = Start.ToString("X");
            Copies = 1;
            Info = (Owner as PIEForm).PieInfo;
        }

        private void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            Slice slice = ParentNode.Tag as Slice;
            try
            {
                Start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (Start < slice.Offset || Start >= slice.End)
                    throw new ArgumentOutOfRangeException("start address");
                errorProvider1.SetError(startTextBox, string.Empty);
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(startTextBox, ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cloneButton_Click(object sender, EventArgs e)
        {
            bool cloned;
            try
            {
                if (!string.IsNullOrEmpty(errorProvider1.GetError(startTextBox)))
				{
                    throw new Exception(errorProvider1.GetError(startTextBox));
				}
                Start -= (Node.Parent.Tag as Slice).Offset;
                if (!repeatCheckBox.Checked && Slice.IsTaken(ParentNode, Start, Start + NodeData.Size - 1))
                {
                    throw new Exception(Properties.Resources.overlapString);
                }

                this.Cursor = Cursors.WaitCursor;
                cloned = clone();
                this.Cursor = Cursors.Arrow;
                if (cloned)
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Clone", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //removes any existing slices when mass cloning
        private bool clearExistingSlices()
        {
            Slice current;
            int deletionIndex = -1;

            foreach (TreeNode t in ParentNode.Nodes)
            {
                current = t.Tag as Slice;
                if (current.End < Start)
                    continue;
                deletionIndex = t.Index;
                break;
            }
            if (deletionIndex >= 0)
            {
                if (MessageBox.Show("Warning: operation will overwrite existing slices", "Slice", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return false;
                }
                else
                {
                    while (ParentNode.Nodes.Count > deletionIndex)
                    {
                        ParentNode.Nodes[deletionIndex].Remove();
                    }
                }
            }
            
            return true;
        }

        //clones a slice
        private void cloneNode(long position, int cloneID)
        {
            TreeNode subnode = new TreeNode();
            subnode.Name = Info.UniqueID.ToString();
            subnode.Text = Node.Text + " " + cloneID.ToString();
            Slice subslice = new Slice(ParentNode.Tag as Slice, position, NodeData.Size);
            subnode.Tag = subslice;
            ParentNode.Nodes.Add(subnode);
            if (subSliceCheckBox.Checked)
            {
                cloneNode(Node, subnode);
            }
        }

        //mass clones a slice
        private void cloneNode(TreeNode original, TreeNode clone)
        {
            TreeNode subnode;
            Slice subslice;

            foreach (TreeNode t in original.Nodes)
            {
                subnode = new TreeNode();
                subnode.Name = Info.UniqueID.ToString();
                subnode.Text = t.Text;
                subslice = new Slice(t.Tag as Slice, clone.Tag as Slice);
                subnode.Tag = subslice;
                clone.Nodes.Add(subnode);
                if (t.Nodes.Count > 0)
                {
                    cloneNode(t, subnode);
                }
            }
        }

        //clones a slice
        private bool clone()
        {
            long size = NodeData.Size;
            long insertPosition = Start;
            long totalSize = (ParentNode.Tag as Slice).Data.Length - 1;
            long max = (totalSize + 1 - insertPosition) / size;

            if (repeatCheckBox.Checked)
            {
                Copies = max;
            }
            else
            {
                Copies = Math.Min(Copies, max);
            }

            if (clearExistingSlices())
            {
                for (int i = 0; i < Copies; ++i)
                {
                    cloneNode(insertPosition, i + 1);
                    insertPosition += size;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void copiesTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                Copies = int.Parse(copiesTextBox.Text);
                if (Copies <= 0)
                {
                    throw new Exception("The number of copies must be 1 or greater");
                }
                errorProvider1.SetError(copiesTextBox, string.Empty);
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(copiesTextBox, ex.Message);
            }
        }

        private void repeatCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            copiesTextBox.Enabled = !repeatCheckBox.Checked;
        }


    }
}
