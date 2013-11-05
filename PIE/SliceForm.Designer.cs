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
            this.AdvancedCheckBox = new System.Windows.Forms.CheckBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.bytesComboBox = new System.Windows.Forms.ComboBox();
            this.bytesLabel = new System.Windows.Forms.Label();
            this.repeatCheckBox = new System.Windows.Forms.CheckBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // startLabel
            // 
            this.startLabel.Location = new System.Drawing.Point(18, 46);
            // 
            // endLabel
            // 
            this.endLabel.Location = new System.Drawing.Point(179, 46);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(168, 89);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(249, 89);
            // 
            // endTextBox
            // 
            this.endTextBox.Location = new System.Drawing.Point(211, 43);
            // 
            // startTextBox
            // 
            this.startTextBox.Location = new System.Drawing.Point(53, 43);
            // 
            // AdvancedCheckBox
            // 
            this.AdvancedCheckBox.AutoSize = true;
            this.AdvancedCheckBox.Location = new System.Drawing.Point(11, 97);
            this.AdvancedCheckBox.Name = "AdvancedCheckBox";
            this.AdvancedCheckBox.Size = new System.Drawing.Size(75, 17);
            this.AdvancedCheckBox.TabIndex = 6;
            this.AdvancedCheckBox.Text = "Advanced";
            this.AdvancedCheckBox.UseVisualStyleBackColor = true;
            this.AdvancedCheckBox.CheckedChanged += new System.EventHandler(this.AdvancedCheckBox_CheckedChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(179, 46);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(27, 13);
            this.sizeLabel.TabIndex = 4;
            this.sizeLabel.Text = "Size";
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
            this.sizeComboBox.Location = new System.Drawing.Point(212, 43);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(48, 21);
            this.sizeComboBox.TabIndex = 1;
            this.sizeComboBox.Visible = false;
            this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.sizeComboBox_SelectedIndexChanged);
            this.sizeComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.sizeComboBox_Validating);
            // 
            // bytesComboBox
            // 
            this.bytesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bytesComboBox.FormattingEnabled = true;
            this.bytesComboBox.Items.AddRange(new object[] {
            "",
            "kilo",
            "mega"});
            this.bytesComboBox.Location = new System.Drawing.Point(278, 43);
            this.bytesComboBox.Name = "bytesComboBox";
            this.bytesComboBox.Size = new System.Drawing.Size(60, 21);
            this.bytesComboBox.TabIndex = 2;
            this.bytesComboBox.Visible = false;
            this.bytesComboBox.SelectedIndexChanged += new System.EventHandler(this.bytesComboBox_SelectedIndexChanged);
            // 
            // bytesLabel
            // 
            this.bytesLabel.AutoSize = true;
            this.bytesLabel.Location = new System.Drawing.Point(344, 46);
            this.bytesLabel.Name = "bytesLabel";
            this.bytesLabel.Size = new System.Drawing.Size(38, 13);
            this.bytesLabel.TabIndex = 5;
            this.bytesLabel.Text = "byte(s)";
            this.bytesLabel.Visible = false;
            // 
            // repeatCheckBox
            // 
            this.repeatCheckBox.AutoSize = true;
            this.repeatCheckBox.Location = new System.Drawing.Point(11, 74);
            this.repeatCheckBox.Name = "repeatCheckBox";
            this.repeatCheckBox.Size = new System.Drawing.Size(127, 17);
            this.repeatCheckBox.TabIndex = 6;
            this.repeatCheckBox.Text = "Repeat over the slice";
            this.repeatCheckBox.UseVisualStyleBackColor = true;
            this.repeatCheckBox.Visible = false;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 20);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(35, 13);
            this.nameLabel.TabIndex = 7;
            this.nameLabel.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(53, 17);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(258, 20);
            this.nameTextBox.TabIndex = 8;
            // 
            // SliceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 140);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.AdvancedCheckBox);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.sizeComboBox);
            this.Controls.Add(this.bytesComboBox);
            this.Controls.Add(this.bytesLabel);
            this.Controls.Add(this.repeatCheckBox);
            this.Name = "SliceForm";
            this.Text = "Slice";
            this.Controls.SetChildIndex(this.repeatCheckBox, 0);
            this.Controls.SetChildIndex(this.bytesLabel, 0);
            this.Controls.SetChildIndex(this.bytesComboBox, 0);
            this.Controls.SetChildIndex(this.sizeComboBox, 0);
            this.Controls.SetChildIndex(this.sizeLabel, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.endLabel, 0);
            this.Controls.SetChildIndex(this.endTextBox, 0);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.startTextBox, 0);
            this.Controls.SetChildIndex(this.startLabel, 0);
            this.Controls.SetChildIndex(this.AdvancedCheckBox, 0);
            this.Controls.SetChildIndex(this.nameLabel, 0);
            this.Controls.SetChildIndex(this.nameTextBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox AdvancedCheckBox;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.ComboBox sizeComboBox;
        private System.Windows.Forms.ComboBox bytesComboBox;
        private System.Windows.Forms.Label bytesLabel;
        private System.Windows.Forms.CheckBox repeatCheckBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameTextBox;
    }
}