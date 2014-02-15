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
        private Color shadowSelectionColor;
        private Color selectionForeColor;

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
            columnNumberCheckBox.Checked = hexBox.ColumnInfoVisible;
            groupSizeMaskedTextBox.Text = hexBox.GroupSize.ToString();
            selectionForeColor = hexBox.SelectionForeColor;
            shadowSelectionColor = hexBox.ShadowSelectionColor;
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
                    optionWriter.WriteLine("AddressColor=" + colorConverter.ConvertToString(addressColorButton.BackColor));
                    optionWriter.WriteLine("SelectionColor=" + colorConverter.ConvertToString(selectionColorButton.BackColor));
                    optionWriter.WriteLine("ShadowSelectionColor=" + colorConverter.ConvertToString(shadowSelectionColor));
                    optionWriter.WriteLine("SelectionForeColor=" + colorConverter.ConvertToString(selectionForeColor));
                    optionWriter.WriteLine("StringViewVisible=" + charCheckBox.Checked.ToString());
                    optionWriter.WriteLine("LineInfoVisible=" + lineCheckBox.Checked.ToString());
                    optionWriter.WriteLine("HexCasing=" + (hexCaseComboBox.SelectedIndex == 0 ? HexCasing.Lower.ToString() : HexCasing.Upper.ToString()));
                    optionWriter.WriteLine("BytesPerLine=" + bytesMaskedTextBox.Text);
                    optionWriter.WriteLine("ByteGrouping=" + byteGroupCheckBox.Checked.ToString());
                    optionWriter.WriteLine("ByteGroupSize=" + groupSizeMaskedTextBox.Text);
                    optionWriter.WriteLine("ColumnNumber=" + columnNumberCheckBox.Checked.ToString());
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
            hexBox.InfoForeColor = addressColorButton.BackColor;
            hexBox.SelectionBackColor = selectionColorButton.BackColor;
            hexBox.ShadowSelectionColor = shadowSelectionColor;
            hexBox.SelectionForeColor = selectionForeColor;
            hexBox.StringViewVisible = charCheckBox.Checked;
            hexBox.LineInfoVisible = lineCheckBox.Checked;
            hexBox.HexCasing = hexCaseComboBox.SelectedIndex == 0 ? HexCasing.Lower : HexCasing.Upper;
            hexBox.BytesPerLine = int.Parse(bytesMaskedTextBox.Text);
            hexBox.GroupSeparatorVisible = byteGroupCheckBox.Checked;
            hexBox.GroupSize = int.Parse(groupSizeMaskedTextBox.Text);
            hexBox.ColumnInfoVisible = columnNumberCheckBox.Checked;
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
                Color selectColor = colorDialog1.Color;
                selectionColorButton.BackColor = selectColor;
                shadowSelectionColor = Color.FromArgb(100, selectColor.R, selectColor.G, selectColor.B);

                //make the selection fore color the inverse of the selected color
                //OR
                //make the selection fore color black if color is light and white if color is dark
                //Luminance:
                //Y = 0.2126 R + 0.7152 G + 0.0722 B
                if (0.2126 * selectColor.R + 0.7152 * selectColor.G + 0.0722 * selectColor.B > 127.0)
                    selectionForeColor = Color.Black;
                else
                    selectionForeColor = Color.White;
            }
        }
    }
}
