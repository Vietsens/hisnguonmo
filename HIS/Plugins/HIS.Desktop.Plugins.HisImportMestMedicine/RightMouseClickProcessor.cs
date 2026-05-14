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
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    delegate void MouseRight_Click(object sender, ItemClickEventArgs e);

    class RightMouseClickProcessor
    {
        MOS.EFMODEL.DataModels.V_HIS_IMP_MEST ImpMestRightClick;
        MouseRight_Click mouseRightClick;
        BarManager barManager;
        PopupMenu menu;
        long roomId;
        string loginName;
        // 42727 - cờ điều khiển hiển thị item menu hoàn ứng theo dòng đang focus
        bool allowCreateRepay;
        bool allowPrintRepay;

        internal enum ModuleType
        {
            ManuExpMestCreate,
            ManuImpMestEdit,
            PrintMps000505,
            // 42727 - menu chuột phải
            TaoGiaoDichChiTien,
            InPhieuHoanUng
        }
        internal ModuleType moduleType { get; set; }

        internal RightMouseClickProcessor(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST currentImpMest, MouseRight_Click MouseRightClick, BarManager barManager, long _roomId, string loginName)
        {
            this.ImpMestRightClick = currentImpMest;
            this.mouseRightClick = MouseRightClick;
            this.barManager = barManager;
            this.roomId = _roomId;
            this.loginName = loginName;
        }

        // 42727 - constructor mở rộng kèm cờ Repay
        internal RightMouseClickProcessor(
            MOS.EFMODEL.DataModels.V_HIS_IMP_MEST currentImpMest,
            MouseRight_Click MouseRightClick,
            BarManager barManager,
            long _roomId,
            string loginName,
            bool allowCreateRepay,
            bool allowPrintRepay)
            : this(currentImpMest, MouseRightClick, barManager, _roomId, loginName)
        {
            this.allowCreateRepay = allowCreateRepay;
            this.allowPrintRepay = allowPrintRepay;
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem itemPrint = new BarButtonItem(barManager, "In gộp biên bản kiểm nhập từ nhà cung cấp", 1);
                itemPrint.Tag = ModuleType.PrintMps000505;
                itemPrint.ItemClick += new ItemClickEventHandler(mouseRightClick);
                menu.AddItem(itemPrint);

                // 42727 - Menu chuột phải cho phiếu nhập lại xuất bán
                if (this.allowCreateRepay)
                {
                    BarButtonItem itemTaoGD = new BarButtonItem(barManager, "Tạo giao dịch chi tiền", 2);
                    itemTaoGD.Tag = ModuleType.TaoGiaoDichChiTien;
                    itemTaoGD.ItemClick += new ItemClickEventHandler(mouseRightClick);
                    menu.AddItem(itemTaoGD);
                }

                if (this.allowPrintRepay)
                {
                    BarButtonItem itemInPhieu = new BarButtonItem(barManager, "In phiếu hoàn ứng", 3);
                    itemInPhieu.Tag = ModuleType.InPhieuHoanUng;
                    itemInPhieu.ItemClick += new ItemClickEventHandler(mouseRightClick);
                    menu.AddItem(itemInPhieu);
                }

                menu.ShowPopup(Cursor.Position);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
