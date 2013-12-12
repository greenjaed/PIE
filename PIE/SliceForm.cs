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
        private Slice dataSource;
        private long size;
        private int baseSize;

        public SliceForm()
        {
            InitializeComponent();
        }

        public SliceForm(TreeNode node) : base(node)
        {
            InitializeComponent();
            dataSource = node.Tag as Slice;
            okButton.Click -= okButton_Click;
            okButton.Click += new EventHandler(sliceButton_Click);
            okButton.Text = "Slice";
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
            Slice subslice = new Slice(dataSource, start, size);
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
                if (size > dataSource.size)
                    throw new ArgumentOutOfRangeException("Size");
                errorProvider1.SetError(sizeComboBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(sizeComboBox, ex.Message);
            }
        }

        private void sliceButton_Click(object sender, EventArgs e)
        {
            try
            {
                checkStart();
                if (!bySizeCheckBox.Checked)
                {
                    checkEnd();
                    size = 1 + end - start;
                }
                else
                {
                    if (errorProvider1.GetError(sizeComboBox) != "")
                        throw new Exception(errorProvider1.GetError(sizeComboBox));
                    if (size == 0)
                        throw new Exception("No size selected");
                    end = start + size - 1;
                }
                if (Slice.IsTaken(node, start, end))
                    throw new Exception(Properties.Resources.overlapString);
                this.Cursor = Cursors.WaitCursor;
                size = Math.Min(dataSource.size, size);
                createSlice();
                changed = true;
                this.Cursor = Cursors.Arrow;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Slice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
