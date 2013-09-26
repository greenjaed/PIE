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
        public IByteProvider source { get; set; }

        public SliceForm()
        {
            InitializeComponent();
        }

        public SliceForm(IByteProvider source)
        {
            InitializeComponent();
            this.source = source;
        }

        private void moveControls(int amount)
        {
            this.Width += amount;
            cancelButton.Location = new Point(cancelButton.Location.X + amount, cancelButton.Location.Y);
            sliceButton.Location = new Point(sliceButton.Location.X + amount, sliceButton.Location.Y);
        }

        private void AdvancedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AdvancedCheckBox.Checked)
            {
                moveControls(150);
                basicPanel.Visible = false;
                advancedPanel.Visible = true;
            }
            else
            {
                moveControls(-150);
                basicPanel.Visible = true;
                advancedPanel.Visible = false;
            }
        }

        private void sliceButton_Click(object sender, EventArgs e)
        {
            long start;
            long size;
            try
            {
                if (AdvancedCheckBox.Checked)
                {

                }
                else
                {
                    long end;
                    start = long.Parse(basicStartTextBox.Text, NumberStyles.HexNumber);
                    end = long.Parse(endTextBox.Text, NumberStyles.HexNumber);
                    if ((size = end - start) < 0)
                        throw new Exception("End address is smaller than start address");
                    if (start < 0 || end < 0)
                        throw new Exception("Negative address values");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void basicStartTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void endTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void advancedStartTextBox_Leave(object sender, EventArgs e)
        {

        }
    }
}
