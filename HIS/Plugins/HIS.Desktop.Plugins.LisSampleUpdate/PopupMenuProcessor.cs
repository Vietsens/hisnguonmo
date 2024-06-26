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
using HIS.Desktop.Plugins.LisSampleUpdate.Resources;
using Inventec.Common.LocalStorage.SdaConfig;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.LisSampleUpdate
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_LIS_SAMPLE _Sample = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        MouseRightClick _MouseRightClick;
        internal enum ItemType
        {
            CapNhatTinhTrangMau,
            LichSuXetNghiem
        }

        internal PopupMenuProcessor(V_LIS_SAMPLE sample, BarManager barmanager, MouseRightClick mouseRightClick)
        {
            this._Sample = sample;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
        }

        internal void InitMenu()
        {
            try
            {
                if (this._Sample == null || this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                if (_Sample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                    && _Sample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                {
                    BarButtonItem bbtnPhieuCongKhaiBN = new BarButtonItem(this._BarManager, "Cập nhật tình trạng mẫu", 0);
                    bbtnPhieuCongKhaiBN.Tag = ItemType.CapNhatTinhTrangMau;
                    bbtnPhieuCongKhaiBN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuCongKhaiBN });

                    BarButtonItem btnLichSuXN = new BarButtonItem(this._BarManager, "Lịch sử xét nghiệm của bệnh nhân", 1);
                    btnLichSuXN.Tag = ItemType.LichSuXetNghiem;
                    btnLichSuXN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { btnLichSuXN });

                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
