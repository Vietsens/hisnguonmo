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

namespace HIS.Desktop.Plugins.SampleCollectionRoom
{
    delegate void SampleMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        PopupMenu _Menu = null;
        SampleMouseRightClick _MouseRightClick;
        internal PopupMenuProcessor(BarManager barManager, SampleMouseRightClick mouseRightClick)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
        }

        internal enum ItemType
        {
            LichSuXetNghiem,
            LayMau
        }
        internal void InitMenuChecked()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                BarButtonItem btnLichSuXN = new BarButtonItem(this._BarManager, "Lấy mẫu", 1);
                btnLichSuXN.Tag = ItemType.LayMau;
                btnLichSuXN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                this._Menu.AddItems(new BarItem[] { btnLichSuXN });
                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void InitMenu()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                ////lấy mẫu nhanh
                //BarButtonItem bbtnLayMauNhanh = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SAMPLE_COLLECTION_LAY_MAU_NHANH", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                //bbtnLayMauNhanh.Tag = ItemType.LayMauNhanh;
                //bbtnLayMauNhanh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                //this._Menu.AddItems(new BarItem[] { bbtnLayMauNhanh });

                ////gọi bệnh nhân vào lấy kết quả nhanh
                //BarButtonItem bbtnGoiBNLayKetQuaNhanh = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_SAMPLE_COLLECTION_GOI_BN_VAO_LAY_KET_QUA_NHANH", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                //bbtnGoiBNLayKetQuaNhanh.Tag = ItemType.GoiBNVaoLayKetQuaNhanh;
                //bbtnGoiBNLayKetQuaNhanh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                //this._Menu.AddItems(new BarItem[] { bbtnGoiBNLayKetQuaNhanh });

                BarButtonItem btnLichSuXN = new BarButtonItem(this._BarManager, "Lịch sử xét nghiệm của bệnh nhân", 1);
                btnLichSuXN.Tag = ItemType.LichSuXetNghiem;
                btnLichSuXN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                this._Menu.AddItems(new BarItem[] { btnLichSuXN });
                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
