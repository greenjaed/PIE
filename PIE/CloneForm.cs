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
        TreeNode node; //the node to clone
        TreeNode parent; //the parent node
        Slice nodeData; //the slice to clone
        //indicates whether any changes occurred
        public bool changed { get; protected set; }

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
            start = nodeData.end + 1;
            startTextBox.Text = (start + nodeData.lastStart).ToString("X");
        }

        private void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                start = long.Parse(startTextBox.Text, NumberStyles.HexNumber) - nodeData.lastStart;
                if (start < 0 || start > nodeData.end)
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
            this.Close();
        }

        private void cloneButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (errorProvider1.GetError(startTextBox) != "")
                    throw new Exception(errorProvider1.GetError(startTextBox));
                if (!repeatCheckBox.Checked && Slice.IsTaken(parent, start, start + nodeData.size - 1))
                    throw new Exception(Properties.Resources.overlapString);
                if (repeatCheckBox.Checked &&
                    MessageBox.Show("Warning: operation will overwrite existing slices", "Slice", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;

                this.Cursor = Cursors.WaitCursor;
                clone();
                changed = true;
                this.Cursor = Cursors.Arrow;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Clone", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //removes any existing slices when mass cloning
        private void clearExistingSlices()
        {
            Slice current;
            int delIndex = 0;

            foreach (TreeNode t in parent.Nodes)
            {
                current = t.Tag as Slice;
                if (current.end < start)
                    continue;
                delIndex = t.Index;
                break;
            }
            while (parent.Nodes.Count > delIndex)
                parent.Nodes[delIndex].Remove();
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
        private void clone()
        {
            cloneNode(start);

            if (repeatCheckBox.Checked)
            {
                long size = nodeData.size;
                long insertPosition = start;
                long totalSize = (parent.Tag as Slice).dataByteProvider.Length - 1;
                clearExistingSlices();
                while (insertPosition + size < totalSize)
                {
                    cloneNode(insertPosition);
                    insertPosition += size;
                }
            }
        }


    }
}
