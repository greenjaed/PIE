﻿namespace PIE
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.notesTextBox = new System.Windows.Forms.TextBox();
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
            this.okButton.Location = new System.Drawing.Point(226, 173);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(307, 173);
            // 
            // endTextBox
            // 
            this.endTextBox.Location = new System.Drawing.Point(211, 43);
            // 
            // startTextBox
            // 
            this.startTextBox.Location = new System.Drawing.Point(53, 43);
            // 
            // sizeLabel
            // 
            this.sizeLabel.Location = new System.Drawing.Point(178, 46);
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.Location = new System.Drawing.Point(211, 42);
            // 
            // bytesComboBox
            // 
            this.bytesComboBox.Location = new System.Drawing.Point(280, 42);
            // 
            // bytesLabel
            // 
            this.bytesLabel.Location = new System.Drawing.Point(346, 46);
            // 
            // bySizeCheckBox
            // 
            this.bySizeCheckBox.Location = new System.Drawing.Point(12, 177);
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
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(9, 69);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(35, 13);
            this.infoLabel.TabIndex = 9;
            this.infoLabel.Text = "Notes";
            // 
            // notesTextBox
            // 
            this.notesTextBox.Location = new System.Drawing.Point(11, 85);
            this.notesTextBox.Multiline = true;
            this.notesTextBox.Name = "notesTextBox";
            this.notesTextBox.Size = new System.Drawing.Size(371, 82);
            this.notesTextBox.TabIndex = 10;
            // 
            // SliceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 209);
            this.Controls.Add(this.notesTextBox);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameLabel);
            this.Name = "SliceForm";
            this.Text = "Slice";
            this.Controls.SetChildIndex(this.sizeLabel, 0);
            this.Controls.SetChildIndex(this.sizeComboBox, 0);
            this.Controls.SetChildIndex(this.bytesComboBox, 0);
            this.Controls.SetChildIndex(this.bytesLabel, 0);
            this.Controls.SetChildIndex(this.bySizeCheckBox, 0);
            this.Controls.SetChildIndex(this.endTextBox, 0);
            this.Controls.SetChildIndex(this.endLabel, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.startTextBox, 0);
            this.Controls.SetChildIndex(this.startLabel, 0);
            this.Controls.SetChildIndex(this.nameLabel, 0);
            this.Controls.SetChildIndex(this.nameTextBox, 0);
            this.Controls.SetChildIndex(this.infoLabel, 0);
            this.Controls.SetChildIndex(this.notesTextBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.TextBox notesTextBox;
    }
}