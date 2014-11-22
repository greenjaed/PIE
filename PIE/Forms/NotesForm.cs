using System;
using System.Windows.Forms;
using PIE.Slices;

namespace PIE
{
    public partial class NotesForm : Form
    {
        private Slice Slice;

        public NotesForm()
        {
            InitializeComponent();
        }

        public NotesForm(Slice slice)
        {
            InitializeComponent();
            this.Slice = slice;
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            infoTextBox.Text = Slice.Notes;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Slice.Notes = infoTextBox.Text;
            this.Close();
        }
    }
}
