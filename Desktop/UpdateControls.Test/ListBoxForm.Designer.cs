namespace UpdateControls.Test
{
    partial class ListBoxForm
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
            this.personListBox = new UpdateControls.Forms.UpdateListBox();
            this.SuspendLayout();
            // 
            // personListBox
            // 
            this.personListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.personListBox.FormattingEnabled = true;
            this.personListBox.Location = new System.Drawing.Point(12, 12);
            this.personListBox.Name = "personListBox";
            this.personListBox.Size = new System.Drawing.Size(260, 238);
            this.personListBox.TabIndex = 0;
            this.personListBox.GetItems += new UpdateControls.Forms.GetCollectionDelegate(this.personListBox_GetItems);
            this.personListBox.GetItemText += new UpdateControls.Forms.GetObjectStringDelegate(this.personListBox_GetItemText);
            // 
            // ListBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.personListBox);
            this.Name = "ListBoxForm";
            this.Text = "ListBoxForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Forms.UpdateListBox personListBox;
    }
}