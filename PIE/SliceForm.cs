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
            endTextBox.Validating -= base.endTextBox_Validating;
            endTextBox.Validating += new CancelEventHandler(this.endTextBox_Validating);
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

        private void sliceButton_Click(object sender, EventArgs e)
        {
            try
            {
                checkValues();
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

        protected override void endTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                end = long.Parse(endTextBox.Text, NumberStyles.HexNumber) - (node.Tag as Slice).lastStart;
                if (end <= dataSource.lastStart || end > dataSource.end + dataSource.lastStart)
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
