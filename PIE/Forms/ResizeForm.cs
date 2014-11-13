using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace PIE
{
    public partial class ResizeForm : Form
    {
        protected long Start;
        protected long End;
        protected long Size;
        protected int BaseSize;
        public TreeNode Node { get; protected set; }
        protected Slice NodeSlice;
        private Slice ParentSlice;

        public ResizeForm()
        {
            InitializeComponent();
        }

        public ResizeForm(TreeNode node)
        {
            InitializeComponent();
            this.Node = node;
            NodeSlice = node.Tag as Slice;
            if (node.Parent != null)
            {
                ParentSlice = node.Parent.Tag as Slice;
                Start = ParentSlice.Offset + NodeSlice.Start;
                End = ParentSlice.Offset + NodeSlice.End;
                startTextBox.Text = Start.ToString("X");
                endTextBox.Text = End.ToString("X");
            }
        }


        protected virtual void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                Start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (Start < ParentSlice.Start + ParentSlice.Offset || Start >= ParentSlice.End + NodeSlice.Offset)
                {
                    throw new ArgumentOutOfRangeException("Start");
                }
                errorProvider1.SetError(startTextBox, string.Empty);
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
                End = long.Parse(endTextBox.Text, NumberStyles.HexNumber);
                if (End <= ParentSlice.Start + ParentSlice.Offset || End > ParentSlice.End + NodeSlice.Offset)
                {
                    throw new ArgumentOutOfRangeException("End");
                }
                if (End < Start)
                {
                    throw new Exception("End address must be greater than start address");
                }

                errorProvider1.SetError(endTextBox, string.Empty);
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
            bool inputsAreValid;

            try
            {
                Node.Remove();
                checkValues(ParentSlice.Offset);
                if (Node.Nodes.Count > 0 &&
                    MessageBox.Show("Warning: subslices may be resized or removed",
                                "Resize",
                                MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return;
                }
                NodeSlice.Resize(Node, Start, End);
                NodeSlice.Invalidate();
                inputsAreValid = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Resize", MessageBoxButtons.OK, MessageBoxIcon.Error);
                inputsAreValid = false;
            }
            finally
            {
                if (!parent.Nodes.Contains(Node))
                {
                    parent.Nodes.Insert(nodeIndex, Node);
                }
            }

            if (inputsAreValid)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void checkField(Control control)
        {
            if (!string.IsNullOrEmpty(errorProvider1.GetError(control)))
            {
                throw new Exception(errorProvider1.GetError(control));
            }
            if (string.IsNullOrEmpty(control.Text))
            {
                throw new Exception("No number specified");
            }
        }

        protected void checkValues(long offset)
        {
            checkField(startTextBox);
            Start -= offset;
            if (!bySizeCheckBox.Checked)
            {
                checkField(endTextBox);
                End -= offset;
                Size = 1 + End - Start;
            }
            else
            {
                checkField(sizeComboBox);
                End = Start + Size - 1;
            }
            if (Slice.IsTaken(Node, Start, End))
            {
                throw new Exception(Properties.Resources.overlapString);
            }
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
            errorProvider1.SetError(sizeComboBox, string.Empty);
        }

        private void sizeComboBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                BaseSize = int.Parse(sizeComboBox.Text);
                calculateSize();
                if (Size > (Node.Tag as Slice).Size)
                {
                    throw new ArgumentOutOfRangeException("Size");
                }
                errorProvider1.SetError(sizeComboBox, string.Empty);
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
            {
                bytesComboBox.SelectedIndex = 0;
            }
            else
            {
                Size = BaseSize << (10 * bytesComboBox.SelectedIndex);
            }
        }
    }
}
