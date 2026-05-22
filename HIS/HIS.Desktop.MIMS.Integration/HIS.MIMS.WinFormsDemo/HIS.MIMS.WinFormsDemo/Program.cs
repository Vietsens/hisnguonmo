using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.MIMS.WinFormsDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                MimsDemoCacheLoader.LoadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load demo cache: " + ex.Message, "Init Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new frmMimsServerHealthCheck());
        }
    }
}
