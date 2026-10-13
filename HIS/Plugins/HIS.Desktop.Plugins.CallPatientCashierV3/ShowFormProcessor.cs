using Inventec.Common.Logging;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    class ShowFormProcessor
    {
        /// <summary>Mở màn chờ toàn màn hình không viền trên màn hình mở rộng; không có thì báo và mở trên màn hình chính (R15).</summary>
        internal static void ShowFormInExtendMonitor(Form control)
        {
            try
            {
                Screen[] sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng", "Thông báo");
                    control.StartPosition = FormStartPosition.CenterScreen;
                    control.Show();
                }
                else
                {
                    Screen sc2 = sc.FirstOrDefault(o => !o.Primary);
                    if (sc2 == null) sc2 = sc[1];
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = new Point(sc2.Bounds.Location.X, sc2.Bounds.Location.Y);
                    control.Size = sc2.Bounds.Size;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
