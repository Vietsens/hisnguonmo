/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientNumOrder
{
    class ShowFormProcessor
    {

        internal static void ShowFormInExtendMonitor(Form control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                sc = sc.OrderBy(o => !o.Primary).ToArray();
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = sc[1].Bounds.Width;
                    control.Top = sc[1].Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = sc[1].Bounds.Location;
                    Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                    control.Location = p;
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
