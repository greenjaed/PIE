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
    public partial class FindForm : Form
    {
        private FindOptions findOptions;
        public HexBox searchMedium { get; set; }

        public FindForm()
        {
            InitializeComponent();
            findOptions = new FindOptions();
        }

        public FindForm(HexBox toSearch)
        {
            InitializeComponent();
            findOptions = new FindOptions();
            searchMedium = toSearch;
        }

        private void textRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            caseCheckBox.Enabled = !caseCheckBox.Enabled;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            validateFindOptions();
            if (findOptions.IsValid)
                search();
        }

        public void search()
        {
            long result = searchMedium.Find(findOptions);
            if (result == -1)
                MessageBox.Show("No match found", "PIE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (result == -2)
                return;
            else
            {
                this.Hide();
                if (!searchMedium.Focused)
                    searchMedium.Focus();
            }
        }

        private void validateFindOptions()
        {
            findOptions.MatchCase = caseCheckBox.Checked;
            if (textRadioButton.Checked)
            {
                findOptions.Text = searchTextBox.Text;
                findOptions.Type = FindType.Text;
                findOptions.IsValid = true;
            }
            else
            {
                if ((findOptions.Hex = hexToByteArray()) == null)
                    findOptions.IsValid = false;
                else
                    findOptions.IsValid = true;
                findOptions.Type = FindType.Hex;
            }
        }

        private byte[] hexToByteArray()
        {
            String hexString = searchTextBox.Text;
            byte[] bytes;
            int numberOfBytes;

            hexString = hexString.Replace(" ", "");
            if ((hexString.Length & 1) == 1)
                hexString = "0" + hexString;
            numberOfBytes = hexString.Length / 2;
            bytes = new byte[numberOfBytes];

            try
            {
                for (int i = 0; i < numberOfBytes; ++i)
                    bytes[i] = Convert.ToByte(hexString.Substring(2 * i, 2), 16);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Formatting Error: " + ex.Message);
                return null;
            }
            return bytes;
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (searchTextBox.Text.Length > 0)
                findButton.Enabled = true;
            else
                findButton.Enabled = false;
        }
    }
}
