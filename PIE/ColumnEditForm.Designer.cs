namespace PIE
{
    partial class ColumnEditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.hexCheckBox = new System.Windows.Forms.CheckBox();
            this.fractionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.signedCheckBox = new System.Windows.Forms.CheckBox();
            this.fractionLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.optionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fractionNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(80, 10);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(121, 20);
            this.nameTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type";
            // 
            // typeComboBox
            // 
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Items.AddRange(new object[] {
            "Integer",
            "Floating Point",
            "Fixed Point",
            "String"});
            this.typeComboBox.Location = new System.Drawing.Point(80, 36);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(121, 21);
            this.typeComboBox.TabIndex = 3;
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(22, 66);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(52, 13);
            this.sizeLabel.TabIndex = 4;
            this.sizeLabel.Text = "Size (bits)";
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeComboBox.Enabled = false;
            this.sizeComboBox.FormattingEnabled = true;
            this.sizeComboBox.Location = new System.Drawing.Point(80, 63);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(121, 21);
            this.sizeComboBox.TabIndex = 5;
            this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.sizeComboBox_SelectedIndexChanged);
            this.sizeComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.sizeComboBox_Validating);
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Controls.Add(this.hexCheckBox);
            this.optionsGroupBox.Controls.Add(this.fractionNumericUpDown);
            this.optionsGroupBox.Controls.Add(this.signedCheckBox);
            this.optionsGroupBox.Controls.Add(this.fractionLabel);
            this.optionsGroupBox.Enabled = false;
            this.optionsGroupBox.Location = new System.Drawing.Point(16, 90);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(185, 76);
            this.optionsGroupBox.TabIndex = 6;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // hexCheckBox
            // 
            this.hexCheckBox.AutoSize = true;
            this.hexCheckBox.Location = new System.Drawing.Point(119, 20);
            this.hexCheckBox.Name = "hexCheckBox";
            this.hexCheckBox.Size = new System.Drawing.Size(45, 17);
            this.hexCheckBox.TabIndex = 1;
            this.hexCheckBox.Text = "Hex";
            this.hexCheckBox.UseVisualStyleBackColor = true;
            // 
            // fractionNumericUpDown
            // 
            this.fractionNumericUpDown.Location = new System.Drawing.Point(80, 43);
            this.fractionNumericUpDown.Name = "fractionNumericUpDown";
            this.fractionNumericUpDown.Size = new System.Drawing.Size(49, 20);
            this.fractionNumericUpDown.TabIndex = 3;
            // 
            // signedCheckBox
            // 
            this.signedCheckBox.AutoSize = true;
            this.signedCheckBox.Location = new System.Drawing.Point(7, 20);
            this.signedCheckBox.Name = "signedCheckBox";
            this.signedCheckBox.Size = new System.Drawing.Size(59, 17);
            this.signedCheckBox.TabIndex = 0;
            this.signedCheckBox.Text = "Signed";
            this.signedCheckBox.UseVisualStyleBackColor = true;
            // 
            // fractionLabel
            // 
            this.fractionLabel.AutoSize = true;
            this.fractionLabel.Location = new System.Drawing.Point(6, 45);
            this.fractionLabel.Name = "fractionLabel";
            this.fractionLabel.Size = new System.Drawing.Size(68, 13);
            this.fractionLabel.TabIndex = 2;
            this.fractionLabel.Text = "Fraction Size";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(45, 172);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(126, 172);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ColumnEditForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(221, 217);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.sizeComboBox);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.typeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ColumnEditForm";
            this.Text = "ColumnEditForm";
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fractionNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.ComboBox sizeComboBox;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.CheckBox hexCheckBox;
        private System.Windows.Forms.CheckBox signedCheckBox;
        private System.Windows.Forms.Label fractionLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.NumericUpDown fractionNumericUpDown;
    }
}