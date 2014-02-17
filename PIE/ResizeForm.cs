using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace PIE
{
    public partial class ResizeForm : Form
    {
        protected long start;
        protected long end;
        protected long size;
        protected int baseSize;
        public TreeNode node { get; protected set; }
        protected Slice nodeSlice;
        private Slice parentSlice;

        public ResizeForm()
        {
            InitializeComponent();
        }

        public ResizeForm(TreeNode node)
        {
            InitializeComponent();
            this.node = node;
            nodeSlice = node.Tag as Slice;
            if (node.Parent != null)
            {
                parentSlice = node.Parent.Tag as Slice;
                start = parentSlice.lastStart + nodeSlice.start;
                end = parentSlice.lastStart + nodeSlice.end;
                startTextBox.Text = start.ToString("X");
                endTextBox.Text = end.ToString("X");
            }
        }


        protected virtual void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (start < parentSlice.start + parentSlice.lastStart || start >= parentSlice.end + nodeSlice.lastStart)
                    throw new ArgumentOutOfRangeException("Start");
                errorProvider1.SetError(startTextBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(startTextBox, ex.Message);
            }
        }

        protected virtual void endTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                end = long.Parse(endTextBox.Text, NumberStyles.HexNumber);
                if (end <= parentSlice.start + parentSlice.lastStart || end > parentSlice.end + nodeSlice.lastStart)
                    throw new ArgumentOutOfRangeException("End");
                if (end < start)
                    throw new Exception("End address must be greater than start address");

                errorProvider1.SetError(endTextBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(endTextBox, ex.Message);
            }

        }

        protected virtual void okButton_Click(object sender, EventArgs e)
        {
            TreeNode parent = node.Parent;
            int nodeIndex = node.Index;
            bool valid;

            try
            {
                node.Remove();
                checkValues(parentSlice.lastStart);
                if (node.Nodes.Count > 0 &&
                    MessageBox.Show("Warning: subslices may be resized or removed", "Resize", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;
                nodeSlice.Resize(node, start, end);
                nodeSlice.Invalidate();
                valid = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Resize", MessageBoxButtons.OK, MessageBoxIcon.Error);
                valid = false;
            }
            finally
            {
                if (!parent.Nodes.Contains(node))
                    parent.Nodes.Insert(nodeIndex, node);
            }

            if (valid)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void checkField(Control control)
        {
            if (errorProvider1.GetError(control) != "")
                throw new Exception(errorProvider1.GetError(control));
            if (control.Text == "")
                throw new Exception("No number specified");
        }

        protected void checkValues(long offset)
        {
            checkField(startTextBox);
            start -= offset;
            if (!bySizeCheckBox.Checked)
            {
                checkField(endTextBox);
                end -= offset;
                size = 1 + end - start;
            }
            else
            {
                checkField(sizeComboBox);
                end = start + size - 1;
            }
            if (Slice.IsTaken(node, start, end))
                throw new Exception(Properties.Resources.overlapString);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void bySizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            endLabel.Visible = endTextBox.Visible = !bySizeCheckBox.Checked;
            sizeLabel.Visible = sizeComboBox.Visible = bySizeCheckBox.Checked;
            bytesLabel.Visible = bytesComboBox.Visible = bySizeCheckBox.Checked;
        }

        private void bytesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculateSize();
        }

        private void sizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(sizeComboBox, "");
        }

        private void sizeComboBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                baseSize = int.Parse(sizeComboBox.Text);
                calculateSize();
                if (size > (node.Tag as Slice).size)
                    throw new ArgumentOutOfRangeException("Size");
                errorProvider1.SetError(sizeComboBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(sizeComboBox, ex.Message);
            }
        }

        //calculates the size of the slice
        private void calculateSize()
        {
            if (bytesComboBox.SelectedIndex == -1)
                bytesComboBox.SelectedIndex = 0;
            else
                size = baseSize << (10 * bytesComboBox.SelectedIndex);
        }

    }
}
