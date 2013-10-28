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

        private void calculateSize()
        {
            if (bytesComboBox.SelectedIndex == -1)
                bytesComboBox.SelectedIndex = 0;
            else
                size = baseSize << (10 * bytesComboBox.SelectedIndex);
        }

        private void createNode(long position, int name)
        {
            TreeNode subnode = new TreeNode();
            subnode.Name = (Owner as PIEForm).uniqueID.ToString();
            subnode.Text = "block " + name.ToString("X");
            Slice subslice = new Slice(dataSource, position, size);
            subnode.Tag = subslice;
            node.Nodes.Add(subnode);
        }

        private void slice()
        {
            int sliceCounter = 0;
            createNode(start, sliceCounter);
            ++sliceCounter;

            if (AdvancedCheckBox.Checked && repeatCheckBox.Checked)
            {
                long insertPosition = start + size;
                clearExistingSlices();
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
                if (!AdvancedCheckBox.Checked)
                {
                    checkEnd();
                    size = 1 + end - start;
                    if (Slice.IsTaken(node, start, end))
                        throw new Exception(Properties.Resources.overlapString);
                }
                else
                {
                    if (errorProvider1.GetError(sizeComboBox) != "")
                        throw new Exception(errorProvider1.GetError(sizeComboBox));
                    if (size == 0)
                        throw new Exception("No size selected");
                    end = start + size - 1;
                    if (repeatCheckBox.Checked)
                    {
                        if (MessageBox.Show("Warning: operation will overwrite existing slices", "Slice", MessageBoxButtons.OKCancel) ==
                            DialogResult.Cancel)
                            return;
                    }
                    else if (Slice.IsTaken(node, start, end))
                        throw new Exception(Properties.Resources.overlapString);
                }
                this.Cursor = Cursors.WaitCursor;
                size = Math.Min(dataSource.size, size);
                slice();
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
