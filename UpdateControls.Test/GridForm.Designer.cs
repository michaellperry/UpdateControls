namespace UpdateControls.Test
{
    partial class GridForm
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
            this.numbersGrid = new UpdateControls.Forms.UpdateGrid();
            this.deleteButton = new UpdateControls.Themes.Forms.ThemedButton();
            ((System.ComponentModel.ISupportInitialize)(this.numbersGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // numbersGrid
            // 
            this.numbersGrid.AllowUserToAddRows = false;
            this.numbersGrid.AllowUserToDeleteRows = false;
            this.numbersGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numbersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.numbersGrid.Location = new System.Drawing.Point(12, 12);
            this.numbersGrid.Name = "numbersGrid";
            this.numbersGrid.Size = new System.Drawing.Size(260, 211);
            this.numbersGrid.TabIndex = 0;
            this.numbersGrid.GetItems += new UpdateControls.Forms.GetCollectionDelegate(this.numbersGrid_GetItems);
            this.numbersGrid.GetColumns += new UpdateControls.Forms.GetColumnDefinitionsDelegate(this.numbersGrid_GetColumns);
            this.numbersGrid.GetCellValue += new UpdateControls.Forms.GetObjectColumnValueDelegate(this.numbersGrid_GetCellValue);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Location = new System.Drawing.Point(197, 229);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.Theme = null;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            this.deleteButton.GetEnabled += new UpdateControls.Forms.GetBoolDelegate(this.deleteButton_GetEnabled);
            // 
            // GridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.numbersGrid);
            this.Name = "GridForm";
            this.Text = "GridForm";
            ((System.ComponentModel.ISupportInitialize)(this.numbersGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UpdateControls.Forms.UpdateGrid numbersGrid;
        private UpdateControls.Themes.Forms.ThemedButton deleteButton;
    }
}