using System.Windows.Forms;
using System;
namespace UpdateControls.Forms
{
    partial class UpdateErrorProvider
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
            if (disposing && _visErrorText != null)
            {
                // Unregister idle-time updates.
                Application.Idle -= new EventHandler(Application_Idle);
                _visErrorText.Dispose();
                foreach (ControlValidater dep in _controlSentry.Values)
                    dep.Dispose();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
