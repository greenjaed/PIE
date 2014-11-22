using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PIE
{
    public partial class ColumnEditForm : Form
    {
        static string[] BitSelect = { "8", "16", "32", "64" };
        public ColumnDescriptor Column { get; private set; }

        public ColumnEditForm()
        {
            InitializeComponent();
        }

        public ColumnEditForm(ColumnDescriptor init)
        {
            InitializeComponent();
            nameTextBox.Text = init.Name;
            typeComboBox.Text = init.TypeOfData.ToString();
            sizeComboBox.Text = (init.Size / (init.TypeOfData == DataType.CharString ? 8 : 1)).ToString();
            if (init.IntFormat == IntFormat.Signed)
            {
                signedRadioButton.Checked = true;
            }
            else if (init.IntFormat == IntFormat.Hex)
            {
                hexRadioButton.Checked = true;
            }
            else
            {
                noneRadioButton.Checked = true;
            }
            fractionNumericUpDown.Value = init.Fraction;
            fractionNumericUpDown.Maximum = init.Size;
            if (init.TypeOfData == DataType.CharString || init.TypeOfData == DataType.FloatingPoint)
            {
                optionsGroupBox.Enabled = false;
            }
            else if (init.TypeOfData == DataType.Integer)
            {
                fractionLabel.Enabled = fractionNumericUpDown.Enabled = false;
            }
            else
            {
                hexRadioButton.Enabled = false;
            }

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
                    sizeComboBox.Items.AddRange(BitSelect);
                    hexRadioButton.Enabled = true;
                    break;
                case 1:
                    sizeComboBox.Items.AddRange(BitSelect);
                    sizeComboBox.Items.RemoveAt(0);
                    optionsGroupBox.Enabled = false;
                    break;
                case 2:
                    sizeComboBox.Items.AddRange(BitSelect);
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
                {
                    --fractionNumericUpDown.Maximum;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(sizeComboBox.Text, out Column.Size))
            {
                MessageBox.Show("Size is an invalid value", "Error");
                return;
            }
            Column = new ColumnDescriptor()
            {
                Name = nameTextBox.Text,
                TypeOfData = (DataType) Enum.ToObject(typeof(DataType), typeComboBox.SelectedIndex),
                Fraction = (int) fractionNumericUpDown.Value,
                Size = int.Parse(sizeComboBox.Text)
            };
            Column.DefaultValue = Column.TypeOfData == DataType.String ? string.Empty : "0";
            if (Column.TypeOfData == DataType.CharString)
            {
                Column.Size <<= 3;
            }
            if (signedRadioButton.Checked)
            {
                Column.IntFormat = IntFormat.Signed;
            }
            else if (hexRadioButton.Checked)
            {
                Column.IntFormat = IntFormat.Hex;
            }
            else
            {
                Column.IntFormat = IntFormat.None;
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void sizeComboBox_Validating(object sender, CancelEventArgs e)
        {
            int test;
            if (typeComboBox.SelectedIndex == 3 && !int.TryParse(sizeComboBox.Text, out test))
            {
                    sizeComboBox.Text = "0";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Column = null;
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void signedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex == 2 && !string.IsNullOrEmpty(sizeComboBox.Text))
            {
                if (signedRadioButton.Checked)
                {
                    --fractionNumericUpDown.Maximum;
                }
                else
                {
                    ++fractionNumericUpDown.Maximum;
                }
            }
        }
    }
}
