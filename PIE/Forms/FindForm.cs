using System;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    public partial class FindForm : Form
    {
        private FindOptions FindOptions;  //the find options
        public HexBox SearchMedium { get; set; }

        public FindForm()
        {
            InitializeComponent();
            FindOptions = new FindOptions();
        }

        public FindForm(HexBox toSearch)
        {
            InitializeComponent();
            FindOptions = new FindOptions();
            SearchMedium = toSearch;
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
            if (FindOptions.IsValid)
            {
                search();
            }
        }

        public void search()
        {
            long result = SearchMedium.Find(FindOptions);
            if (result == -1)
            {
                MessageBox.Show("No match found", "PIE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == -2)
            {
                return;
            }
        }

        private void validateFindOptions()
        {
            FindOptions.MatchCase = caseCheckBox.Checked;
            if (textRadioButton.Checked)
            {
                FindOptions.Text = searchTextBox.Text;
                FindOptions.Type = FindType.Text;
                FindOptions.IsValid = true;
            }
            else
            {
                if ((FindOptions.Hex = hexToByteArray()) == null)
                {
                    FindOptions.IsValid = false;
                }
                else
                {
                    FindOptions.IsValid = true;
                }
                FindOptions.Type = FindType.Hex;
            }
        }

        //converts hex string to byte array
        private byte[] hexToByteArray()
        {
            String hexString = searchTextBox.Text;
            byte[] bytes;
            int numberOfBytes;

            hexString = hexString.Replace(" ", string.Empty);
            if ((hexString.Length & 1) == 1)
            {
                hexString = "0" + hexString;
            }
            numberOfBytes = hexString.Length / 2;
            bytes = new byte[numberOfBytes];

            try
            {
                for (int i = 0; i < numberOfBytes; ++i)
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(2 * i, 2), 16);
                }
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
            {
                findButton.Enabled = true;
            }
            else
            {
                findButton.Enabled = false;
            }
        }
    }
}
