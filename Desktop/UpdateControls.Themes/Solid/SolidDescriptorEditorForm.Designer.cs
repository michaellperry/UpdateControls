namespace UpdateControls.Themes.Solid
{
    partial class SolidDescriptorEditorForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.aperatureTrackBar = new System.Windows.Forms.TrackBar();
            this.ambienceTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.directionPictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.transparentCheckBox = new System.Windows.Forms.CheckBox();
            this.thicknessTrackBar = new System.Windows.Forms.TrackBar();
            this.colorPictureBox = new System.Windows.Forms.PictureBox();
            this.sharpnessTrackBar = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.colorButton = new System.Windows.Forms.Button();
            this.refractionTrackBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tilePreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.ellipsePreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.previewTabControl = new System.Windows.Forms.TabControl();
            this.tileTabPage = new System.Windows.Forms.TabPage();
            this.tabTabPage = new System.Windows.Forms.TabPage();
            this.tabPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.ellipseTabPage = new System.Windows.Forms.TabPage();
            this.idleTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aperatureTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ambienceTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionPictureBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thicknessTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sharpnessTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.refractionTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tilePreviewPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ellipsePreviewPictureBox)).BeginInit();
            this.previewTabControl.SuspendLayout();
            this.tileTabPage.SuspendLayout();
            this.tabTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabPreviewPictureBox)).BeginInit();
            this.ellipseTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.aperatureTrackBar);
            this.groupBox1.Controls.Add(this.ambienceTrackBar);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.directionPictureBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 169);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 247);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Light Source";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Direction:";
            // 
            // aperatureTrackBar
            // 
            this.aperatureTrackBar.Location = new System.Drawing.Point(70, 176);
            this.aperatureTrackBar.Maximum = 100;
            this.aperatureTrackBar.Name = "aperatureTrackBar";
            this.aperatureTrackBar.Size = new System.Drawing.Size(104, 45);
            this.aperatureTrackBar.TabIndex = 4;
            this.aperatureTrackBar.TickFrequency = 10;
            this.aperatureTrackBar.Scroll += new System.EventHandler(this.aperatureTrackBar_Scroll);
            // 
            // ambienceTrackBar
            // 
            this.ambienceTrackBar.Location = new System.Drawing.Point(70, 125);
            this.ambienceTrackBar.Maximum = 100;
            this.ambienceTrackBar.Name = "ambienceTrackBar";
            this.ambienceTrackBar.Size = new System.Drawing.Size(104, 45);
            this.ambienceTrackBar.TabIndex = 2;
            this.ambienceTrackBar.TickFrequency = 10;
            this.ambienceTrackBar.Scroll += new System.EventHandler(this.ambienceTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 176);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Aperature:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ambience:";
            // 
            // directionPictureBox
            // 
            this.directionPictureBox.Location = new System.Drawing.Point(70, 19);
            this.directionPictureBox.Name = "directionPictureBox";
            this.directionPictureBox.Size = new System.Drawing.Size(100, 100);
            this.directionPictureBox.TabIndex = 0;
            this.directionPictureBox.TabStop = false;
            this.directionPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.directionPictureBox_MouseDown);
            this.directionPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.directionPictureBox_MouseMove);
            this.directionPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.directionPictureBox_Paint);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.transparentCheckBox);
            this.groupBox2.Controls.Add(this.thicknessTrackBar);
            this.groupBox2.Controls.Add(this.colorPictureBox);
            this.groupBox2.Controls.Add(this.sharpnessTrackBar);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.colorButton);
            this.groupBox2.Controls.Add(this.refractionTrackBar);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(199, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(205, 247);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Material";
            // 
            // transparentCheckBox
            // 
            this.transparentCheckBox.AutoSize = true;
            this.transparentCheckBox.Location = new System.Drawing.Point(9, 151);
            this.transparentCheckBox.Name = "transparentCheckBox";
            this.transparentCheckBox.Size = new System.Drawing.Size(83, 17);
            this.transparentCheckBox.TabIndex = 5;
            this.transparentCheckBox.Text = "Transparent";
            this.transparentCheckBox.UseVisualStyleBackColor = true;
            this.transparentCheckBox.CheckedChanged += new System.EventHandler(this.transparentCheckBox_CheckedChanged);
            // 
            // thicknessTrackBar
            // 
            this.thicknessTrackBar.Location = new System.Drawing.Point(88, 49);
            this.thicknessTrackBar.Maximum = 100;
            this.thicknessTrackBar.Name = "thicknessTrackBar";
            this.thicknessTrackBar.Size = new System.Drawing.Size(104, 45);
            this.thicknessTrackBar.TabIndex = 2;
            this.thicknessTrackBar.TickFrequency = 10;
            this.thicknessTrackBar.Scroll += new System.EventHandler(this.thicknessTrackBar_Scroll);
            // 
            // colorPictureBox
            // 
            this.colorPictureBox.Location = new System.Drawing.Point(88, 19);
            this.colorPictureBox.Name = "colorPictureBox";
            this.colorPictureBox.Size = new System.Drawing.Size(104, 23);
            this.colorPictureBox.TabIndex = 7;
            this.colorPictureBox.TabStop = false;
            // 
            // sharpnessTrackBar
            // 
            this.sharpnessTrackBar.Location = new System.Drawing.Point(88, 100);
            this.sharpnessTrackBar.Maximum = 100;
            this.sharpnessTrackBar.Name = "sharpnessTrackBar";
            this.sharpnessTrackBar.Size = new System.Drawing.Size(104, 45);
            this.sharpnessTrackBar.TabIndex = 4;
            this.sharpnessTrackBar.TickFrequency = 10;
            this.sharpnessTrackBar.Scroll += new System.EventHandler(this.sharpnessTrackBar_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Sharpness:";
            // 
            // colorButton
            // 
            this.colorButton.Location = new System.Drawing.Point(6, 19);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(75, 23);
            this.colorButton.TabIndex = 0;
            this.colorButton.Text = "Color";
            this.colorButton.UseVisualStyleBackColor = true;
            this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // refractionTrackBar
            // 
            this.refractionTrackBar.Location = new System.Drawing.Point(88, 176);
            this.refractionTrackBar.Maximum = 100;
            this.refractionTrackBar.Name = "refractionTrackBar";
            this.refractionTrackBar.Size = new System.Drawing.Size(104, 45);
            this.refractionTrackBar.TabIndex = 7;
            this.refractionTrackBar.TickFrequency = 10;
            this.refractionTrackBar.Scroll += new System.EventHandler(this.refractionTrackBar_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 181);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Refraction:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Thickness:";
            // 
            // tilePreviewPictureBox
            // 
            this.tilePreviewPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePreviewPictureBox.Location = new System.Drawing.Point(3, 3);
            this.tilePreviewPictureBox.Name = "tilePreviewPictureBox";
            this.tilePreviewPictureBox.Size = new System.Drawing.Size(378, 119);
            this.tilePreviewPictureBox.TabIndex = 2;
            this.tilePreviewPictureBox.TabStop = false;
            this.tilePreviewPictureBox.Resize += new System.EventHandler(this.ImageResize);
            // 
            // ellipsePreviewPictureBox
            // 
            this.ellipsePreviewPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ellipsePreviewPictureBox.Location = new System.Drawing.Point(3, 3);
            this.ellipsePreviewPictureBox.Name = "ellipsePreviewPictureBox";
            this.ellipsePreviewPictureBox.Size = new System.Drawing.Size(378, 119);
            this.ellipsePreviewPictureBox.TabIndex = 3;
            this.ellipsePreviewPictureBox.TabStop = false;
            this.ellipsePreviewPictureBox.Resize += new System.EventHandler(this.ImageResize);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(248, 422);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(329, 422);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // previewTabControl
            // 
            this.previewTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.previewTabControl.Controls.Add(this.tileTabPage);
            this.previewTabControl.Controls.Add(this.tabTabPage);
            this.previewTabControl.Controls.Add(this.ellipseTabPage);
            this.previewTabControl.Location = new System.Drawing.Point(12, 12);
            this.previewTabControl.Name = "previewTabControl";
            this.previewTabControl.SelectedIndex = 0;
            this.previewTabControl.Size = new System.Drawing.Size(392, 151);
            this.previewTabControl.TabIndex = 0;
            this.previewTabControl.SelectedIndexChanged += new System.EventHandler(this.previewTabControl_SelectedIndexChanged);
            // 
            // tileTabPage
            // 
            this.tileTabPage.Controls.Add(this.tilePreviewPictureBox);
            this.tileTabPage.Location = new System.Drawing.Point(4, 22);
            this.tileTabPage.Name = "tileTabPage";
            this.tileTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tileTabPage.Size = new System.Drawing.Size(384, 125);
            this.tileTabPage.TabIndex = 0;
            this.tileTabPage.Text = "Tile";
            this.tileTabPage.UseVisualStyleBackColor = true;
            // 
            // tabTabPage
            // 
            this.tabTabPage.Controls.Add(this.tabPreviewPictureBox);
            this.tabTabPage.Location = new System.Drawing.Point(4, 22);
            this.tabTabPage.Name = "tabTabPage";
            this.tabTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabTabPage.Size = new System.Drawing.Size(384, 125);
            this.tabTabPage.TabIndex = 2;
            this.tabTabPage.Text = "Tab";
            this.tabTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPreviewPictureBox
            // 
            this.tabPreviewPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPreviewPictureBox.Location = new System.Drawing.Point(3, 3);
            this.tabPreviewPictureBox.Name = "tabPreviewPictureBox";
            this.tabPreviewPictureBox.Size = new System.Drawing.Size(378, 119);
            this.tabPreviewPictureBox.TabIndex = 0;
            this.tabPreviewPictureBox.TabStop = false;
            this.tabPreviewPictureBox.Resize += new System.EventHandler(this.ImageResize);
            // 
            // ellipseTabPage
            // 
            this.ellipseTabPage.Controls.Add(this.ellipsePreviewPictureBox);
            this.ellipseTabPage.Location = new System.Drawing.Point(4, 22);
            this.ellipseTabPage.Name = "ellipseTabPage";
            this.ellipseTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ellipseTabPage.Size = new System.Drawing.Size(384, 125);
            this.ellipseTabPage.TabIndex = 1;
            this.ellipseTabPage.Text = "Ellipse";
            this.ellipseTabPage.UseVisualStyleBackColor = true;
            // 
            // idleTimer
            // 
            this.idleTimer.Enabled = true;
            this.idleTimer.Interval = 200;
            this.idleTimer.Tick += new System.EventHandler(this.idleTimer_Tick);
            // 
            // SolidDescriptorEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 452);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.previewTabControl);
            this.Name = "SolidDescriptorEditorForm";
            this.Text = "Solid";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aperatureTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ambienceTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionPictureBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thicknessTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sharpnessTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.refractionTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tilePreviewPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ellipsePreviewPictureBox)).EndInit();
            this.previewTabControl.ResumeLayout(false);
            this.tileTabPage.ResumeLayout(false);
            this.tabTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabPreviewPictureBox)).EndInit();
            this.ellipseTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar aperatureTrackBar;
        private System.Windows.Forms.TrackBar ambienceTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox directionPictureBox;
        private System.Windows.Forms.TrackBar refractionTrackBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar sharpnessTrackBar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.PictureBox colorPictureBox;
        private System.Windows.Forms.PictureBox tilePreviewPictureBox;
        private System.Windows.Forms.PictureBox ellipsePreviewPictureBox;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl previewTabControl;
        private System.Windows.Forms.TabPage tileTabPage;
        private System.Windows.Forms.TabPage ellipseTabPage;
        private System.Windows.Forms.TrackBar thicknessTrackBar;
        private System.Windows.Forms.Timer idleTimer;
        private System.Windows.Forms.TabPage tabTabPage;
        private System.Windows.Forms.PictureBox tabPreviewPictureBox;
        private System.Windows.Forms.CheckBox transparentCheckBox;
    }
}