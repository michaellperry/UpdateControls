namespace UpdateControls.Test
{
    partial class HeaderForm
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
            this.header = new UpdateControls.Themes.Forms.Header();
            this.focusedCheckBox = new UpdateControls.Themes.Forms.ThemedCheckBox();
            this.headerTextBox = new UpdateControls.Forms.UpdateTextBox();
            this.themedButton1 = new UpdateControls.Themes.Forms.ThemedButton();
            this.SuspendLayout();
            // 
            // header
            // 
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Location = new System.Drawing.Point(0, 0);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(292, 23);
            this.header.TabIndex = 0;
            this.header.Text = "Got focus?";
            this.header.Theme = null;
            this.header.GetFocus += new UpdateControls.Forms.GetBoolDelegate(this.header_GetFocus);
            this.header.GetText += new UpdateControls.Forms.GetStringDelegate(this.header_GetText);
            // 
            // focusedCheckBox
            // 
            this.focusedCheckBox.CheckedTheme = null;
            this.focusedCheckBox.Location = new System.Drawing.Point(13, 30);
            this.focusedCheckBox.Name = "focusedCheckBox";
            this.focusedCheckBox.Size = new System.Drawing.Size(113, 23);
            this.focusedCheckBox.TabIndex = 1;
            this.focusedCheckBox.Text = "Got focus?";
            this.focusedCheckBox.UncheckedTheme = null;
            this.focusedCheckBox.SetChecked += new UpdateControls.Forms.SetBoolDelegate(this.focusedCheckBox_SetChecked);
            this.focusedCheckBox.GetChecked += new UpdateControls.Forms.GetBoolDelegate(this.header_GetFocus);
            // 
            // headerTextBox
            // 
            this.headerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerTextBox.Location = new System.Drawing.Point(13, 60);
            this.headerTextBox.Name = "headerTextBox";
            this.headerTextBox.Size = new System.Drawing.Size(267, 20);
            this.headerTextBox.TabIndex = 2;
            this.headerTextBox.GetText += new UpdateControls.Forms.GetStringDelegate(this.header_GetText);
            this.headerTextBox.SetText += new UpdateControls.Forms.SetStringDelegate(this.headerTextBox_SetText);
            // 
            // themedButton1
            // 
            this.themedButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.themedButton1.Location = new System.Drawing.Point(13, 87);
            this.themedButton1.Name = "themedButton1";
            this.themedButton1.Size = new System.Drawing.Size(267, 23);
            this.themedButton1.TabIndex = 3;
            this.themedButton1.Text = "themedButton1";
            this.themedButton1.Theme = null;
            this.themedButton1.GetEnabled += new UpdateControls.Forms.GetBoolDelegate(this.header_GetFocus);
            // 
            // HeaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.themedButton1);
            this.Controls.Add(this.headerTextBox);
            this.Controls.Add(this.focusedCheckBox);
            this.Controls.Add(this.header);
            this.Name = "HeaderForm";
            this.Text = "HeaderForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UpdateControls.Themes.Forms.Header header;
        private UpdateControls.Themes.Forms.ThemedCheckBox focusedCheckBox;
        private UpdateControls.Forms.UpdateTextBox headerTextBox;
        private UpdateControls.Themes.Forms.ThemedButton themedButton1;
    }
}