using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public partial class OptionForm : Form
    {
        private HexBox hexBox;
        private FontConverter fontConverter;

        public OptionForm()
        {
            InitializeComponent();
        }

        public OptionForm(HexBox hexBox)
        {
            InitializeComponent();
            this.hexBox = hexBox;
            fontConverter = new FontConverter();
            initializeControls();
        }

        private void initializeControls()
        {
            fontTextBox.ForeColor = hexBox.ForeColor;
            fontTextBox.BackColor = hexBox.BackColor;
            fontTextBox.Font = hexBox.Font;
            fontTextBox.Text = fontConverter.ConvertToString(hexBox.Font);
            backColorButton.BackColor = hexBox.BackColor;
            charCheckBox.Checked = hexBox.StringViewVisible;
            lineCheckBox.Checked = hexBox.LineInfoVisible;
            hexCaseComboBox.SelectedIndex = hexBox.HexCasing == HexCasing.Lower ? 0 : 1;
            bytesMaskedTextBox.Text = hexBox.BytesPerLine.ToString();
            addressColorButton.BackColor = hexBox.InfoForeColor;
            selectionColorButton.BackColor = hexBox.SelectionBackColor;
            byteGroupCheckBox.Checked = groupSizeMaskedTextBox.Enabled = hexBox.GroupSeparatorVisible;
            columnIndexCheckBox.Checked = hexBox.ColumnInfoVisible;
        }

        private void changeButton_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = fontTextBox.Font;
            fontDialog1.Color = fontTextBox.ForeColor;

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                fontTextBox.ForeColor = fontDialog1.Color;
                fontTextBox.Font = fontDialog1.Font;
                fontTextBox.Text = fontConverter.ConvertToString(fontDialog1.Font);
            }
        }

        private void backColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = backColorButton.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                backColorButton.BackColor = colorDialog1.Color;
                fontTextBox.BackColor = colorDialog1.Color;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            saveChanges();
            applyChanges();
            //save the changes to a configuration file
            this.Close();
        }

        private void saveChanges()
        {
            ColorConverter colorConverter = new ColorConverter();
            try
            {
                using (StreamWriter optionWriter = new StreamWriter(Application.StartupPath + Properties.Resources.configFileString))
                {
                    optionWriter.WriteLine("[HexBox]");
                    optionWriter.WriteLine("ForeColor=" + colorConverter.ConvertToString(fontTextBox.ForeColor));
                    optionWriter.WriteLine("Font=" + fontConverter.ConvertToString(fontTextBox.Font));
                    optionWriter.WriteLine("BackColor=" + colorConverter.ConvertToString(backColorButton.BackColor));
                    optionWriter.WriteLine("StringViewVisible=" + charCheckBox.Checked.ToString());
                    optionWriter.WriteLine("LineInfoVisible=" + lineCheckBox.Checked.ToString());
                    optionWriter.WriteLine("HexCasing=" + (hexCaseComboBox.SelectedIndex == 0 ? HexCasing.Lower.ToString() : HexCasing.Upper.ToString()));
                    optionWriter.WriteLine("BytesPerLine=" + bytesMaskedTextBox.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void applyChanges()
        {
            hexBox.ForeColor = fontTextBox.ForeColor;
            hexBox.Font = fontTextBox.Font;
            hexBox.BackColor = fontTextBox.BackColor;
            hexBox.StringViewVisible = charCheckBox.Checked;
            hexBox.LineInfoVisible = lineCheckBox.Checked;
            hexBox.HexCasing = hexCaseComboBox.SelectedIndex == 0 ? HexCasing.Lower : HexCasing.Upper;
            hexBox.BytesPerLine = int.Parse(bytesMaskedTextBox.Text);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void invertButton_Click(object sender, EventArgs e)
        {
            Color temp = fontTextBox.ForeColor;
            fontTextBox.ForeColor = fontTextBox.BackColor;
            fontTextBox.BackColor = temp;
            backColorButton.BackColor = temp;
        }

        private void byteGroupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            groupSizeMaskedTextBox.Enabled = byteGroupCheckBox.Checked;
        }

        private void addressColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = addressColorButton.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
                addressColorButton.BackColor = colorDialog1.Color;

        }

        private void selectionColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = selectionColorButton.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                selectionColorButton.BackColor = colorDialog1.Color;
                //make the shadowselectioncolor a lighter shade of the selected color
                //make the selection fore color the inverse of the selected color
                //OR
                //make the selection fore color black if color is light and white if color is dark
            }
        }
    }
}
