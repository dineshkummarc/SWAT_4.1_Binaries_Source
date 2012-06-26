namespace SWAT_Editor
{
    partial class DDEditorForm
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
            this.ddEditor1 = new SWAT_Editor.Controls.DDEditor();
            this.SuspendLayout();
            // 
            // ddEditor1
            // 
            this.ddEditor1.AutoScroll = true;
            this.ddEditor1.Location = new System.Drawing.Point(24, -12);
            this.ddEditor1.Name = "ddEditor1";
            this.ddEditor1.Size = new System.Drawing.Size(693, 476);
            this.ddEditor1.TabIndex = 0;
            // 
            // DDEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 462);
            this.Controls.Add(this.ddEditor1);
            this.MinimumSize = new System.Drawing.Size(751, 496);
            this.Name = "DDEditorForm";
            this.Text = "Data Driven Editor";
            this.Load += new System.EventHandler(this.DDEditorForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.DDEditor ddEditor1;

    }
}