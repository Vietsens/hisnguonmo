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
using DevExpress.XtraTreeList.Nodes;
using MOS.EFMODEL.DataModels;
using Inventec.UC.Paging;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.MedicalStore.ADO;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.Skins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.Plugins.MedicalStore.ChooseStore;
using HIS.Desktop.Print;
using DevExpress.XtraBars;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.MedicalStore
{
    public partial class UCMedicalStore : UserControl
    {
        private void gridViewtreatment_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewTreatment.GetVisibleRowHandle(hi.RowHandle);

                    var currentRowTreatment = (TreatmentADO)gridViewTreatment.GetRow(rowHandle);

                    gridViewTreatment.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewTreatment.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }

                    var listDataSourceTreatment = (List<TreatmentADO>)gridControlTreatment.DataSource;
                    List<TreatmentADO> listCheckTreatment = listDataSourceTreatment.Where(o => o.CheckTreatment && (o.DATA_STORE_ID ?? 0) == 0).ToList();

                    PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(currentRowTreatment, barManager1, Treatment_MouseRightClick, (RefeshReference)BtnRefreshs, listCheckTreatment);
                    popupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRefreshs()
        {
            try
            {
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Treatment_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.Del:
                            repositoryItemButtonEdit_Delete_Enable_ButtonClick(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.SaveStore:
                            btnSave_Click(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.Print:
                            repositoryItemButtonEdit_Print_Enable_ButtonClick(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.TreatmentBorrow:
                            Btn_TreatmentBorrow_Enable_ButtonClick(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.AddPatientProgram:
                            Btn_AddPatientProgram_ButtonClick(null, null);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
