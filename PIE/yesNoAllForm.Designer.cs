namespace PIE
{
    partial class yesNoAllForm
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
            this.bodyLabel = new System.Windows.Forms.Label();
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.yesAllbutton = new System.Windows.Forms.Button();
            this.noAllbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bodyLabel
            // 
            this.bodyLabel.AutoSize = true;
            this.bodyLabel.Location = new System.Drawing.Point(13, 13);
            this.bodyLabel.Name = "bodyLabel";
            this.bodyLabel.Size = new System.Drawing.Size(35, 13);
            this.bodyLabel.TabIndex = 0;
            this.bodyLabel.Text = "label1";
            // 
            // yesButton
            // 
            this.yesButton.Location = new System.Drawing.Point(12, 80);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
            // 
            // noButton
            // 
            this.noButton.Location = new System.Drawing.Point(174, 80);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.TabIndex = 2;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            this.noButton.Click += new System.EventHandler(this.noButton_Click);
            // 
            // yesAllbutton
            // 
            this.yesAllbutton.Location = new System.Drawing.Point(93, 80);
            this.yesAllbutton.Name = "yesAllbutton";
            this.yesAllbutton.Size = new System.Drawing.Size(75, 23);
            this.yesAllbutton.TabIndex = 3;
            this.yesAllbutton.Text = "Yes to All";
            this.yesAllbutton.UseVisualStyleBackColor = true;
            this.yesAllbutton.Click += new System.EventHandler(this.yesAllbutton_Click);
            // 
            // noAllbutton
            // 
            this.noAllbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.noAllbutton.Location = new System.Drawing.Point(255, 80);
            this.noAllbutton.Name = "noAllbutton";
            this.noAllbutton.Size = new System.Drawing.Size(75, 23);
            this.noAllbutton.TabIndex = 4;
            this.noAllbutton.Text = "No to All";
            this.noAllbutton.UseVisualStyleBackColor = true;
            this.noAllbutton.Click += new System.EventHandler(this.noAllbutton_Click);
            // 
            // yesNoAllForm
            // 
            this.AcceptButton = this.yesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.noAllbutton;
            this.ClientSize = new System.Drawing.Size(355, 125);
            this.Controls.Add(this.noAllbutton);
            this.Controls.Add(this.yesAllbutton);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.bodyLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "yesNoAllForm";
            this.Text = "yesNoAllForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label bodyLabel;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.Button yesAllbutton;
        private System.Windows.Forms.Button noAllbutton;
    }
}