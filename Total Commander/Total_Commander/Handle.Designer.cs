namespace Total_Commander
{
    partial class Handle
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
            this.Button_Overwrite = new System.Windows.Forms.Button();
            this.Button_Skip = new System.Windows.Forms.Button();
            this.Button_OverwriteAll = new System.Windows.Forms.Button();
            this.Button_SkipAll = new System.Windows.Forms.Button();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Button_Overwrite
            // 
            this.Button_Overwrite.Location = new System.Drawing.Point(21, 64);
            this.Button_Overwrite.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Button_Overwrite.Name = "Button_Overwrite";
            this.Button_Overwrite.Size = new System.Drawing.Size(112, 28);
            this.Button_Overwrite.TabIndex = 1;
            this.Button_Overwrite.Text = "Overwrite";
            this.Button_Overwrite.UseVisualStyleBackColor = true;
            this.Button_Overwrite.Click += new System.EventHandler(this.Button_Overwrite_Click);
            // 
            // Button_Skip
            // 
            this.Button_Skip.Location = new System.Drawing.Point(21, 20);
            this.Button_Skip.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Button_Skip.Name = "Button_Skip";
            this.Button_Skip.Size = new System.Drawing.Size(112, 28);
            this.Button_Skip.TabIndex = 1;
            this.Button_Skip.Text = "Skip";
            this.Button_Skip.UseVisualStyleBackColor = true;
            this.Button_Skip.Click += new System.EventHandler(this.Button_Skip_Click);
            // 
            // Button_OverwriteAll
            // 
            this.Button_OverwriteAll.Location = new System.Drawing.Point(154, 64);
            this.Button_OverwriteAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Button_OverwriteAll.Name = "Button_OverwriteAll";
            this.Button_OverwriteAll.Size = new System.Drawing.Size(112, 28);
            this.Button_OverwriteAll.TabIndex = 1;
            this.Button_OverwriteAll.Text = "Overwrite all";
            this.Button_OverwriteAll.UseVisualStyleBackColor = true;
            this.Button_OverwriteAll.Click += new System.EventHandler(this.Button_OverwriteAll_Click);
            // 
            // Button_SkipAll
            // 
            this.Button_SkipAll.Location = new System.Drawing.Point(154, 20);
            this.Button_SkipAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Button_SkipAll.Name = "Button_SkipAll";
            this.Button_SkipAll.Size = new System.Drawing.Size(112, 28);
            this.Button_SkipAll.TabIndex = 1;
            this.Button_SkipAll.Text = "Skip all";
            this.Button_SkipAll.UseVisualStyleBackColor = true;
            this.Button_SkipAll.Click += new System.EventHandler(this.Button_SkipAll_Click);
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.Location = new System.Drawing.Point(21, 108);
            this.Button_Cancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(245, 32);
            this.Button_Cancel.TabIndex = 1;
            this.Button_Cancel.Text = "Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            this.Button_Cancel.Click += new System.EventHandler(this.Button_Cancel_Click);
            // 
            // Hanlde
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 161);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.Button_SkipAll);
            this.Controls.Add(this.Button_OverwriteAll);
            this.Controls.Add(this.Button_Skip);
            this.Controls.Add(this.Button_Overwrite);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Hanlde";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hanlde";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Button_Overwrite;
        private System.Windows.Forms.Button Button_Skip;
        private System.Windows.Forms.Button Button_OverwriteAll;
        private System.Windows.Forms.Button Button_SkipAll;
        private System.Windows.Forms.Button Button_Cancel;
    }
}