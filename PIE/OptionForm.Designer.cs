namespace PIE
{
    partial class OptionForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupSizeMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.bytesMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.hexCaseComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.byteGroupCheckBox = new System.Windows.Forms.CheckBox();
            this.columnNumberCheckBox = new System.Windows.Forms.CheckBox();
            this.lineCheckBox = new System.Windows.Forms.CheckBox();
            this.charCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.invertButton = new System.Windows.Forms.Button();
            this.changeButton = new System.Windows.Forms.Button();
            this.selectionColorButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.addressColorButton = new System.Windows.Forms.Button();
            this.backColorButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.fontTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cancelButton);
            this.splitContainer1.Panel2.Controls.Add(this.okButton);
            this.splitContainer1.Size = new System.Drawing.Size(353, 321);
            this.splitContainer1.SplitterDistance = 270;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupSizeMaskedTextBox);
            this.groupBox2.Controls.Add(this.bytesMaskedTextBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.hexCaseComboBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.byteGroupCheckBox);
            this.groupBox2.Controls.Add(this.columnNumberCheckBox);
            this.groupBox2.Controls.Add(this.lineCheckBox);
            this.groupBox2.Controls.Add(this.charCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(326, 124);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Display";
            // 
            // groupSizeMaskedTextBox
            // 
            this.groupSizeMaskedTextBox.Location = new System.Drawing.Point(247, 71);
            this.groupSizeMaskedTextBox.Mask = "999";
            this.groupSizeMaskedTextBox.Name = "groupSizeMaskedTextBox";
            this.groupSizeMaskedTextBox.PromptChar = ' ';
            this.groupSizeMaskedTextBox.Size = new System.Drawing.Size(66, 20);
            this.groupSizeMaskedTextBox.TabIndex = 6;
            // 
            // bytesMaskedTextBox
            // 
            this.bytesMaskedTextBox.Location = new System.Drawing.Point(247, 45);
            this.bytesMaskedTextBox.Mask = "999";
            this.bytesMaskedTextBox.Name = "bytesMaskedTextBox";
            this.bytesMaskedTextBox.PromptChar = ' ';
            this.bytesMaskedTextBox.Size = new System.Drawing.Size(66, 20);
            this.bytesMaskedTextBox.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(159, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Bytes Per Group";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(159, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Bytes Per Line";
            // 
            // hexCaseComboBox
            // 
            this.hexCaseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hexCaseComboBox.FormattingEnabled = true;
            this.hexCaseComboBox.Items.AddRange(new object[] {
            "Lower",
            "Upper"});
            this.hexCaseComboBox.Location = new System.Drawing.Point(247, 18);
            this.hexCaseComboBox.Name = "hexCaseComboBox";
            this.hexCaseComboBox.Size = new System.Drawing.Size(66, 21);
            this.hexCaseComboBox.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(159, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Hex Case";
            // 
            // byteGroupCheckBox
            // 
            this.byteGroupCheckBox.AutoSize = true;
            this.byteGroupCheckBox.Location = new System.Drawing.Point(7, 73);
            this.byteGroupCheckBox.Name = "byteGroupCheckBox";
            this.byteGroupCheckBox.Size = new System.Drawing.Size(93, 17);
            this.byteGroupCheckBox.TabIndex = 2;
            this.byteGroupCheckBox.Text = "Byte Grouping";
            this.byteGroupCheckBox.UseVisualStyleBackColor = true;
            this.byteGroupCheckBox.CheckedChanged += new System.EventHandler(this.byteGroupCheckBox_CheckedChanged);
            // 
            // columnNumberCheckBox
            // 
            this.columnNumberCheckBox.AutoSize = true;
            this.columnNumberCheckBox.Location = new System.Drawing.Point(7, 99);
            this.columnNumberCheckBox.Name = "columnNumberCheckBox";
            this.columnNumberCheckBox.Size = new System.Drawing.Size(101, 17);
            this.columnNumberCheckBox.TabIndex = 2;
            this.columnNumberCheckBox.Text = "Column Number";
            this.columnNumberCheckBox.UseVisualStyleBackColor = true;
            // 
            // lineCheckBox
            // 
            this.lineCheckBox.AutoSize = true;
            this.lineCheckBox.Location = new System.Drawing.Point(7, 47);
            this.lineCheckBox.Name = "lineCheckBox";
            this.lineCheckBox.Size = new System.Drawing.Size(87, 17);
            this.lineCheckBox.TabIndex = 2;
            this.lineCheckBox.Text = "Line Address";
            this.lineCheckBox.UseVisualStyleBackColor = true;
            // 
            // charCheckBox
            // 
            this.charCheckBox.AutoSize = true;
            this.charCheckBox.Location = new System.Drawing.Point(7, 20);
            this.charCheckBox.Name = "charCheckBox";
            this.charCheckBox.Size = new System.Drawing.Size(77, 17);
            this.charCheckBox.TabIndex = 0;
            this.charCheckBox.Text = "Characters";
            this.charCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.invertButton);
            this.groupBox1.Controls.Add(this.changeButton);
            this.groupBox1.Controls.Add(this.selectionColorButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.addressColorButton);
            this.groupBox1.Controls.Add(this.backColorButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.fontTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 124);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Font and Color";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(154, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Selection Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Address Color";
            // 
            // invertButton
            // 
            this.invertButton.Location = new System.Drawing.Point(238, 51);
            this.invertButton.Name = "invertButton";
            this.invertButton.Size = new System.Drawing.Size(75, 23);
            this.invertButton.TabIndex = 7;
            this.invertButton.Text = "Invert";
            this.invertButton.UseVisualStyleBackColor = true;
            this.invertButton.Click += new System.EventHandler(this.invertButton_Click);
            // 
            // changeButton
            // 
            this.changeButton.Location = new System.Drawing.Point(238, 22);
            this.changeButton.Name = "changeButton";
            this.changeButton.Size = new System.Drawing.Size(75, 23);
            this.changeButton.TabIndex = 6;
            this.changeButton.Text = "Change...";
            this.changeButton.UseVisualStyleBackColor = true;
            this.changeButton.Click += new System.EventHandler(this.changeButton_Click);
            // 
            // selectionColorButton
            // 
            this.selectionColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectionColorButton.Location = new System.Drawing.Point(238, 86);
            this.selectionColorButton.Name = "selectionColorButton";
            this.selectionColorButton.Size = new System.Drawing.Size(23, 23);
            this.selectionColorButton.TabIndex = 5;
            this.selectionColorButton.UseVisualStyleBackColor = true;
            this.selectionColorButton.Click += new System.EventHandler(this.selectionColorButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Font";
            // 
            // addressColorButton
            // 
            this.addressColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addressColorButton.Location = new System.Drawing.Point(93, 86);
            this.addressColorButton.Name = "addressColorButton";
            this.addressColorButton.Size = new System.Drawing.Size(23, 23);
            this.addressColorButton.TabIndex = 5;
            this.addressColorButton.UseVisualStyleBackColor = true;
            this.addressColorButton.Click += new System.EventHandler(this.addressColorButton_Click);
            // 
            // backColorButton
            // 
            this.backColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backColorButton.Location = new System.Drawing.Point(83, 50);
            this.backColorButton.Name = "backColorButton";
            this.backColorButton.Size = new System.Drawing.Size(23, 23);
            this.backColorButton.TabIndex = 5;
            this.backColorButton.UseVisualStyleBackColor = true;
            this.backColorButton.Click += new System.EventHandler(this.backColorButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Back Color";
            // 
            // fontTextBox
            // 
            this.fontTextBox.BackColor = System.Drawing.Color.White;
            this.fontTextBox.Location = new System.Drawing.Point(83, 24);
            this.fontTextBox.Name = "fontTextBox";
            this.fontTextBox.ReadOnly = true;
            this.fontTextBox.Size = new System.Drawing.Size(149, 20);
            this.fontTextBox.TabIndex = 4;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(262, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(181, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // colorDialog1
            // 
            this.colorDialog1.SolidColorOnly = true;
            // 
            // fontDialog1
            // 
            this.fontDialog1.FixedPitchOnly = true;
            this.fontDialog1.ShowColor = true;
            // 
            // OptionForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(353, 321);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fontTextBox;
        private System.Windows.Forms.Button backColorButton;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox charCheckBox;
        private System.Windows.Forms.CheckBox lineCheckBox;
        private System.Windows.Forms.Button changeButton;
        private System.Windows.Forms.ComboBox hexCaseComboBox;
        private System.Windows.Forms.MaskedTextBox bytesMaskedTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button invertButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addressColorButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button selectionColorButton;
        private System.Windows.Forms.MaskedTextBox groupSizeMaskedTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox byteGroupCheckBox;
        private System.Windows.Forms.CheckBox columnNumberCheckBox;


    }
}