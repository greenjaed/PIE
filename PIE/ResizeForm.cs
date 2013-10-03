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
        long start;
        long end;
        TreeNode toResize;

        public ResizeForm()
        {
            InitializeComponent();
        }

        public ResizeForm(TreeNode toResize)
        {
            InitializeComponent();
            this.toResize = toResize;
        }


        private void startTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                start = long.Parse(startTextBox.Text, NumberStyles.HexNumber);
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
                if (end == 0)
                    throw new Exception("End address must be greater than 0");
                if (end < start)
                    throw new Exception("End address must be greater than start address");

                errorProvider1.SetError(endTextBox, "");
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(endTextBox, ex.Message);
            }

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Data wrongSize = toResize.Tag as Data;
            TreeNode parent = toResize.Parent;
            int nodeIndex = toResize.Index;
            bool valid;

            try
            {
                if (errorProvider1.GetError(startTextBox) != "")
                    throw new Exception(errorProvider1.GetError(startTextBox));
                if (errorProvider1.GetError(endTextBox) != "")
                    throw new Exception(errorProvider1.GetError(endTextBox));
                toResize.Remove();
                if (Data.IsTaken(parent, start, end))
                    throw new Exception(Properties.Resources.overlapString);
                wrongSize.resize(start, end);
                valid = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Resize", MessageBoxButtons.OK, MessageBoxIcon.Error);
                valid = false;
            }
            finally
            {
                parent.Nodes.Insert(nodeIndex, toResize);
            }

            if (valid)
                this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
