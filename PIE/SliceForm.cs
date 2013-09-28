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

        private void AdvancedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            endLabel.Visible = endTextBox.Visible = !AdvancedCheckBox.Checked;
            sizeLabel.Visible = sizeComboBox.Visible = AdvancedCheckBox.Checked;
            bytesLabel.Visible = bytesComboBox.Visible = AdvancedCheckBox.Checked;
            repeatCheckBox.Visible = AdvancedCheckBox.Checked;
            if (AdvancedCheckBox.Checked && end == 0)
                invalidLabel2.Visible = true;
            else if (size == 0)
                invalidLabel2.Visible = true;
        }

        private void sliceButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (invalidLabel1.Visible)
                    throw new Exception(toolTip1.GetToolTip(invalidLabel1));
                if (invalidLabel2.Visible)
                    throw new Exception(toolTip1.GetToolTip(invalidLabel2));
                if (!AdvancedCheckBox.Checked)
                    size = 1 + end - start;
                this.Cursor = Cursors.WaitCursor;
                slice();
                this.Cursor = Cursors.Arrow;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Slice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void startTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                start = validateString(startTextBox.Text);
                invalidLabel1.Visible = false;
                toolTip1.SetToolTip(invalidLabel1, "");
            }
            catch (Exception ex)
            {
                invalidLabel1.Visible = true;
                toolTip1.SetToolTip(invalidLabel1, ex.Message);
                start = -1;
            }
        }

        private long validateString(string toCheck)
        {
            long assign = long.Parse(toCheck, NumberStyles.HexNumber);
            if (assign < 0 || assign > dataSource.dataByteProvider.Length - 1)
                throw new Exception("Address out of range");
            if (!AdvancedCheckBox.Checked)
            {
                if (end > 0 && start >= 0 && start >= end)
                    throw new Exception("End address must be greater than start address");
            }
            return assign;
        }

        private void endTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                if ((end = validateString(endTextBox.Text)) == 0)
                    throw new Exception("End address must be greater than zero");
                invalidLabel2.Visible = false;
                toolTip1.SetToolTip(invalidLabel2, "");
            }
            catch (Exception ex)
            {
                invalidLabel2.Visible = true;
                toolTip1.SetToolTip(invalidLabel2, ex.Message);
                end = 0;
            }

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

        private void createNode(long position, int name)
        {
            TreeNode subnode = new TreeNode();
            subnode.Name = (Owner as PIEForm).uniqueID.ToString();
            subnode.Text = "block " + name.ToString("X");
            Data subslice = new Data(dataSource, position, size);
            subnode.Tag = subslice;
            dataNode.Nodes.Add(subnode);
        }

        private void sizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                baseSize = int.Parse(sizeComboBox.Text);
                calculateSize();
                invalidLabel2.Visible = false;
                toolTip1.SetToolTip(invalidLabel2, "");
            }
            catch (Exception ex)
            {
                invalidLabel2.Visible = true;
                toolTip1.SetToolTip(invalidLabel2, ex.Message);
            }
        }

        private void bytesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculateSize();
        }

        private void calculateSize()
        {
            if (bytesComboBox.SelectedIndex == -1)
                bytesComboBox.SelectedIndex = 0;
            size = baseSize << (10 * bytesComboBox.SelectedIndex);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
