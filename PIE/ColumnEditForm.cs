using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PIE
{
    public partial class ColumnEditForm : Form
    {
        static string[] bitSelect = { "8", "16", "32", "64" };
        public DataInfo column { get; private set; }

        public ColumnEditForm()
        {
            InitializeComponent();
        }

        public ColumnEditForm(DataInfo init)
        {
            InitializeComponent();
            nameTextBox.Text = init.name;
            typeComboBox.Text = init.dataType;
            sizeComboBox.Text = init.size.ToString();
            if (init.intFormat == IntFormat.Signed)
                signedRadioButton.Checked = true;
            else if (init.intFormat == IntFormat.Hex)
                hexRadioButton.Checked = true;
            else
                noneRadioButton.Checked = true;
            fractionNumericUpDown.Value = init.fraction;
            fractionNumericUpDown.Maximum = init.size;
            if (init.dataType == "String" || init.dataType == "Floating Point")
                optionsGroupBox.Enabled = false;
            else if (init.dataType == "Integer")
                fractionLabel.Enabled = fractionNumericUpDown.Enabled = false;
            else
                hexRadioButton.Enabled = false;

        }

        private void resetControls()
        {
            sizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sizeLabel.Text = "Size (bits)";
            sizeComboBox.Items.Clear();
            optionsGroupBox.Enabled = true;
            hexRadioButton.Enabled = false;
            fractionLabel.Enabled = fractionNumericUpDown.Enabled = false;
            noneRadioButton.Checked = true;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetControls();
            switch (typeComboBox.SelectedIndex)
            {
                case 0:
                    sizeComboBox.Items.AddRange(bitSelect);
                    hexRadioButton.Enabled = true;
                    break;
                case 1:
                    sizeComboBox.Items.AddRange(bitSelect);
                    sizeComboBox.Items.RemoveAt(0);
                    optionsGroupBox.Enabled = false;
                    break;
                case 2:
                    sizeComboBox.Items.AddRange(bitSelect);
                    break;
                case 3:
                    optionsGroupBox.Enabled = false;
                    sizeLabel.Text = "Size (bytes)";
                    sizeComboBox.DropDownStyle = ComboBoxStyle.DropDown;
                    for (int i = 1; i <= 8; ++i)
                        sizeComboBox.Items.Add(i.ToString());
                    break;
            }
            sizeComboBox.Enabled = true;
        }

        private void sizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex == 2)
            {
                fractionLabel.Enabled = fractionNumericUpDown.Enabled = true;
                fractionNumericUpDown.Maximum = int.Parse(sizeComboBox.Text);
                if (signedRadioButton.Checked)
                    --fractionNumericUpDown.Maximum;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            column = new DataInfo();
            column.name = nameTextBox.Text;
            column.dataType = typeComboBox.Text;
            if (!int.TryParse(sizeComboBox.Text, out column.size))
            {
                MessageBox.Show("Size is an invalid value", "Error");
                return;
            }
            column.size = int.Parse(sizeComboBox.Text);
            if (signedRadioButton.Checked)
                column.intFormat = IntFormat.Signed;
            else if (hexRadioButton.Checked)
                column.intFormat = IntFormat.Hex;
            else
                column.intFormat = IntFormat.None;
            column.fraction = (int) fractionNumericUpDown.Value;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void sizeComboBox_Validating(object sender, CancelEventArgs e)
        {
            int test;
            if (typeComboBox.SelectedIndex == 3)
            {
                if (!int.TryParse(sizeComboBox.Text, out test))
                    sizeComboBox.Text = "0";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            column = null;
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void signedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex == 2 && sizeComboBox.Text != "")
            {
                if (signedRadioButton.Checked)
                    --fractionNumericUpDown.Maximum;
                else
                    ++fractionNumericUpDown.Maximum;
            }
        }


    }
}
