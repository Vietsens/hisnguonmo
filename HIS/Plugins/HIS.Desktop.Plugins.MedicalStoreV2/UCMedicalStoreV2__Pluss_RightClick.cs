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
using HIS.Desktop.Plugins.MedicalStoreV2.ADO;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.Skins;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.Plugins.MedicalStoreV2.ChooseStore;
using HIS.Desktop.Print;
using DevExpress.XtraBars;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.MedicalStoreV2
{
    public partial class UCMedicalStoreV2 : UserControlBase
    {
        //public void gridViewTreatment_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        //{
        //    try
        //    {
        //        GridHitInfo hi = e.HitInfo;
        //        if (hi.InRowCell)
        //        {
        //            int rowHandle = gridViewMediRecord.GetVisibleRowHandle(hi.RowHandle);

        //            var currentRowTreatment = (TreatmentADO)gridViewMediRecord.GetRow(rowHandle);

        //            gridViewMediRecord.OptionsSelection.EnableAppearanceFocusedCell = true;
        //            gridViewMediRecord.OptionsSelection.EnableAppearanceFocusedRow = true;
        //            if (barManager1 == null)
        //            {
        //                barManager1 = new BarManager();
        //                barManager1.Form = this;
        //            }

        //            var listDataSourceTreatment = (List<TreatmentADO>)gridControlMediRecord.DataSource;

        //            PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(currentRowTreatment, barManager1, Treatment_MouseRightClick, (RefeshReference)BtnRefreshs);
        //            popupMenuProcessor.InitMenu();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        public void BtnRefreshs()
        {
            try
            {
                LoadDataToGridControlTreatment();
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
                        case PopupMenuProcessor.ItemType.CheckingTreatmentEmr:
                            CheckingTreatmentEmr();
                            break;
                        //case PopupMenuProcessor.ItemType.Del:
                        //    repositoryItemButtonEdit_Delete_Enable_ButtonClick(null, null);
                        //    break;
                        //case PopupMenuProcessor.ItemType.SaveStore:
                        //    btnSave_Click(null, null);
                        //    break;
                        //case PopupMenuProcessor.ItemType.Print:
                        //    repositoryItemButtonEdit_Print_Enable_ButtonClick(null, null);
                        //    break;
                        //case PopupMenuProcessor.ItemType.TreatmentBorrow:
                        //    Btn_TreatmentBorrow_Enable_ButtonClick(null, null);
                        //    break;
                        //case PopupMenuProcessor.ItemType.AddPatientProgram:
                        //    Btn_AddPatientProgram_ButtonClick(null, null);
                        //    break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediRecord_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridHitInfo hi = e.HitInfo;
            if (hi.InRowCell)
            {
                int rowHandle = gridViewMediRecord.GetVisibleRowHandle(hi.RowHandle);

                var currentRowTreatment = (MediRecordADO)gridViewMediRecord.GetRow(rowHandle);

                gridViewMediRecord.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridViewMediRecord.OptionsSelection.EnableAppearanceFocusedRow = true;
                if (barManager1 == null)
                {
                    barManager1 = new BarManager();
                    barManager1.Form = this;
                }

                PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(currentRowTreatment, barManager1, MediRecord_MouseRightClick, (RefeshReference)BtnRefreshs);
                popupMenuProcessor.InitMenu();
            }
        }

        void MediRecord_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.TrackingInMediRecord:
                            btnTrackingInMediRecord();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTrackingInMediRecord()
        {
            try
            {
                    V_HIS_MEDI_RECORD MediRecord = new V_HIS_MEDI_RECORD();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_MEDI_RECORD>(MediRecord, gridViewMediRecord.GetFocusedRow());
                   Inventec.Common.Logging.LogSystem.Info("MediRecord: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => MediRecord), MediRecord));
                    List<object> listArgs = new List<object>();
                    listArgs.Add(MediRecord);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TrackingInMediRecord", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckingTreatmentEmr()
        {
            try
            {
                this.currentTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(currentTreatment, gridViewTreatment.GetFocusedRow());
                if (currentTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentTreatment.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTreatmentRecordChecking", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
