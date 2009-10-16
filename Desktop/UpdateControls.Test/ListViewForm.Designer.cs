namespace UpdateControls.Test
{
    partial class ListViewForm
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
            this.personListView = new UpdateControls.Forms.UpdateListView();
            this.SuspendLayout();
            // 
            // personListView
            // 
            this.personListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.personListView.Location = new System.Drawing.Point(0, 0);
            this.personListView.Name = "personListView";
            this.personListView.Size = new System.Drawing.Size(284, 264);
            this.personListView.TabIndex = 0;
            this.personListView.UseCompatibleStateImageBehavior = false;
            this.personListView.GetGroupName += new UpdateControls.Forms.GetObjectStringDelegate(this.personListView_GetGroupName);
            this.personListView.GetItemText += new UpdateControls.Forms.GetObjectStringDelegate(this.personListView_GetItemText);
            this.personListView.GetGroupAlignment += new UpdateControls.Forms.GetObjectHorizontalAlignmentDelegate(this.personListView_GetGroupAlignment);
            this.personListView.GetItems += new UpdateControls.Forms.GetCollectionDelegate(this.personListView_GetItems);
            this.personListView.GetGroups += new UpdateControls.Forms.GetCollectionDelegate(this.personListView_GetGroups);
            this.personListView.GetItemGroup += new UpdateControls.Forms.GetObjectObjectDelegate(this.personListView_GetItemGroup);
            // 
            // ListViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.personListView);
            this.Name = "ListViewForm";
            this.Text = "ListViewForm";
            this.ResumeLayout(false);

        }

        #endregion

        private UpdateControls.Forms.UpdateListView personListView;
    }
}