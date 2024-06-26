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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Common;
using System.Runtime.InteropServices;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        internal PopupMenu menu;

        public enum MOUSE_RIGHT_TYPE
        {
            EDIT_DAY_NUM
        }


        private void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager1);
                // Add item and show
                menu.ItemLinks.Clear();
                if (CheckEditDayNum())
                {
                    BarButtonItem itemEditDayNum = new BarButtonItem(barManager1, "Sửa số ngày", 1);
                    itemEditDayNum.Tag = MOUSE_RIGHT_TYPE.EDIT_DAY_NUM;
                    itemEditDayNum.ItemClick += new ItemClickEventHandler(setProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemEditDayNum });
                }
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setProcessMenu(object sender, ItemClickEventArgs e)
        {
            try
            {
                var btn = e.Item as BarButtonItem;
                MOUSE_RIGHT_TYPE processType = (MOUSE_RIGHT_TYPE)btn.Tag;
                switch (processType)
                {
                    case MOUSE_RIGHT_TYPE.EDIT_DAY_NUM:
                        frmEditDayNum frm = new frmEditDayNum(ReloadDataEditDayNum);
                        frm.ShowDialog();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadDataEditDayNum(object data)
        {
            try
            {
                if (data != null)
                {
                    decimal dayNum = (decimal)data;
                    List<MediMateTypeADO> mediMatyTypeADOs = GetListCheck();// GetMediMatySelected();

                    foreach (var item in mediMatyTypeADOs)
                    {
                        if (item.DayNum == null)
                        {
                            item.DayNum = 1;
                            Inventec.Common.Logging.LogSystem.Debug("Thuoc khong có Use_time_to-----" + item.MEDI_MATE_TYPE_NAME);
                        }
                        if (item.IsMedicine)
                        {
                            TakeBeanMedicineProccess(item, Math.Ceiling((decimal)(((item.EXP_AMOUNT / item.DayNum) * dayNum) ?? 0)), item.ClientSessionKey);
                        }
                        else if (this.currentMediMate.IsMaterial)
                        {
                            TakeBeanMaterialProccess(item, Math.Ceiling((decimal)(((item.EXP_AMOUNT / item.DayNum) * dayNum) ?? 0)), item.ClientSessionKey);
                        }
                        item.DayNum = (long?)dayNum;
                    }

                    LoadDataToGridExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool CheckEditDayNum()
        {
            bool result = true;
            try
            {
                List<MediMateTypeADO> mediMatyTypeADOs = GetListCheck();// GetMediMatySelected();
                result = result && !(mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0);
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {

                    MediMateTypeADO mediMatyTypeADO = mediMatyTypeADOs.FirstOrDefault(o => !o.IsMedicine);
                    if (mediMatyTypeADO != null)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MediMateTypeADO> GetMediMatySelected()
        {
            List<MediMateTypeADO> result = new List<MediMateTypeADO>();
            try
            {
                //int[] selectRows = gridViewExpMestDetail.GetSelectedRows();
                //if (selectRows != null && selectRows.Count() > 0)
                //{
                //    for (int i = 0; i < selectRows.Count(); i++)
                //    {
                //        var mediMatyTypeADO = (MediMateTypeADO)gridViewExpMestDetail.GetRow(selectRows[i]);
                //        result.Add(mediMatyTypeADO);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
