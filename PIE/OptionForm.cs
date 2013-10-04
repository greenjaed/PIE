using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public partial class OptionForm : Form
    {
        private HexBox hexBox;
        public OptionForm()
        {
            InitializeComponent();
        }

        public OptionForm(HexBox hexBox)
        {
            InitializeComponent();
            this.hexBox = hexBox;
            initializeControls();
        }

        private void initializeControls()
        {
            fontTextBox.ForeColor = hexBox.ForeColor;
            fontTextBox.Font = hexBox.Font;
            fontTextBox.Text = hexBox.Font.Name + ", " + hexBox.Font.Size.ToString() + " pt";
            backColorButton.BackColor = hexBox.BackColor;
            charCheckBox.Checked = hexBox.StringViewVisible;
            columnCheckBox.Checked = hexBox.ColumnInfoVisible;
            lineCheckBox.Checked = hexBox.LineInfoVisible;
            hexCaseComboBox.SelectedIndex = hexBox.HexCasing == HexCasing.Lower ? 0 : 1;
            bytesMaskedTextBox.Text = hexBox.BytesPerLine.ToString();
            separatorCheckBox.Checked = hexBox.GroupSeparatorVisible;
            groupSizeMaskedTextBox.Text = hexBox.GroupSize.ToString();
            fontDialog1.Font = hexBox.Font;
            fontDialog1.Color = hexBox.ForeColor;
        }

        private void changeButton_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                fontTextBox.ForeColor = fontDialog1.Color;
                fontTextBox.Font = fontDialog1.Font;
                fontTextBox.Text = fontDialog1.Font.Name + ", " + fontDialog1.Font.Size.ToString() + " pt";
            }
        }

        private void backColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                backColorButton.BackColor = colorDialog1.Color;
                fontTextBox.BackColor = colorDialog1.Color;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            recordChanges();
            this.Close();
        }

        private void recordChanges()
        {
            hexBox.ForeColor = fontTextBox.ForeColor;
            hexBox.Font = fontTextBox.Font;
            hexBox.BackColor = backColorButton.BackColor;
            hexBox.StringViewVisible = charCheckBox.Checked;
            hexBox.ColumnInfoVisible = columnCheckBox.Checked;
            hexBox.LineInfoVisible = lineCheckBox.Checked;
            hexBox.HexCasing = hexCaseComboBox.SelectedIndex == 0 ? HexCasing.Lower : HexCasing.Upper;
            hexBox.BytesPerLine = int.Parse(bytesMaskedTextBox.Text);
            hexBox.GroupSeparatorVisible = separatorCheckBox.Checked;
            hexBox.GroupSize = int.Parse(groupSizeMaskedTextBox.Text);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
