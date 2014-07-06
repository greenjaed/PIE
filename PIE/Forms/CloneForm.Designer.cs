namespace PIE
{
    partial class CloneForm
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
            this.cloneButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.startTextBox = new System.Windows.Forms.TextBox();
            this.subSliceCheckBox = new System.Windows.Forms.CheckBox();
            this.repeatCheckBox = new System.Windows.Forms.CheckBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.copiesTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // cloneButton
            // 
            this.cloneButton.Location = new System.Drawing.Point(12, 115);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(75, 23);
            this.cloneButton.TabIndex = 0;
            this.cloneButton.Text = "Clone";
            this.cloneButton.UseVisualStyleBackColor = true;
            this.cloneButton.Click += new System.EventHandler(this.cloneButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(93, 115);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start";
            // 
            // startTextBox
            // 
            this.startTextBox.Location = new System.Drawing.Point(58, 17);
            this.startTextBox.Name = "startTextBox";
            this.startTextBox.Size = new System.Drawing.Size(100, 20);
            this.startTextBox.TabIndex = 2;
            this.startTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.startTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.startTextBox_Validating);
            // 
            // subSliceCheckBox
            // 
            this.subSliceCheckBox.AutoSize = true;
            this.subSliceCheckBox.Location = new System.Drawing.Point(12, 92);
            this.subSliceCheckBox.Name = "subSliceCheckBox";
            this.subSliceCheckBox.Size = new System.Drawing.Size(114, 17);
            this.subSliceCheckBox.TabIndex = 3;
            this.subSliceCheckBox.Text = "Include Sub Slices";
            this.subSliceCheckBox.UseVisualStyleBackColor = true;
            // 
            // repeatCheckBox
            // 
            this.repeatCheckBox.AutoSize = true;
            this.repeatCheckBox.Location = new System.Drawing.Point(76, 69);
            this.repeatCheckBox.Name = "repeatCheckBox";
            this.repeatCheckBox.Size = new System.Drawing.Size(82, 17);
            this.repeatCheckBox.TabIndex = 4;
            this.repeatCheckBox.Text = "Fill the Slice";
            this.repeatCheckBox.UseVisualStyleBackColor = true;
            this.repeatCheckBox.CheckedChanged += new System.EventHandler(this.repeatCheckBox_CheckedChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Copies";
            // 
            // copiesTextBox
            // 
            this.copiesTextBox.Location = new System.Drawing.Point(58, 43);
            this.copiesTextBox.Name = "copiesTextBox";
            this.copiesTextBox.Size = new System.Drawing.Size(100, 20);
            this.copiesTextBox.TabIndex = 6;
            this.copiesTextBox.Text = "1";
            this.copiesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.copiesTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.copiesTextBox_Validating);
            // 
            // CloneForm
            // 
            this.AcceptButton = this.cloneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(194, 158);
            this.Controls.Add(this.copiesTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.repeatCheckBox);
            this.Controls.Add(this.subSliceCheckBox);
            this.Controls.Add(this.startTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.cloneButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CloneForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Clone";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cloneButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox startTextBox;
        private System.Windows.Forms.CheckBox subSliceCheckBox;
        private System.Windows.Forms.CheckBox repeatCheckBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TextBox copiesTextBox;
        private System.Windows.Forms.Label label2;
    }
}