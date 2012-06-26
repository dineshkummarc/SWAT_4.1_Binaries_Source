using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor
{
    public partial class DDEditorForm : Form
    {
        public DDEditorForm()
        {
            InitializeComponent();
        }

        public void DDEditorForm_Resize(Object sender, EventArgs e)
        {
            ddEditor1.Size = this.Size - new Size(20, 35);
        }

        private void DDEditorForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}
