using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitNEO
{
    public static class Utils
    {
        public static void showToast(string type, string message)
        {
            FrmToast toast = new FrmToast(type, message);
            toast.Show();
        }

        public static string appPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
            //Directory.GetCurrentDirectory()
        }
    }
}
