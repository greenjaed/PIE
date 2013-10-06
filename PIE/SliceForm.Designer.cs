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
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // sliceButton
            // 
            this.sliceButton.Location = new System.Drawing.Point(185, 75);
            this.sliceButton.Name = "sliceButton";
            this.sliceButton.Size = new System.Drawing.Size(75, 23);
            this.sliceButton.TabIndex = 4;
            this.sliceButton.Text = "Slice";
            this.sliceButton.UseVisualStyleBackColor = true;
            this.sliceButton.Click += new System.EventHandler(this.sliceButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(266, 75);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // AdvancedCheckBox
            // 
            this.AdvancedCheckBox.AutoSize = true;
            this.AdvancedCheckBox.Location = new System.Drawing.Point(15, 75);
            this.AdvancedCheckBox.Name = "AdvancedCheckBox";
            this.AdvancedCheckBox.Size = new System.Drawing.Size(75, 17);
            this.AdvancedCheckBox.TabIndex = 6;
            this.AdvancedCheckBox.Text = "Advanced";
            this.AdvancedCheckBox.UseVisualStyleBackColor = true;
            this.AdvancedCheckBox.CheckedChanged += new System.EventHandler(this.AdvancedCheckBox_CheckedChanged);
            // 
            // endTextBox
            // 
            this.endTextBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.endTextBox.Location = new System.Drawing.Point(199, 12);
            this.endTextBox.Name = "endTextBox";
            this.endTextBox.Size = new System.Drawing.Size(100, 20);
            this.endTextBox.TabIndex = 1;
            this.endTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.endTextBox_Validating);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(167, 15);
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
            this.sizeComboBox.Location = new System.Drawing.Point(200, 12);
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
            this.bytesComboBox.Location = new System.Drawing.Point(266, 12);
            this.bytesComboBox.Name = "bytesComboBox";
            this.bytesComboBox.Size = new System.Drawing.Size(60, 21);
            this.bytesComboBox.TabIndex = 2;
            this.bytesComboBox.Visible = false;
            this.bytesComboBox.SelectedIndexChanged += new System.EventHandler(this.bytesComboBox_SelectedIndexChanged);
            // 
            // endLabel
            // 
            this.endLabel.AutoSize = true;
            this.endLabel.Location = new System.Drawing.Point(167, 15);
            this.endLabel.Name = "endLabel";
            this.endLabel.Size = new System.Drawing.Size(26, 13);
            this.endLabel.TabIndex = 3;
            this.endLabel.Text = "End";
            // 
            // startTextBox
            // 
            this.startTextBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.startTextBox.Location = new System.Drawing.Point(47, 12);
            this.startTextBox.Name = "startTextBox";
            this.startTextBox.Size = new System.Drawing.Size(100, 20);
            this.startTextBox.TabIndex = 0;
            this.startTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.startTextBox_Validating);
            // 
            // bytesLabel
            // 
            this.bytesLabel.AutoSize = true;
            this.bytesLabel.Location = new System.Drawing.Point(332, 15);
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
            // 
            // repeatCheckBox
            // 
            this.repeatCheckBox.AutoSize = true;
            this.repeatCheckBox.Location = new System.Drawing.Point(214, 52);
            this.repeatCheckBox.Name = "repeatCheckBox";
            this.repeatCheckBox.Size = new System.Drawing.Size(127, 17);
            this.repeatCheckBox.TabIndex = 6;
            this.repeatCheckBox.Text = "Repeat over the slice";
            this.repeatCheckBox.UseVisualStyleBackColor = true;
            this.repeatCheckBox.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // SliceForm
            // 
            this.AcceptButton = this.sliceButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(377, 115);
            this.Controls.Add(this.AdvancedCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.repeatCheckBox);
            this.Controls.Add(this.sliceButton);
            this.Controls.Add(this.startLabel);
            this.Controls.Add(this.bytesLabel);
            this.Controls.Add(this.startTextBox);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.bytesComboBox);
            this.Controls.Add(this.sizeComboBox);
            this.Controls.Add(this.endTextBox);
            this.Controls.Add(this.endLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SliceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Slice";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}