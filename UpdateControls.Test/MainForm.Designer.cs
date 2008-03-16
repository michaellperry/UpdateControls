namespace UpdateControls.Test
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.logo1 = new UpdateControls.Themes.Forms.Logo();
            this.themedTabDeck1 = new UpdateControls.Themes.Forms.ThemedTabDeck(this.components);
            this.theme1 = new UpdateControls.Themes.Forms.Theme(this.components);
            this.topRadioButton = new UpdateControls.Themes.Forms.ThemedRadioButton();
            this.leftRadioButton = new UpdateControls.Themes.Forms.ThemedRadioButton();
            this.rightRadioButton = new UpdateControls.Themes.Forms.ThemedRadioButton();
            this.bottomRadioButton = new UpdateControls.Themes.Forms.ThemedRadioButton();
            this.shuffleButton = new UpdateControls.Themes.Forms.ThemedButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // logo1
            // 
            this.logo1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logo1.HelixColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(206)))), ((int)(((byte)(0)))));
            this.logo1.Location = new System.Drawing.Point(553, 12);
            this.logo1.Name = "logo1";
            this.logo1.Size = new System.Drawing.Size(75, 75);
            this.logo1.TabIndex = 11;
            this.logo1.TabStop = false;
            this.logo1.Text = "logo1";
            this.logo1.Theme = null;
            // 
            // themedTabDeck1
            // 
            this.themedTabDeck1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.themedTabDeck1.CloseTheme = null;
            this.themedTabDeck1.HasImages = true;
            this.themedTabDeck1.Location = new System.Drawing.Point(13, 12);
            this.themedTabDeck1.Name = "themedTabDeck1";
            this.themedTabDeck1.OwnsImages = true;
            this.themedTabDeck1.Size = new System.Drawing.Size(534, 315);
            this.themedTabDeck1.TabIndex = 12;
            this.themedTabDeck1.TabStop = false;
            this.themedTabDeck1.Text = "themedTabDeck1";
            this.themedTabDeck1.Theme = this.theme1;
            this.themedTabDeck1.GetItemImage += new UpdateControls.Themes.GetObjectImageDelegate(this.themedTabDeck1_GetItemImage);
            this.themedTabDeck1.GetItems += new UpdateControls.Forms.GetCollectionDelegate(this.themedTabDeck1_GetItems);
            this.themedTabDeck1.CreateContent += new UpdateControls.Themes.CreateControlDelegate(this.themedTabDeck1_CreateContent);
            this.themedTabDeck1.CloseTab += new UpdateControls.Forms.ObjectActionDelegate(this.themedTabDeck1_CloseTab);
            this.themedTabDeck1.SetTabPosition += new UpdateControls.Themes.SetObjectIntegerDelegate(this.themedTabDeck1_SetTabPosition);
            // 
            // theme1
            // 
            this.theme1.ColorBody = System.Drawing.Color.Black;
            this.theme1.ColorEven = System.Drawing.Color.Snow;
            this.theme1.ColorHeader = System.Drawing.Color.Black;
            this.theme1.ColorOdd = System.Drawing.Color.WhiteSmoke;
            this.theme1.ColorSelected = System.Drawing.Color.BlanchedAlmond;
            this.theme1.FontBody = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.theme1.FontHeader = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.theme1.SolidDisabled = ((UpdateControls.Themes.SolidDescriptorEditable)(resources.GetObject("theme1.SolidDisabled")));
            this.theme1.SolidFocused = ((UpdateControls.Themes.SolidDescriptorEditable)(resources.GetObject("theme1.SolidFocused")));
            this.theme1.SolidHover = ((UpdateControls.Themes.SolidDescriptorEditable)(resources.GetObject("theme1.SolidHover")));
            this.theme1.SolidPressed = ((UpdateControls.Themes.SolidDescriptorEditable)(resources.GetObject("theme1.SolidPressed")));
            this.theme1.SolidRegular = ((UpdateControls.Themes.SolidDescriptorEditable)(resources.GetObject("theme1.SolidRegular")));
            // 
            // topRadioButton
            // 
            this.topRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.topRadioButton.CheckedTheme = null;
            this.topRadioButton.Location = new System.Drawing.Point(553, 93);
            this.topRadioButton.Name = "topRadioButton";
            this.topRadioButton.Size = new System.Drawing.Size(75, 23);
            this.topRadioButton.TabIndex = 13;
            this.topRadioButton.Text = "Top";
            this.topRadioButton.UncheckedTheme = null;
            this.topRadioButton.SetChecked += new UpdateControls.Forms.ActionDelegate(this.topRadioButton_SetChecked);
            this.topRadioButton.GetChecked += new UpdateControls.Forms.GetBoolDelegate(this.topRadioButton_GetChecked);
            // 
            // leftRadioButton
            // 
            this.leftRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.leftRadioButton.CheckedTheme = null;
            this.leftRadioButton.Location = new System.Drawing.Point(553, 122);
            this.leftRadioButton.Name = "leftRadioButton";
            this.leftRadioButton.Size = new System.Drawing.Size(75, 23);
            this.leftRadioButton.TabIndex = 14;
            this.leftRadioButton.Text = "Left";
            this.leftRadioButton.UncheckedTheme = null;
            this.leftRadioButton.SetChecked += new UpdateControls.Forms.ActionDelegate(this.leftRadioButton_SetChecked);
            this.leftRadioButton.GetChecked += new UpdateControls.Forms.GetBoolDelegate(this.leftRadioButton_GetChecked);
            // 
            // rightRadioButton
            // 
            this.rightRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rightRadioButton.CheckedTheme = null;
            this.rightRadioButton.Location = new System.Drawing.Point(553, 151);
            this.rightRadioButton.Name = "rightRadioButton";
            this.rightRadioButton.Size = new System.Drawing.Size(75, 23);
            this.rightRadioButton.TabIndex = 15;
            this.rightRadioButton.Text = "Right";
            this.rightRadioButton.UncheckedTheme = null;
            this.rightRadioButton.SetChecked += new UpdateControls.Forms.ActionDelegate(this.rightRadioButton_SetChecked);
            this.rightRadioButton.GetChecked += new UpdateControls.Forms.GetBoolDelegate(this.rightRadioButton_GetChecked);
            // 
            // bottomRadioButton
            // 
            this.bottomRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomRadioButton.CheckedTheme = null;
            this.bottomRadioButton.Location = new System.Drawing.Point(553, 180);
            this.bottomRadioButton.Name = "bottomRadioButton";
            this.bottomRadioButton.Size = new System.Drawing.Size(75, 23);
            this.bottomRadioButton.TabIndex = 16;
            this.bottomRadioButton.Text = "Bottom";
            this.bottomRadioButton.UncheckedTheme = null;
            this.bottomRadioButton.SetChecked += new UpdateControls.Forms.ActionDelegate(this.bottomRadioButton_SetChecked);
            this.bottomRadioButton.GetChecked += new UpdateControls.Forms.GetBoolDelegate(this.bottomRadioButton_GetChecked);
            // 
            // shuffleButton
            // 
            this.shuffleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.shuffleButton.Location = new System.Drawing.Point(553, 209);
            this.shuffleButton.Name = "shuffleButton";
            this.shuffleButton.Size = new System.Drawing.Size(75, 23);
            this.shuffleButton.TabIndex = 17;
            this.shuffleButton.Text = "Shuffle";
            this.shuffleButton.Theme = null;
            this.shuffleButton.Click += new System.EventHandler(this.shuffleButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ahr_C3_Straight_Closed.png");
            this.imageList1.Images.SetKeyName(1, "ahr_HHB_Straight_Closed.png");
            this.imageList1.Images.SetKeyName(2, "ahr_T_Straight_Closed.png");
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(640, 339);
            this.Controls.Add(this.shuffleButton);
            this.Controls.Add(this.bottomRadioButton);
            this.Controls.Add(this.rightRadioButton);
            this.Controls.Add(this.leftRadioButton);
            this.Controls.Add(this.topRadioButton);
            this.Controls.Add(this.themedTabDeck1);
            this.Controls.Add(this.logo1);
            this.Name = "MainForm";
            this.Text = "Glide Scrolling";
            this.ResumeLayout(false);

        }

        #endregion

        private UpdateControls.Themes.Forms.Logo logo1;
        private UpdateControls.Themes.Forms.ThemedTabDeck themedTabDeck1;
        private UpdateControls.Themes.Forms.ThemedRadioButton topRadioButton;
        private UpdateControls.Themes.Forms.ThemedRadioButton leftRadioButton;
        private UpdateControls.Themes.Forms.ThemedRadioButton rightRadioButton;
        private UpdateControls.Themes.Forms.ThemedRadioButton bottomRadioButton;
        private UpdateControls.Themes.Forms.ThemedButton shuffleButton;
        private System.Windows.Forms.ImageList imageList1;
        private UpdateControls.Themes.Forms.Theme theme1;


    }
}

