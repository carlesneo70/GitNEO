using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitNEO
{
    public partial class FrmDialog : Form
    {
        public String InputValue
        {
            get { return textBoxResponseValue.Text; }
            set { textBoxResponseValue.Text = value; }
        }

        public FrmDialog(string title, string text, string value = "")
        {
            InitializeComponent();

            Text = title;
            labelText.Text = text;
            textBoxResponseValue.Text = value;
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FrmDialog_Load(object sender, EventArgs e)
        {
            textBoxResponseValue.Focus();
        }
    }
}
