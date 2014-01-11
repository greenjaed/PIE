using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public partial class CloneForm : Form
    {
        long start; //the starting address
        long copies; //the number of times the slice is cloned;
        TreeNode node; //the node to clone
        TreeNode parent; //the parent node
        Slice nodeData; //the slice to clone

        public CloneForm()
        {
            InitializeComponent();
        }

        public CloneForm(TreeNode node)
        {
            InitializeComponent();
            this.node = node;
            parent = node.Parent;
            nodeData = node.Tag as Slice;
            start = (node.Parent.Tag as Slice).lastStart + nodeData.size;
            startTextBox.Text = start.ToString("X");
            copies = 1;
        }

        private void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            Slice slice = parent.Tag as Slice;
            try
            {
                start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (start < slice.lastStart || start >= slice.end)
                    throw new ArgumentOutOfRangeException("start address");
                errorProvider1.SetError(startTextBox, "");
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
                if (errorProvider1.GetError(startTextBox) != "")
                    throw new Exception(errorProvider1.GetError(startTextBox));
                start -= (node.Parent.Tag as Slice).lastStart;
                if (!repeatCheckBox.Checked && Slice.IsTaken(parent, start, start + nodeData.size - 1))
                    throw new Exception(Properties.Resources.overlapString);

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
            int delIndex = -1;

            foreach (TreeNode t in parent.Nodes)
            {
                current = t.Tag as Slice;
                if (current.end < start)
                    continue;
                delIndex = t.Index;
                break;
            }
            if (delIndex >= 0)
            {
                if (MessageBox.Show("Warning: operation will overwrite existing slices", "Slice", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return false;
                else
                    while (parent.Nodes.Count > delIndex)
                        parent.Nodes[delIndex].Remove();
            }
            
            return true;
        }

        //clones a slice
        private void cloneNode(long position)
        {
            TreeNode subnode = new TreeNode();
            subnode.Name = (Owner as PIEForm).uniqueID.ToString();
            subnode.Text = position.ToString("X") + "-" + (position + nodeData.size - 1).ToString("X");
            Slice subslice = new Slice(parent.Tag as Slice, position, nodeData.size);
            subnode.Tag = subslice;
            parent.Nodes.Add(subnode);
            if (subSliceCheckBox.Checked)
                cloneNode(node, subnode);
        }

        //mass clones a slice
        private void cloneNode(TreeNode original, TreeNode clone)
        {
            TreeNode subnode;
            Slice subslice;

            foreach (TreeNode t in original.Nodes)
            {
                subnode = new TreeNode();
                subnode.Name = (Owner as PIEForm).uniqueID.ToString();
                subnode.Text = t.Text;
                subslice = new Slice(t.Tag as Slice, clone.Tag as Slice);
                subnode.Tag = subslice;
                clone.Nodes.Add(subnode);
                if (t.Nodes.Count > 0)
                    cloneNode(t, subnode);
            }
        }

        //clones a slice
        private bool clone()
        {
            long size = nodeData.size;
            long insertPosition = start;
            long totalSize = (parent.Tag as Slice).dataByteProvider.Length - 1;
            long max = (totalSize - insertPosition) / size;

            if (repeatCheckBox.Checked)
                copies = max;
            else
                copies = Math.Min(copies, max);

            if (clearExistingSlices())
            {
                for (int i = 0; i < copies; ++i)
                {
                    cloneNode(insertPosition);
                    insertPosition += size;
                }
                return true;
            }
            else
                return false;
        }

        private void copiesTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                copies = int.Parse(copiesTextBox.Text);
                if (copies <= 0)
                    throw new Exception("The number of copies must be 1 or greater");
                errorProvider1.SetError(copiesTextBox, "");
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
