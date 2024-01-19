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
    public partial class FrmToast : Form
    {
        int toastX, toastY;

        public FrmToast(string type, string message)
        {
            InitializeComponent();

            label1.Text = type;
            label2.Text = message;
            switch (type)
            {

                case "SUCCESS":
                    panel1.BackColor = Color.FromArgb(57, 155, 53);
                    pictureBox1.Image = Properties.Resources.ok_48px;
                    break;
                case "ERROR":
                    panel1.BackColor = Color.FromArgb(227, 50, 45);
                    pictureBox1.Image = Properties.Resources.cancel_48px;
                    break;
                case "INFO":
                    panel1.BackColor = Color.FromArgb(18, 136, 191);
                    pictureBox1.Image = Properties.Resources.info_48px;
                    break;
                case "WARNING":
                    panel1.BackColor = Color.FromArgb(245, 171, 35);
                    pictureBox1.Image = Properties.Resources.error_48px;
                    break;
            }
        }

        private void FrmToast_Load(object sender, EventArgs e)
        {
            Position();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toastY -= 10;
            this.Location = new Point(toastX, toastY);
            if (toastY <= Screen.FromControl(this).WorkingArea.Bottom - this.Height - 10)
            {
                timer1.Stop();
                timer2.Start();
            }
        }
        int y = 100;

        private void timer2_Tick(object sender, EventArgs e)
        {
            y--;
            if (y <= 0)
            {
                toastY += 1;
                this.Location = new Point(toastX, toastY += 10);
                if (toastY >= 800)
                {
                    timer2.Stop();
                    y = 100;
                    this.Close();
                }
            }
        }

        private void Position()
        {
            //int ScreenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            //int ScreenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            Screen rightmost = Screen.AllScreens[0];
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
                    rightmost = screen;
            }

            int ScreenWidth = rightmost.WorkingArea.Right;
            int ScreenHeight = rightmost.WorkingArea.Bottom;

            toastX = ScreenWidth - this.Width - 5;
            toastY = ScreenHeight - this.Height + 70;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(toastX, toastY);
            this.TopMost = true;
        }
    }
}
