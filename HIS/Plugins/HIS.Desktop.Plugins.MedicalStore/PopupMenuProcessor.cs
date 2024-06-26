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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.MedicalStore.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStore
{
    delegate void MedicalStoreRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        BarManager _BarManager = null;
        PopupMenu _Menu = null;
        MedicalStoreRightClick _MouseRightClick;
        TreatmentADO _TreatmentPoppupPrint;
        RefeshReference BtnRefreshPhimTat;
        List<TreatmentADO> listCheckTreatment;

        internal PopupMenuProcessor(TreatmentADO currenttreatment, BarManager barManager, MedicalStoreRightClick mouseRightClick, RefeshReference _BtnRefreshPhimTat, List<TreatmentADO> listCheckTreatment)
        {
            this._BarManager = barManager;
            this._MouseRightClick = mouseRightClick;
            this._TreatmentPoppupPrint = currenttreatment;
            this.listCheckTreatment = listCheckTreatment;
            this.BtnRefreshPhimTat = _BtnRefreshPhimTat;
        }

        internal enum ItemType
        {
            Del,
            Print,
            TreatmentBorrow,
            AddPatientProgram,
            SaveStore
        }

        internal void InitMenu()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);

                if (listCheckTreatment != null && listCheckTreatment.Count > 0)
                {
                    BarButtonItem itemSaveStore = new BarButtonItem(this._BarManager, "Lưu kho", 1);
                    itemSaveStore.Tag = ItemType.SaveStore;
                    itemSaveStore.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    _Menu.AddItems(new BarItem[] { itemSaveStore });
                }

                if (_TreatmentPoppupPrint.GIVE_DATE == null && _TreatmentPoppupPrint.DATA_STORE_ID > 0)
                {
                    BarButtonItem itemDel = new BarButtonItem(this._BarManager, "Xóa khỏi kho", 1);
                    itemDel.Tag = ItemType.Del;
                    itemDel.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    _Menu.AddItems(new BarItem[] { itemDel });
                }

                if (_TreatmentPoppupPrint.DATA_STORE_ID > 0)
                {
                    BarButtonItem itemPrint = new BarButtonItem(this._BarManager, "In", 2);
                    itemPrint.Tag = ItemType.Print;
                    itemPrint.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    _Menu.AddItems(new BarItem[] { itemPrint });

                    BarButtonItem itemTreatmentBorrow = new BarButtonItem(this._BarManager, "Mượn HSBA", 3);
                    itemTreatmentBorrow.Tag = ItemType.TreatmentBorrow;
                    itemTreatmentBorrow.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    _Menu.AddItems(new BarItem[] { itemTreatmentBorrow });
                }

                BarButtonItem itemAddPatientProgram = new BarButtonItem(this._BarManager, "Gán bệnh nhân chương trình", 4);
                itemAddPatientProgram.Tag = ItemType.AddPatientProgram;
                itemAddPatientProgram.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                _Menu.AddItems(new BarItem[] { itemAddPatientProgram });

                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
