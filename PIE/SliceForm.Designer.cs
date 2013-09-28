namespace PIE
{
    partial class SliceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sliceButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.AdvancedCheckBox = new System.Windows.Forms.CheckBox();
            this.endTextBox = new System.Windows.Forms.TextBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.bytesComboBox = new System.Windows.Forms.ComboBox();
            this.endLabel = new System.Windows.Forms.Label();
            this.startTextBox = new System.Windows.Forms.TextBox();
            this.bytesLabel = new System.Windows.Forms.Label();
            this.startLabel = new System.Windows.Forms.Label();
            this.repeatCheckBox = new System.Windows.Forms.CheckBox();
            this.invalidLabel1 = new System.Windows.Forms.Label();
            this.invalidLabel2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // sliceButton
            // 
            this.sliceButton.Location = new System.Drawing.Point(187, 87);
            this.sliceButton.Name = "sliceButton";
            this.sliceButton.Size = new System.Drawing.Size(75, 23);
            this.sliceButton.TabIndex = 4;
            this.sliceButton.Text = "Slice";
            this.toolTip1.SetToolTip(this.sliceButton, "Slice the slice");
            this.sliceButton.UseVisualStyleBackColor = true;
            this.sliceButton.Click += new System.EventHandler(this.sliceButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(268, 87);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cancelButton, "Cancel");
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // AdvancedCheckBox
            // 
            this.AdvancedCheckBox.AutoSize = true;
            this.AdvancedCheckBox.Location = new System.Drawing.Point(17, 87);
            this.AdvancedCheckBox.Name = "AdvancedCheckBox";
            this.AdvancedCheckBox.Size = new System.Drawing.Size(75, 17);
            this.AdvancedCheckBox.TabIndex = 6;
            this.AdvancedCheckBox.Text = "Advanced";
            this.toolTip1.SetToolTip(this.AdvancedCheckBox, "Show advanced input");
            this.AdvancedCheckBox.UseVisualStyleBackColor = true;
            this.AdvancedCheckBox.CheckedChanged += new System.EventHandler(this.AdvancedCheckBox_CheckedChanged);
            // 
            // endTextBox
            // 
            this.endTextBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.endTextBox.Location = new System.Drawing.Point(185, 12);
            this.endTextBox.Name = "endTextBox";
            this.endTextBox.Size = new System.Drawing.Size(100, 20);
            this.endTextBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.endTextBox, "Input an end address in hexidecimal");
            this.endTextBox.Leave += new System.EventHandler(this.endTextBox_Leave);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(152, 15);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(27, 13);
            this.sizeLabel.TabIndex = 4;
            this.sizeLabel.Text = "Size";
            this.toolTip1.SetToolTip(this.sizeLabel, "The size of the slice");
            this.sizeLabel.Visible = false;
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.FormattingEnabled = true;
            this.sizeComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256",
            "512"});
            this.sizeComboBox.Location = new System.Drawing.Point(185, 12);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(48, 21);
            this.sizeComboBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.sizeComboBox, "Select the slice size");
            this.sizeComboBox.Visible = false;
            this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.sizeComboBox_SelectedIndexChanged);
            this.sizeComboBox.Leave += new System.EventHandler(this.sizeComboBox_SelectedIndexChanged);
            // 
            // bytesComboBox
            // 
            this.bytesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bytesComboBox.FormattingEnabled = true;
            this.bytesComboBox.Items.AddRange(new object[] {
            "",
            "kilo",
            "mega"});
            this.bytesComboBox.Location = new System.Drawing.Point(239, 12);
            this.bytesComboBox.Name = "bytesComboBox";
            this.bytesComboBox.Size = new System.Drawing.Size(60, 21);
            this.bytesComboBox.TabIndex = 2;
            this.toolTip1.SetToolTip(this.bytesComboBox, "Select the slice magnitude");
            this.bytesComboBox.Visible = false;
            this.bytesComboBox.SelectedIndexChanged += new System.EventHandler(this.bytesComboBox_SelectedIndexChanged);
            // 
            // endLabel
            // 
            this.endLabel.AutoSize = true;
            this.endLabel.Location = new System.Drawing.Point(153, 15);
            this.endLabel.Name = "endLabel";
            this.endLabel.Size = new System.Drawing.Size(26, 13);
            this.endLabel.TabIndex = 3;
            this.endLabel.Text = "End";
            this.toolTip1.SetToolTip(this.endLabel, "The end address (inclusive)");
            // 
            // startTextBox
            // 
            this.startTextBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.startTextBox.Location = new System.Drawing.Point(47, 12);
            this.startTextBox.Name = "startTextBox";
            this.startTextBox.Size = new System.Drawing.Size(100, 20);
            this.startTextBox.TabIndex = 0;
            this.toolTip1.SetToolTip(this.startTextBox, "Input a start address in hexidecimal");
            this.startTextBox.Leave += new System.EventHandler(this.startTextBox_Leave);
            // 
            // bytesLabel
            // 
            this.bytesLabel.AutoSize = true;
            this.bytesLabel.Location = new System.Drawing.Point(305, 16);
            this.bytesLabel.Name = "bytesLabel";
            this.bytesLabel.Size = new System.Drawing.Size(38, 13);
            this.bytesLabel.TabIndex = 5;
            this.bytesLabel.Text = "byte(s)";
            this.bytesLabel.Visible = false;
            // 
            // startLabel
            // 
            this.startLabel.AutoSize = true;
            this.startLabel.Location = new System.Drawing.Point(12, 15);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(29, 13);
            this.startLabel.TabIndex = 2;
            this.startLabel.Text = "Start";
            this.toolTip1.SetToolTip(this.startLabel, "The start address");
            // 
            // repeatCheckBox
            // 
            this.repeatCheckBox.AutoSize = true;
            this.repeatCheckBox.Location = new System.Drawing.Point(216, 64);
            this.repeatCheckBox.Name = "repeatCheckBox";
            this.repeatCheckBox.Size = new System.Drawing.Size(127, 17);
            this.repeatCheckBox.TabIndex = 6;
            this.repeatCheckBox.Text = "Repeat over the slice";
            this.toolTip1.SetToolTip(this.repeatCheckBox, "Repeat this slice over the rest of the slice");
            this.repeatCheckBox.UseVisualStyleBackColor = true;
            this.repeatCheckBox.Visible = false;
            // 
            // invalidLabel1
            // 
            this.invalidLabel1.AutoSize = true;
            this.invalidLabel1.ForeColor = System.Drawing.Color.Red;
            this.invalidLabel1.Location = new System.Drawing.Point(44, 35);
            this.invalidLabel1.Name = "invalidLabel1";
            this.invalidLabel1.Size = new System.Drawing.Size(73, 13);
            this.invalidLabel1.TabIndex = 7;
            this.invalidLabel1.Text = "Invalid Format";
            this.toolTip1.SetToolTip(this.invalidLabel1, "s");
            this.invalidLabel1.Visible = false;
            // 
            // invalidLabel2
            // 
            this.invalidLabel2.AutoSize = true;
            this.invalidLabel2.ForeColor = System.Drawing.Color.Red;
            this.invalidLabel2.Location = new System.Drawing.Point(182, 35);
            this.invalidLabel2.Name = "invalidLabel2";
            this.invalidLabel2.Size = new System.Drawing.Size(73, 13);
            this.invalidLabel2.TabIndex = 7;
            this.invalidLabel2.Text = "Invalid Format";
            this.invalidLabel2.Visible = false;
            // 
            // SliceForm
            // 
            this.AcceptButton = this.sliceButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(361, 128);
            this.Controls.Add(this.endTextBox);
            this.Controls.Add(this.invalidLabel2);
            this.Controls.Add(this.AdvancedCheckBox);
            this.Controls.Add(this.invalidLabel1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.repeatCheckBox);
            this.Controls.Add(this.sliceButton);
            this.Controls.Add(this.startLabel);
            this.Controls.Add(this.bytesLabel);
            this.Controls.Add(this.startTextBox);
            this.Controls.Add(this.endLabel);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.bytesComboBox);
            this.Controls.Add(this.sizeComboBox);
            this.Name = "SliceForm";
            this.Text = "Slice";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sliceButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox AdvancedCheckBox;
        private System.Windows.Forms.TextBox endTextBox;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.ComboBox sizeComboBox;
        private System.Windows.Forms.ComboBox bytesComboBox;
        private System.Windows.Forms.Label endLabel;
        private System.Windows.Forms.TextBox startTextBox;
        private System.Windows.Forms.Label bytesLabel;
        private System.Windows.Forms.Label startLabel;
        private System.Windows.Forms.CheckBox repeatCheckBox;
        private System.Windows.Forms.Label invalidLabel1;
        private System.Windows.Forms.Label invalidLabel2;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}