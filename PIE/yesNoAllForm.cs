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
    public enum yesNoAllResult
    {
        Yes,
        YesAll,
        No,
        NoAll
    };

    public partial class YesNoAllForm : Form
    {
        public yesNoAllResult Result { get; protected set; }

        public YesNoAllForm()
        {
            InitializeComponent();
        }

        public YesNoAllForm(String body, String caption)
        {
            bodyLabel.Text = body;
            this.Text = caption;
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            Result = yesNoAllResult.Yes;
            this.Close();
        }

        private void yesAllbutton_Click(object sender, EventArgs e)
        {
            Result = yesNoAllResult.YesAll;
            this.Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            Result = yesNoAllResult.No;
            this.Close();
        }

        private void noAllbutton_Click(object sender, EventArgs e)
        {
            Result = yesNoAllResult.NoAll;
            this.Close();
        }
    }
}
