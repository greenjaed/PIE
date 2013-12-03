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
            infoTextBox.Text = slice.notes;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            slice.notes = infoTextBox.Text;
            this.Close();
        }
    }
}
