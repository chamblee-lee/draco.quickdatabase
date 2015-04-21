using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CodeGenerator
{
    public partial class ShowErrorInf : XtraForm
    {
        private string es = "";
        public ShowErrorInf(string errorstr)
        {
            InitializeComponent();
            this.es = errorstr;
        }

        private void ShowErrorInf_Load(object sender, EventArgs e)
        {
            this.richTextBox1.Text = this.es;
        }
    }
}
