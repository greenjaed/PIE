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
    public partial class SliceForm : Form
    {
        private Data dataSource;
        public TreeNode dataNode { get; set; }
        private long start;
        private long end;
        private long size;
        private int baseSize;

        public SliceForm()
        {
            InitializeComponent();
            start = -1;
        }

        public SliceForm(TreeNode dataNode)
        {
            InitializeComponent();
            this.dataNode = dataNode;
            dataSource = dataNode.Tag as Data;
            start = -1;
        }

        private void calculateSize()
        {
            if (bytesComboBox.SelectedIndex == -1)
                bytesComboBox.SelectedIndex = 0;
            size = baseSize << (10 * bytesComboBox.SelectedIndex);
        }

        private void createNode(long position, int name)
        {
            TreeNode subnode = new TreeNode();
            subnode.Name = (Owner as PIEForm).uniqueID.ToString();
            subnode.Text = "block " + name.ToString("X");
            Data subslice = new Data(dataSource, position, size);
            subnode.Tag = subslice;
            dataNode.Nodes.Add(subnode);
        }

        private void slice()
        {
            int sliceCounter = 0;
            createNode(start, sliceCounter);
            ++sliceCounter;

            if (AdvancedCheckBox.Checked && repeatCheckBox.Checked)
            {
                long insertPosition = start + size;
                while (insertPosition + size < dataSource.dataByteProvider.Length - 1)
                {
                    createNode(insertPosition, sliceCounter);
                    ++sliceCounter;
                    insertPosition += size;
                }

                if ((size = (dataSource.dataByteProvider.Length - 1) - insertPosition) > 0)
                    createNode(insertPosition, sliceCounter);
            }
        }

        private long validateString(string toCheck)
        {
            long assign = long.Parse(toCheck, NumberStyles.HexNumber);
            if (assign < 0 || assign > dataSource.dataByteProvider.Length - 1)
                throw new Exception("Address out of range");
            return assign;
        }

        private void AdvancedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            endLabel.Visible = endTextBox.Visible = !AdvancedCheckBox.Checked;
            sizeLabel.Visible = sizeComboBox.Visible = AdvancedCheckBox.Checked;
            bytesLabel.Visible = bytesComboBox.Visible = AdvancedCheckBox.Checked;
            repeatCheckBox.Visible = AdvancedCheckBox.Checked;
        }

        private void bytesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculateSize();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void endTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if ((end = validateString(endTextBox.Text)) == 0)
                    throw new Exception("End address must be greater than zero");
                if (start >= end)
                    throw new Exception("End address must be greater than start address");
                errorProvider1.SetError(endTextBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(endTextBox, ex.Message);
            }
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
                if (errorProvider1.GetError(startTextBox) != "")
                    throw new Exception(errorProvider1.GetError(startTextBox));
                if (errorProvider1.GetError(endTextBox) != "")
                    throw new Exception(errorProvider1.GetError(endTextBox));
                if (errorProvider1.GetError(sizeComboBox) != "")
                    throw new Exception(errorProvider1.GetError(sizeComboBox));
                if (!AdvancedCheckBox.Checked)
                {
                    size = 1 + end - start;
                    if (Data.IsTaken(dataNode, start, end))
                        throw new Exception(Properties.Resources.overlapString);
                }
                this.Cursor = Cursors.WaitCursor;
                size = Math.Min(dataSource.dataByteProvider.Length, size);
                slice();
                this.Cursor = Cursors.Arrow;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Slice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                start = validateString(startTextBox.Text);
                errorProvider1.SetError(startTextBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(startTextBox, ex.Message);
            }
        }
    }
}
