using System;
using System.Windows.Forms;

namespace PIE
{
    public partial class NotesForm : Form
    {
        private Slice slice;

        public NotesForm()
        {
            InitializeComponent();
        }

        public NotesForm(Slice slice)
        {
            InitializeComponent();
            this.slice = slice;
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            infoTextBox.Text = slice.Notes;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            slice.Notes = infoTextBox.Text;
            this.Close();
        }
    }
}
