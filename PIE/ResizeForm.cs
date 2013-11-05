using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PIE
{
    public partial class ResizeForm : Form
    {
        protected long start;
        protected long end;
        protected TreeNode node;
        public bool changed { get; protected set; }

        public ResizeForm()
        {
            InitializeComponent();
        }

        public ResizeForm(TreeNode node)
        {
            InitializeComponent();
            this.node = node;
            Slice nodeData = node.Tag as Slice;
            startTextBox.Text = nodeData.start.ToString("X");
            endTextBox.Text = nodeData.end.ToString("X");
            start = nodeData.start;
            end = nodeData.end;
        }


        private void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
                if (start > (node.Tag as Slice).end)
                    throw new ArgumentOutOfRangeException("Start");
                errorProvider1.SetError(startTextBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(startTextBox, ex.Message);
            }
        }

        private void endTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                end = long.Parse(endTextBox.Text, NumberStyles.HexNumber);
                if (end == 0 || end > (node.Tag as Slice).end)
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
            Slice wrongSize = node.Tag as Slice;
            TreeNode parent = node.Parent;
            int nodeIndex = node.Index;
            bool valid;

            try
            {
                checkStart();
                checkEnd();
                node.Remove();
                if (Slice.IsTaken(parent, start, end))
                    throw new Exception(Properties.Resources.overlapString);
                if (node.Nodes.Count > 0 &&
                    MessageBox.Show("Warning: subslices may be resized or removed", "Resize", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;
                wrongSize.Resize(node, start, end);
                valid = true;
                changed = true;
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
                this.Close();
        }

        protected void checkEnd()
        {
            if (errorProvider1.GetError(endTextBox) != "")
                throw new Exception(errorProvider1.GetError(endTextBox));
            if (endTextBox.Text == "")
                throw new Exception("No end address specified");
        }

        protected void checkStart()
        {
            if (errorProvider1.GetError(startTextBox) != "")
                throw new Exception(errorProvider1.GetError(startTextBox));
            if (startTextBox.Text == "")
                throw new Exception("No start address specified");
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
