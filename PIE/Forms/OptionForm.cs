using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public partial class OptionForm : Form
    {
        private HexBox HexBox;
        private FontConverter FontConverter;
        private Color ShadowSelectionColor;
        private Color SelectionForeColor;

        public OptionForm()
        {
            InitializeComponent();
        }

        public OptionForm(HexBox hexBox)
        {
            InitializeComponent();
            this.HexBox = hexBox;
            FontConverter = new FontConverter();
            initializeControls();
        }

        private void initializeControls()
        {
            fontTextBox.ForeColor = HexBox.ForeColor;
            fontTextBox.BackColor = HexBox.BackColor;
            fontTextBox.Font = HexBox.Font;
            fontTextBox.Text = FontConverter.ConvertToString(HexBox.Font);
            backColorButton.BackColor = HexBox.BackColor;
            charCheckBox.Checked = HexBox.StringViewVisible;
            lineCheckBox.Checked = HexBox.LineInfoVisible;
            hexCaseComboBox.SelectedIndex = HexBox.HexCasing == HexCasing.Lower ? 0 : 1;
            bytesMaskedTextBox.Text = HexBox.BytesPerLine.ToString();
            addressColorButton.BackColor = HexBox.InfoForeColor;
            selectionColorButton.BackColor = HexBox.SelectionBackColor;
            byteGroupCheckBox.Checked = groupSizeMaskedTextBox.Enabled = HexBox.GroupSeparatorVisible;
            columnNumberCheckBox.Checked = HexBox.ColumnInfoVisible;
            groupSizeMaskedTextBox.Text = HexBox.GroupSize.ToString();
            SelectionForeColor = HexBox.SelectionForeColor;
            ShadowSelectionColor = HexBox.ShadowSelectionColor;
        }

        private void changeButton_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = fontTextBox.Font;
            fontDialog1.Color = fontTextBox.ForeColor;

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                fontTextBox.ForeColor = fontDialog1.Color;
                fontTextBox.Font = fontDialog1.Font;
                fontTextBox.Text = FontConverter.ConvertToString(fontDialog1.Font);
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
                    optionWriter.WriteLine("Font=" + FontConverter.ConvertToString(fontTextBox.Font));
                    optionWriter.WriteLine("BackColor=" + colorConverter.ConvertToString(backColorButton.BackColor));
                    optionWriter.WriteLine("AddressColor=" + colorConverter.ConvertToString(addressColorButton.BackColor));
                    optionWriter.WriteLine("SelectionColor=" + colorConverter.ConvertToString(selectionColorButton.BackColor));
                    optionWriter.WriteLine("ShadowSelectionColor=" + colorConverter.ConvertToString(ShadowSelectionColor));
                    optionWriter.WriteLine("SelectionForeColor=" + colorConverter.ConvertToString(SelectionForeColor));
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
            HexBox.ForeColor = fontTextBox.ForeColor;
            HexBox.Font = fontTextBox.Font;
            HexBox.BackColor = fontTextBox.BackColor;
            HexBox.InfoForeColor = addressColorButton.BackColor;
            HexBox.SelectionBackColor = selectionColorButton.BackColor;
            HexBox.ShadowSelectionColor = ShadowSelectionColor;
            HexBox.SelectionForeColor = SelectionForeColor;
            HexBox.StringViewVisible = charCheckBox.Checked;
            HexBox.LineInfoVisible = lineCheckBox.Checked;
            HexBox.HexCasing = hexCaseComboBox.SelectedIndex == 0 ? HexCasing.Lower : HexCasing.Upper;
            HexBox.BytesPerLine = int.Parse(bytesMaskedTextBox.Text);
            HexBox.GroupSeparatorVisible = byteGroupCheckBox.Checked;
            HexBox.GroupSize = int.Parse(groupSizeMaskedTextBox.Text);
            HexBox.ColumnInfoVisible = columnNumberCheckBox.Checked;
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
            {
                addressColorButton.BackColor = colorDialog1.Color;
            }
        }

        private void selectionColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = selectionColorButton.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color selectColor = colorDialog1.Color;
                selectionColorButton.BackColor = selectColor;
                ShadowSelectionColor = Color.FromArgb(100, selectColor.R, selectColor.G, selectColor.B);

                //make the selection fore color the inverse of the selected color
                //OR
                //make the selection fore color black if color is light and white if color is dark
                //Luminance:
                //Y = 0.2126 R + 0.7152 G + 0.0722 B
                if (0.2126 * selectColor.R + 0.7152 * selectColor.G + 0.0722 * selectColor.B > 127.0)
                {
                    SelectionForeColor = Color.Black;
                }
                else
                {
                    SelectionForeColor = Color.White;
                }
            }
        }
    }
}
