using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Be.Windows.Forms;

namespace PIE
{
    public partial class SliceForm : ResizeForm
    {
        private long offsetEnd;

        public SliceForm()
        {
            InitializeComponent();
        }

        public SliceForm(TreeNode node) : base(node)
        {
            InitializeComponent();
            nodeSlice = node.Tag as Slice;
            okButton.Click -= okButton_Click;
            okButton.Click += new EventHandler(sliceButton_Click);
            endTextBox.Validating -= base.endTextBox_Validating;
            endTextBox.Validating += new CancelEventHandler(this.endTextBox_Validating);
            startTextBox.Validating -= base.startTextBox_Validating;
            startTextBox.Validating += new CancelEventHandler(startTextBox_Validating);
            okButton.Text = "Slice";

            offsetEnd = nodeSlice.lastStart + nodeSlice.end;
            start = nodeSlice.lastStart;
            end = offsetEnd;
            endTextBox.Text = end.ToString("X");
            startTextBox.Text = start.ToString("X");
        }

        //calculates the size of the slice
        private void calculateSize()
        {
            if (bytesComboBox.SelectedIndex == -1)
                bytesComboBox.SelectedIndex = 0;
            else
                size = baseSize << (10 * bytesComboBox.SelectedIndex);
        }

        private void createSlice()
        {
            TreeNode subnode = new TreeNode();
            String nodeText = nameTextBox.Text == "" ? "new slice" : nameTextBox.Text;
            subnode.Name = (Owner as PIEForm).uniqueID.ToString();
            subnode.Text = nodeText;
            Slice subslice = new Slice(nodeSlice, start, size);
            subslice.notes = notesTextBox.Text;
            subnode.Tag = subslice;
            node.Nodes.Add(subnode);
        }

        private void clearExistingSlices()
        {
            Slice current;
            int delIndex = 0;
            
            foreach (TreeNode t in node.Nodes)
            {
                current = t.Tag as Slice;
                if (current.end < start)
                    continue;
                delIndex = t.Index;
                break;
            }
            while (node.Nodes.Count > delIndex)
                node.Nodes[delIndex].Remove();
        }

        private void sliceButton_Click(object sender, EventArgs e)
        {
            try
            {
                checkValues(nodeSlice.lastStart);
                this.Cursor = Cursors.WaitCursor;
                size = Math.Min(nodeSlice.size, size);
                createSlice();
                this.Cursor = Cursors.Arrow;
                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Slice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected new void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (start < nodeSlice.lastStart || start >= offsetEnd)
                    throw new ArgumentOutOfRangeException("Start");
                errorProvider1.SetError(startTextBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(startTextBox, ex.Message);
            }
        }

        protected new void endTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                end = long.Parse(endTextBox.Text, NumberStyles.HexNumber);
                if (end <= nodeSlice.lastStart || end > offsetEnd)
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

    }
}
