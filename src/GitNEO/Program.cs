using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitNEO
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Buat instance dari MainForm
            FrmMain frmMain = new FrmMain();

            // Jalankan aplikasi tanpa menampilkan form utama
            Application.Run();
        }
    }
}
