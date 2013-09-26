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
            this.basicStartTextBox = new System.Windows.Forms.TextBox();
            this.endTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sliceButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.AdvancedCheckBox = new System.Windows.Forms.CheckBox();
            this.advancedPanel = new System.Windows.Forms.Panel();
            this.advancedStartTextBox = new System.Windows.Forms.TextBox();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.bytesComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.repeatCheckBox = new System.Windows.Forms.CheckBox();
            this.basicPanel = new System.Windows.Forms.Panel();
            this.advancedPanel.SuspendLayout();
            this.basicPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // basicStartTextBox
            // 
            this.basicStartTextBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.basicStartTextBox.Location = new System.Drawing.Point(50, 14);
            this.basicStartTextBox.Name = "basicStartTextBox";
            this.basicStartTextBox.Size = new System.Drawing.Size(100, 20);
            this.basicStartTextBox.TabIndex = 0;
            this.basicStartTextBox.Leave += new System.EventHandler(this.basicStartTextBox_Leave);
            // 
            // endTextBox
            // 
            this.endTextBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.endTextBox.Location = new System.Drawing.Point(188, 14);
            this.endTextBox.Name = "endTextBox";
            this.endTextBox.Size = new System.Drawing.Size(100, 20);
            this.endTextBox.TabIndex = 1;
            this.endTextBox.Leave += new System.EventHandler(this.endTextBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(156, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "End";
            // 
            // sliceButton
            // 
            this.sliceButton.Location = new System.Drawing.Point(141, 110);
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
            this.cancelButton.Location = new System.Drawing.Point(222, 110);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // AdvancedCheckBox
            // 
            this.AdvancedCheckBox.AutoSize = true;
            this.AdvancedCheckBox.Location = new System.Drawing.Point(12, 114);
            this.AdvancedCheckBox.Name = "AdvancedCheckBox";
            this.AdvancedCheckBox.Size = new System.Drawing.Size(75, 17);
            this.AdvancedCheckBox.TabIndex = 6;
            this.AdvancedCheckBox.Text = "Advanced";
            this.AdvancedCheckBox.UseVisualStyleBackColor = true;
            this.AdvancedCheckBox.CheckedChanged += new System.EventHandler(this.AdvancedCheckBox_CheckedChanged);
            // 
            // advancedPanel
            // 
            this.advancedPanel.Controls.Add(this.repeatCheckBox);
            this.advancedPanel.Controls.Add(this.label5);
            this.advancedPanel.Controls.Add(this.label4);
            this.advancedPanel.Controls.Add(this.label3);
            this.advancedPanel.Controls.Add(this.bytesComboBox);
            this.advancedPanel.Controls.Add(this.sizeComboBox);
            this.advancedPanel.Controls.Add(this.advancedStartTextBox);
            this.advancedPanel.Location = new System.Drawing.Point(12, 12);
            this.advancedPanel.Name = "advancedPanel";
            this.advancedPanel.Size = new System.Drawing.Size(464, 82);
            this.advancedPanel.TabIndex = 7;
            this.advancedPanel.Visible = false;
            // 
            // advancedStartTextBox
            // 
            this.advancedStartTextBox.Location = new System.Drawing.Point(50, 14);
            this.advancedStartTextBox.Name = "advancedStartTextBox";
            this.advancedStartTextBox.Size = new System.Drawing.Size(100, 20);
            this.advancedStartTextBox.TabIndex = 0;
            this.advancedStartTextBox.Leave += new System.EventHandler(this.advancedStartTextBox_Leave);
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
            this.sizeComboBox.Location = new System.Drawing.Point(189, 14);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(121, 21);
            this.sizeComboBox.TabIndex = 1;
            // 
            // bytesComboBox
            // 
            this.bytesComboBox.FormattingEnabled = true;
            this.bytesComboBox.Items.AddRange(new object[] {
            "",
            "kilo",
            "mega"});
            this.bytesComboBox.Location = new System.Drawing.Point(316, 14);
            this.bytesComboBox.Name = "bytesComboBox";
            this.bytesComboBox.Size = new System.Drawing.Size(60, 21);
            this.bytesComboBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Start";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(156, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Size";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(382, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "bytes";
            // 
            // repeatCheckBox
            // 
            this.repeatCheckBox.AutoSize = true;
            this.repeatCheckBox.Location = new System.Drawing.Point(316, 51);
            this.repeatCheckBox.Name = "repeatCheckBox";
            this.repeatCheckBox.Size = new System.Drawing.Size(127, 17);
            this.repeatCheckBox.TabIndex = 6;
            this.repeatCheckBox.Text = "Repeat over the slice";
            this.repeatCheckBox.UseVisualStyleBackColor = true;
            // 
            // basicPanel
            // 
            this.basicPanel.Controls.Add(this.label1);
            this.basicPanel.Controls.Add(this.basicStartTextBox);
            this.basicPanel.Controls.Add(this.endTextBox);
            this.basicPanel.Controls.Add(this.label2);
            this.basicPanel.Location = new System.Drawing.Point(12, 12);
            this.basicPanel.Name = "basicPanel";
            this.basicPanel.Size = new System.Drawing.Size(300, 82);
            this.basicPanel.TabIndex = 8;
            // 
            // SliceForm
            // 
            this.AcceptButton = this.sliceButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(334, 162);
            this.Controls.Add(this.AdvancedCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.sliceButton);
            this.Controls.Add(this.basicPanel);
            this.Controls.Add(this.advancedPanel);
            this.Name = "SliceForm";
            this.Text = "Slice";
            this.advancedPanel.ResumeLayout(false);
            this.advancedPanel.PerformLayout();
            this.basicPanel.ResumeLayout(false);
            this.basicPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox basicStartTextBox;
        private System.Windows.Forms.TextBox endTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sliceButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox AdvancedCheckBox;
        private System.Windows.Forms.Panel advancedPanel;
        private System.Windows.Forms.CheckBox repeatCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox bytesComboBox;
        private System.Windows.Forms.ComboBox sizeComboBox;
        private System.Windows.Forms.TextBox advancedStartTextBox;
        private System.Windows.Forms.Panel basicPanel;
    }
}