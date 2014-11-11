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
        public TreeNode Node { get; protected set; }
        protected Slice nodeSlice;
        private Slice parentSlice;

        public ResizeForm()
        {
            InitializeComponent();
        }

        public ResizeForm(TreeNode node)
        {
            InitializeComponent();
            this.Node = node;
            nodeSlice = node.Tag as Slice;
            if (node.Parent != null)
            {
                parentSlice = node.Parent.Tag as Slice;
                start = parentSlice.Offset + nodeSlice.Start;
                end = parentSlice.Offset + nodeSlice.End;
                startTextBox.Text = start.ToString("X");
                endTextBox.Text = end.ToString("X");
            }
        }


        protected virtual void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (start < parentSlice.Start + parentSlice.Offset || start >= parentSlice.End + nodeSlice.Offset)
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
                if (end <= parentSlice.Start + parentSlice.Offset || end > parentSlice.End + nodeSlice.Offset)
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
            TreeNode parent = Node.Parent;
            int nodeIndex = Node.Index;
            bool valid;

            try
            {
                Node.Remove();
                checkValues(parentSlice.Offset);
                if (Node.Nodes.Count > 0 &&
                    MessageBox.Show("Warning: subslices may be resized or removed", "Resize", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;
                nodeSlice.Resize(Node, start, end);
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
                if (!parent.Nodes.Contains(Node))
                    parent.Nodes.Insert(nodeIndex, Node);
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
            if (Slice.IsTaken(Node, start, end))
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
                if (size > (Node.Tag as Slice).Size)
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
