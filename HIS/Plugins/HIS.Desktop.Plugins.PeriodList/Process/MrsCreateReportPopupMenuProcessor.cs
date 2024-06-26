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
using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PeriodList.Process
{
    delegate void MouseRight_Click(object sender, ItemClickEventArgs e);

    class MrsCreateReportPopupMenuProcessor
    {
        MouseRight_Click mouse_Click;
        BarManager barManager;
        PopupMenu menu;

        internal MrsCreateReportPopupMenuProcessor(MouseRight_Click _click, BarManager barManager)
        {
            this.mouse_Click = _click;
            this.barManager = barManager;
        }

        internal enum PrintType
        {
            TaoBaoCaoHoachToanDoanhThu,
            TaoBaoCaoTheoKhoaChiDinh,
            TaoBaoCaoTheoKhoaThucHien,
            TaoBaoCaoHoachToanTheoQuyNam
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                BarButtonItem hoachToan = new BarButtonItem(barManager, "Tạo báo cáo hoạch toán doanh thu", 1);
                hoachToan.Tag = PrintType.TaoBaoCaoHoachToanDoanhThu;
                hoachToan.ItemClick += new ItemClickEventHandler(this.mouse_Click);

                BarButtonItem chiDinh = new BarButtonItem(barManager, "Tạo báo cáo theo khoa chỉ định", 2);
                chiDinh.Tag = PrintType.TaoBaoCaoTheoKhoaChiDinh;
                chiDinh.ItemClick += new ItemClickEventHandler(this.mouse_Click);

                BarButtonItem thucHien = new BarButtonItem(barManager, "Tạo báo cáo theo khoa thực hiện", 3);
                thucHien.Tag = PrintType.TaoBaoCaoTheoKhoaThucHien;
                thucHien.ItemClick += new ItemClickEventHandler(this.mouse_Click);

                menu.AddItems(new BarItem[] { hoachToan, chiDinh, thucHien });
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
