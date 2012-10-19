using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmbeddedOffice
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            msWordControl1.New();
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            msWordControl1.UnloadWindow();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            msWordControl1.CloseDocument();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            var fDialog = new OpenFileDialog();
            if (fDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            msWordControl1.OpenFile(fDialog.FileName);
        }
    }
}
