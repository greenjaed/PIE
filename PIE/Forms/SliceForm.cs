using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace PIE
{
    public partial class SliceForm : ResizeForm
    {
        private long OffsetEnd;

        public SliceForm()
        {
            InitializeComponent();
        }

        public SliceForm(TreeNode node) : base(node)
        {
            InitializeComponent();
            NodeSlice = node.Tag as Slice;
            okButton.Click -= okButton_Click;
            okButton.Click += new EventHandler(sliceButton_Click);
            endTextBox.Validating -= base.endTextBox_Validating;
            endTextBox.Validating += new CancelEventHandler(this.endTextBox_Validating);
            startTextBox.Validating -= base.startTextBox_Validating;
            startTextBox.Validating += new CancelEventHandler(startTextBox_Validating);
            okButton.Text = "Slice";

            OffsetEnd = NodeSlice.Offset + NodeSlice.End;
            Start = NodeSlice.Offset;
            End = OffsetEnd;
            endTextBox.Text = End.ToString("X");
            startTextBox.Text = Start.ToString("X");
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

        private void createSlice()
        {
            TreeNode subnode = new TreeNode();
            String nodeText = string.IsNullOrEmpty(nameTextBox.Text) ? "new slice" : nameTextBox.Text;
            subnode.Name = (Owner as PIEForm).UniqueID.ToString();
            subnode.Text = nodeText;
            Slice subslice = new Slice(NodeSlice, Start, Size);
            subnode.Tag = subslice;
            Node.Nodes.Add(subnode);
        }

        private void clearExistingSlices()
        {
            Slice current;
            int delIndex = 0;
            
            foreach (TreeNode t in Node.Nodes)
            {
                current = t.Tag as Slice;
                if (current.End < Start)
                {
                    continue;
                }
                delIndex = t.Index;
                break;
            }
            while (Node.Nodes.Count > delIndex)
            {
                Node.Nodes[delIndex].Remove();
            }
        }

        private void sliceButton_Click(object sender, EventArgs e)
        {
            try
            {
                checkValues(NodeSlice.Offset);
                this.Cursor = Cursors.WaitCursor;
                Size = Math.Min(NodeSlice.Size, Size);
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
                Start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (Start < NodeSlice.Offset || Start >= OffsetEnd)
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

        protected new void endTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                End = long.Parse(endTextBox.Text, NumberStyles.HexNumber);
                if (End <= NodeSlice.Offset || End > OffsetEnd)
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

    }
}
