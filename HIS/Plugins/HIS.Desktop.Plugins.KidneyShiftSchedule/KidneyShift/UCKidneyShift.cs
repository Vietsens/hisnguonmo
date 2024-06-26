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
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KidneyShiftSchedule.Config;
using HIS.Desktop.Plugins.KidneyShiftSchedule.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Core;
using System.Threading;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.PopupLoader;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.KidneyShiftSchedule.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift
{
    public partial class UCKidneyShift : UserControlBase
    {
        #region Variables
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_ROOM requestRoom;
        HIS_DEPARTMENT currentDepartment = null;

        long treatmentId = 0;
        HIS.Desktop.ADO.KidneyShiftScheduleADO.DelegateProcessDataResult processDataResult;
        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        long[] serviceTypeIdAllows;
        List<long> patientTypeIdAls;
        Dictionary<long, V_HIS_SERVICE> dicServices;
        List<MOS.EFMODEL.DataModels.HIS_MACHINE> currentMachines;
        List<TreatmentBedRoomADO> _TreatmentBedRoomADOs { get; set; }
        List<ServiceReqADO> _ServiceReqADOs { get; set; }
        TreatmentBedRoomADO currentTreatmentBedRoomADO;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;
        int positionHandleControl = -1;
        int actionType = 0;
        const string START_TIME = "000000";
        const string END_TIME = "235959";
        HisServiceReqListResultSDO serviceReqListResultSDO;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        #endregion

        #region Construct - OnLoad
        public UCKidneyShift(KidneyShiftScheduleADO dataADO, Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.currentModule = currentModule;
                this.actionType = GlobalVariables.ActionAdd;
                if (dataADO != null)
                {
                    this.processDataResult = dataADO.DgProcessDataResult;
                    this.treatmentId = dataADO.TreatmentId;
                }
                this.gridControlServiceReqKidneyshift.ToolTipController = this.tooltipServiceRequest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCKidneyShift_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                this.SetDefaultData();
                this.requestRoom = GetRequestRoom(this.currentModule != null ? this.currentModule.RoomId : 0);
                this.InitDataWithCurrentWorking();
                this.FillDataToControlsForm();
                this.FillDataToGridTreatmentBedRoom();
                this.FillDataToGridServiceReqKidneyShift();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private method
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.KidneyShiftSchedule.Resources.Lang", typeof(HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift.UCKidneyShift).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboUser.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboUser.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceForAdd.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboServiceForAdd.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAddIntoSchedule.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.btnAddIntoSchedule.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExpMestTemplateForAdd.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboExpMestTemplateForAdd.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMarchineForAdd.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboMarchineForAdd.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCaForAdd.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboCaForAdd.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchForPatientInBedroom.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.btnSearchForPatientInBedroom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSearchAllInDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.chkSearchAllInDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBedroomForPatientInBedroom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboBedroomForPatientInBedroom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearchForSearchServiceReqKidneyshift.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.btnSearchForSearchServiceReqKidneyshift.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintForSearchServiceReqKidneyshift.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.btnPrintForSearchServiceReqKidneyshift.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMarchineForSearchServiceReqKidneyshift.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboMarchineForSearchServiceReqKidneyshift.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCaForSearchServiceReqKidneyshift.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboCaForSearchServiceReqKidneyshift.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPreviousForSearchServiceReqKidneyshift.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.btnPreviousForSearchServiceReqKidneyshift.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNextForSearchServiceReqKidneyshift.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.btnNextForSearchServiceReqKidneyshift.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDayOfWeekForSearchServiceReqKidneyshift.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboDayOfWeekForSearchServiceReqKidneyshift.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCKidneyShift.cboExecuteRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDel.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumnDel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.InstructionTimeDisplay.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCKidneyShift.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFortxtLoginName.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.lciFortxtLoginName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("UCKidneyShift.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void InitTimer()
        //{
        //    this.timerSyncToRAM = new System.Windows.Forms.Timer();
        //    this.timerSyncToRAM.Interval = 2000;
        //    this.timerSyncToRAM.Enabled = true;
        //    this.timerSyncToRAM.Tick += this.timerSyncToRAM_Tick;
        //    this.timerSyncToRAM.Start();
        //}

        //private void timerSyncToRAM_Tick(object sender, EventArgs e)
        //{
        //    ProcessSyncAllData();
        //}

        //private void TheadProcessSyncAllData()
        //{
        //    try
        //    {
        //        Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessSyncAllData));
        //        thread.Priority = ThreadPriority.Highest;
        //        try
        //        {
        //            thread.Start();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogSystem.Error(ex);
        //            thread.Abort();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Warn(ex);
        //    }
        //}

        //private void ProcessSyncAllData()
        //{
        //    try
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug("ProcessSyncAllData. 1");
        //        if (GlobalDatas.Treatments != null)
        //        {
        //            Inventec.Common.Logging.LogSystem.Debug("ProcessSyncAllData. 2");
        //            this.FillDataToGridTreatmentBedRoom();
        //            timerSyncToRAM.Stop();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Warn(ex);
        //    }
        //}

        private void SetDefaultData()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.KidneyShiftSchedule.Resources.Lang", typeof(HIS.Desktop.Plugins.KidneyShiftSchedule.KidneyShift.UCKidneyShift).Assembly);
                HisConfigCFG.LoadConfig();
                this.actionType = GlobalVariables.ActionAdd;
                ButtonEdit_DelDisable.Enabled = false;
                ButtonEdit_DelDisable.ReadOnly = true;
                repositoryItemTextEditDisable.Enabled = false;
                repositoryItemTextEditDisable.ReadOnly = true;

                dateDateForAdd.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_ROOM GetRequestRoom(long requestRoomId)
        {
            V_HIS_ROOM result = new V_HIS_ROOM();
            try
            {
                if (requestRoomId > 0)
                {
                    result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
                    this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == result.DEPARTMENT_ID);
                }
            }
            catch (Exception ex)
            {
                result = new V_HIS_ROOM();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #region Button clicks
        private void btnSearchForSearchServiceReqKidneyshift_Click(object sender, EventArgs e)
        {
            this.FillDataToGridServiceReqKidneyShift();
        }

        private void btnSearchForPatientInBedroom_Click(object sender, EventArgs e)
        {
            this.FillDataToGridTreatmentBedRoom();
        }

        private void btnAddIntoSchedule_Click(object sender, EventArgs e)
        {
            ProcessAddClick();
        }

        private void btnPrintForSearchServiceReqKidneyshift_Click(object sender, EventArgs e)
        {
            Print(printTypeCode);
        }

        private void btnPreviousForSearchServiceReqKidneyshift_Click(object sender, EventArgs e)
        {
            try
            {
                dateDateForSearchServiceReqKidneyshift.EditValue = null;
                dateWeekFrom.EditValue = dateWeekFrom.DateTime.AddDays(-7);
                dateWeekTo.EditValue = dateWeekTo.DateTime.AddDays(-7);
                this.FillDataToGridServiceReqKidneyShift();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNextForSearchServiceReqKidneyshift_Click(object sender, EventArgs e)
        {
            try
            {
                dateDateForSearchServiceReqKidneyshift.EditValue = null;
                dateWeekFrom.EditValue = dateWeekFrom.DateTime.AddDays(7);
                dateWeekTo.EditValue = dateWeekTo.DateTime.AddDays(7);
                this.FillDataToGridServiceReqKidneyShift();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void tooltipServiceRequest_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlServiceReqKidneyshift)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlServiceReqKidneyshift.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlTreatmentBedRoom_Click(object sender, EventArgs e)
        {
            try
            {
                this.RowTreatmentBedRoomRowClick();
                if (dateDateForAdd.EditValue == null)
                {
                    dateDateForAdd.Focus();
                }
                else if (cboCaForAdd.EditValue == null)
                    cboCaForAdd.Focus();
                else
                {
                    this.cboMarchineForAdd.Focus();
                    this.cboMarchineForAdd.SelectAll();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentBedRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        TreatmentBedRoomADO oneServiceSDO = (TreatmentBedRoomADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "STT")
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (((ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.CurrentPage) - 1) * (ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.PageSize));
                            }
                            if (e.Column.FieldName == "TDL_PATIENT_DOB_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.TDL_PATIENT_DOB);
                            }
                            if (e.Column.FieldName == "IN_TIME_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.IN_TIME);
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqKidneyshift_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        ServiceReqADO oneServiceSDO = (ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "STT")
                            {
                                e.Value = (e.ListSourceRowIndex + 1);
                            }
                            if (e.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                //Chua xu ly: mau trang
                                //dang xu ly: mau vang
                                //Da ket thuc: mau den

                                long statusId = oneServiceSDO.SERVICE_REQ_STT_ID;
                                if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    e.Value = imageListIcon.Images[0];
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    e.Value = imageListIcon.Images[1];
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    e.Value = imageListIcon.Images[4];
                                }
                                else
                                {
                                    e.Value = imageListIcon.Images[0];
                                }
                            }
                            if (e.Column.FieldName == "InstructionTimeDisplay")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.INTRUCTION_DATE);
                            }
                            if (e.Column.FieldName == "TDL_PATIENT_DOB_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.TDL_PATIENT_DOB);
                            }
                            if (e.Column.FieldName == "IN_TIME_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.IN_TIME);
                            }
                            if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.CREATE_TIME ?? 0);
                            }
                            if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.MODIFY_TIME ?? 0);
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqKidneyshift_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "BtnDelete")
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        short sERVICE_REQ_STT_ID = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewServiceReqKidneyshift.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_ID") ?? "0").ToString());
                        //string cREATOR = (gridViewServiceReqKidneyshift.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                        if (sERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                            //&& cREATOR == loginName
                            )
                            e.RepositoryItem = this.ButtonEdit_DelEnable;
                        else
                            e.RepositoryItem = this.repositoryItemTextEditDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqKidneyshift_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ServiceReqADO data = view.GetFocusedRow() as ServiceReqADO;
                if (data == null) return;

                if (view.FocusedColumn.FieldName == "BtnDelete")
                {
                    //string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (data.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                        //|| data.CREATOR != loginName
                        )
                        e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_DelEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Inventec.Common.Logging.LogSystem.Debug("ButtonEdit_DelEnable_ButtonClick.0");
            try
            {
                var serviceReqADO = (ServiceReqADO)this.gridViewServiceReqKidneyshift.GetFocusedRow();
                if (MessageBox.Show(ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.CanhBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    // string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (serviceReqADO.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL //&& serviceReqADO.CREATOR == loginName
                        )
                    {
                        WaitingManager.Show();
                        MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                        sdo.Id = serviceReqADO.ID;
                        sdo.RequestRoomId = this.currentModule.RoomId;
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERVICE_REQ_DELETE, ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        WaitingManager.Hide();
                        if (success)
                        {
                            //Thêm log thao tác khi thực hiện xóa lịch chạy thận

                            this.FillDataToGridServiceReqKidneyShift();
                        }

                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    else
                    {
                        MessageManager.Show(ResourceMessage.HeThongThongBaoBanKhongCoQuyenHuyDuLieuNay);
                        Inventec.Common.Logging.LogSystem.Warn("Yeu cau chay than khong o trang thai chua xu ly hoac tai khoan thao tac khong phai tai khoan tao du lieu, khong the huy bo yeu cau");
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeywordForPatientInBedroom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.FillDataToGridTreatmentBedRoom();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateDateForSearchServiceReqKidneyshift_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dateDateForSearchServiceReqKidneyshift.Update();
                    this.CalculateDateWeekFromToBySelectedDate();
                    if (dateDateForSearchServiceReqKidneyshift.EditValue != null)
                    {
                        this.ChangeForDateOfWeekSearchServiceReqKidneyshift();
                    }
                    else
                    {
                        cboDayOfWeekForSearchServiceReqKidneyshift.EditValue = null;
                        cboDayOfWeekForSearchServiceReqKidneyshift.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateDateForSearchServiceReqKidneyshift_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dateDateForSearchServiceReqKidneyshift.Update();
                    this.CalculateDateWeekFromToBySelectedDate();
                    this.ChangeForDateOfWeekSearchServiceReqKidneyshift();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekFrom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dateWeekFrom.Update();
                    this.ChangeForDateSearchServiceReqKidneyshift();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekFrom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dateWeekFrom.Update();
                    this.ChangeForDateSearchServiceReqKidneyshift();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dateWeekTo.Update();
                    this.ChangeForDateSearchServiceReqKidneyshift();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekTo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dateWeekTo.Update();
                    this.ChangeForDateSearchServiceReqKidneyshift();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCaForSearchServiceReqKidneyshift_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboCaForSearchServiceReqKidneyshift.Text))
                {
                    cboCaForSearchServiceReqKidneyshift.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeForDateSearchServiceReqKidneyshift()
        {
            try
            {
                if (cboDayOfWeekForSearchServiceReqKidneyshift.EditValue != null)
                {
                    int dayInWeek = (int)cboDayOfWeekForSearchServiceReqKidneyshift.EditValue;
                    DateTime dtFrom = dateWeekFrom.DateTime;

                    //int delta = DayOfWeek.Monday - dtFrom.DayOfWeek;
                    int addDay = ((DayOfWeek)dayInWeek == DayOfWeek.Sunday ? 6 : (DayOfWeek)dayInWeek - DayOfWeek.Monday);
                    DateTime dtGen = dtFrom.AddDays(addDay);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => addDay), addDay) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dtGen.Date), dtGen.Date) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dateDateForSearchServiceReqKidneyshift.DateTime.Date), dateDateForSearchServiceReqKidneyshift.DateTime.Date));

                    if (dateDateForSearchServiceReqKidneyshift.DateTime.Date != dtGen.Date)
                        dateDateForSearchServiceReqKidneyshift.EditValue = dtGen;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeForDateOfWeekSearchServiceReqKidneyshift()
        {
            try
            {

                DayOfWeek dayOfWeek = dateDateForSearchServiceReqKidneyshift.DateTime.DayOfWeek;
                cboDayOfWeekForSearchServiceReqKidneyshift.EditValue = (int)dayOfWeek;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateWeekFromToBySelectedDate()
        {
            try
            {
                if (dateDateForSearchServiceReqKidneyshift.EditValue != null)
                {
                    DateTime? startWeek = StartWeekSystemDateTime(dateDateForSearchServiceReqKidneyshift.DateTime);
                    if (startWeek != null && startWeek.Value.Date != dateWeekFrom.DateTime.Date)
                    {
                        dateWeekFrom.EditValue = startWeek;
                        dateWeekTo.EditValue = dateWeekFrom.DateTime.AddDays(6);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDayOfWeekForSearchServiceReqKidneyshift_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDayOfWeekForSearchServiceReqKidneyshift.EditValue == null)
                {
                    dateDateForSearchServiceReqKidneyshift.EditValue = null;
                    cboDayOfWeekForSearchServiceReqKidneyshift.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboDayOfWeekForSearchServiceReqKidneyshift.Properties.Buttons[1].Visible = true;
                    ChangeForDateSearchServiceReqKidneyshift();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDayOfWeekForSearchServiceReqKidneyshift_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDayOfWeekForSearchServiceReqKidneyshift.EditValue = null;
                    dateDateForSearchServiceReqKidneyshift.EditValue = null;
                    cboDayOfWeekForSearchServiceReqKidneyshift.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboExecuteRoom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboExecuteRoom.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtExecuteRoom.Text = data.EXECUTE_ROOM_CODE;
                            this.InitComboMachine(cboMarchineForAdd, false);
                            this.InitComboMachine(cboMarchineForSearchServiceReqKidneyshift, false);
                            this.cboMarchineForAdd.EditValue = null;
                            this.cboMarchineForSearchServiceReqKidneyshift.EditValue = null;
                        }
                    }
                    this.dateWeekFrom.Focus();
                    this.dateWeekFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboExecuteRoom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboExecuteRoom.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtExecuteRoom.Text = data.EXECUTE_ROOM_CODE;
                            this.InitComboMachine(cboMarchineForAdd, false);
                            this.InitComboMachine(cboMarchineForSearchServiceReqKidneyshift, false);
                            this.cboMarchineForAdd.EditValue = null;
                            this.cboMarchineForSearchServiceReqKidneyshift.EditValue = null;

                            this.dateWeekFrom.Focus();
                            this.dateWeekFrom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExecuteRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCombo = true;
                    string searchCode = txtExecuteRoom.Text;
                    if (!String.IsNullOrEmpty(searchCode))
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.EXECUTE_ROOM_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var result = data != null ? (data.Count > 1 ? data.Where(o => o.EXECUTE_ROOM_CODE.ToLower() == searchCode.ToLower()).ToList() : data) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCombo = false;
                            this.cboExecuteRoom.Properties.Buttons[1].Visible = true;
                            this.cboExecuteRoom.EditValue = result.First().ID;
                            this.txtExecuteRoom.Text = result.First().EXECUTE_ROOM_CODE;
                            this.InitComboMachine(cboMarchineForAdd, false);
                            this.InitComboMachine(cboMarchineForSearchServiceReqKidneyshift, false);
                            this.cboMarchineForAdd.EditValue = null;
                            this.cboMarchineForSearchServiceReqKidneyshift.EditValue = null;
                            this.dateWeekFrom.Focus();
                            this.dateWeekFrom.ShowPopup();
                        }
                    }
                    if (showCombo)
                    {
                        cboExecuteRoom.Properties.Buttons[1].Visible = false;
                        cboExecuteRoom.EditValue = null;
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMarchineForSearchServiceReqKidneyshift_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboMarchineForSearchServiceReqKidneyshift.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MACHINE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMarchineForSearchServiceReqKidneyshift.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            cboMarchineForSearchServiceReqKidneyshift.Properties.Buttons[1].Visible = true;
                        }
                    }
                    txtKeywordForSearchServiceReqKidneyshift.Focus();
                    txtKeywordForSearchServiceReqKidneyshift.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMarchineForSearchServiceReqKidneyshift_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboMarchineForSearchServiceReqKidneyshift.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MACHINE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMarchineForSearchServiceReqKidneyshift.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            cboMarchineForSearchServiceReqKidneyshift.Properties.Buttons[1].Visible = true;

                            txtKeywordForSearchServiceReqKidneyshift.Focus();
                            txtKeywordForSearchServiceReqKidneyshift.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMarchineForSearchServiceReqKidneyshift_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboMarchineForSearchServiceReqKidneyshift.Properties.Buttons[1].Visible = false;
                    cboMarchineForSearchServiceReqKidneyshift.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dateWeekTo.Focus();
                    this.dateWeekTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateWeekTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtKeywordForSearchServiceReqKidneyshift.Focus();
                    this.txtKeywordForSearchServiceReqKidneyshift.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeywordForSearchServiceReqKidneyshift_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.FillDataToGridServiceReqKidneyShift();
            }
        }

        private void cboMarchineForAdd_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboMarchineForAdd.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MACHINE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMarchineForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            //cboMarchineForAdd.Properties.Buttons[1].Visible = true;
                        }
                    }
                    cboExpMestTemplateForAdd.Focus();
                    cboExpMestTemplateForAdd.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMarchineForAdd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboMarchineForAdd.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MACHINE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MACHINE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMarchineForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            //cboMarchineForAdd.Properties.Buttons[1].Visible = true;

                            cboExpMestTemplateForAdd.Focus();
                            cboExpMestTemplateForAdd.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedroomForPatientInBedroom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboBedroomForPatientInBedroom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().Where(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBedroomForPatientInBedroom.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtBedroomForPatientInBedroom.Text = data.BED_ROOM_CODE;
                            cboBedroomForPatientInBedroom.Properties.Buttons[1].Visible = true;
                        }
                    }
                    chkSearchAllInDepartment.Focus();
                    chkSearchAllInDepartment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedroomForPatientInBedroom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboBedroomForPatientInBedroom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().Where(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBedroomForPatientInBedroom.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtBedroomForPatientInBedroom.Text = data.BED_ROOM_CODE;
                            cboBedroomForPatientInBedroom.Properties.Buttons[1].Visible = true;

                            chkSearchAllInDepartment.Focus();
                            chkSearchAllInDepartment.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedroomForPatientInBedroom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboBedroomForPatientInBedroom.Properties.Buttons[1].Visible = false;
                    cboBedroomForPatientInBedroom.EditValue = null;
                    txtBedroomForPatientInBedroom.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBedroomForPatientInBedroom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCombo = true;
                    string searchCode = txtBedroomForPatientInBedroom.Text;
                    if (!String.IsNullOrEmpty(searchCode))
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().Where(o => o.BED_ROOM_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var result = data != null ? (data.Count > 1 ? data.Where(o => o.BED_ROOM_CODE.ToLower() == searchCode.ToLower()).ToList() : data) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCombo = false;
                            cboBedroomForPatientInBedroom.Properties.Buttons[1].Visible = true;
                            cboBedroomForPatientInBedroom.EditValue = result.First().ID;
                            txtBedroomForPatientInBedroom.Text = result.First().BED_ROOM_CODE;

                            chkSearchAllInDepartment.Focus();
                            chkSearchAllInDepartment.SelectAll();
                            e.Handled = true;
                        }
                    }
                    if (showCombo)
                    {
                        cboBedroomForPatientInBedroom.Properties.Buttons[1].Visible = false;
                        cboBedroomForPatientInBedroom.EditValue = null;
                        cboBedroomForPatientInBedroom.Focus();
                        cboBedroomForPatientInBedroom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceForAdd_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboServiceForAdd.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtServiceForAdd.Text = data.SERVICE_CODE;
                            this.ProcessServiceChange(data);
                        }
                    }
                    cboPatientType.Focus();
                    cboPatientType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceForAdd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboServiceForAdd.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            this.txtServiceForAdd.Text = data.SERVICE_CODE;
                            this.ProcessServiceChange(data);

                            cboPatientType.Focus();
                            cboPatientType.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceForAdd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCombo = true;
                    string searchCode = txtServiceForAdd.Text;
                    if (!String.IsNullOrEmpty(searchCode))
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().Where(o => o.SERVICE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var result = data != null ? (data.Count > 1 ? data.Where(o => o.SERVICE_CODE.ToLower() == searchCode.ToLower()).ToList() : data) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCombo = false;
                            cboServiceForAdd.EditValue = result.First().ID;
                            txtServiceForAdd.Text = result.First().SERVICE_CODE;
                            this.ProcessServiceChange(result.First());

                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                            e.Handled = true;
                        }
                    }
                    if (showCombo)
                    {
                        cboServiceForAdd.EditValue = null;
                        cboServiceForAdd.Focus();
                        cboServiceForAdd.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateDateForAdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboCaForAdd.Focus();
                cboCaForAdd.ShowPopup();
            }
        }

        private void cboCaForAdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboMarchineForAdd.Focus();
                cboMarchineForAdd.ShowPopup();
            }
        }

        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtLoginName.Focus();
                txtLoginName.SelectAll();
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                if (this.cboPatientType.EditValue != null)
                {

                }
                txtLoginName.Focus();
                txtLoginName.SelectAll();
            }
        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.cboPatientType.EditValue != null)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                }
            }
        }

        private void cboExpMestTemplateForAdd_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboExpMestTemplateForAdd.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboExpMestTemplateForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            //cboExpMestTemplateForAdd.Properties.Buttons[1].Visible = true;
                        }
                    }
                    txtNoteForAdd.Focus();
                    txtNoteForAdd.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestTemplateForAdd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboExpMestTemplateForAdd.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboExpMestTemplateForAdd.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            //cboExpMestTemplateForAdd.Properties.Buttons[1].Visible = true;

                            txtNoteForAdd.Focus();
                            txtNoteForAdd.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestTemplateForAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboExpMestTemplateForAdd.Properties.Buttons[1].Visible = false;
                    cboExpMestTemplateForAdd.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoteForAdd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAddIntoSchedule.Focus();
                e.Handled = true;
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboUser.EditValue = null;
                        this.FocusShowpopup(cboUser, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser();
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                            this.FocusShowpopup(cboUser, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }

                    this.FocusWhileSelectedUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser();
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }
                }
                else
                {
                    this.cboUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusWhileSelectedUser()
        {
            try
            {
                this.txtNoteForAdd.Focus();
                this.txtNoteForAdd.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public Method
        public void AddKidneyShiftShortcut()
        {
            if (btnAddIntoSchedule.Enabled)
                ProcessAddClick();
        }

        public void SearchServiceReqKidneyShiftShortcut()
        {
            if (btnSearchForSearchServiceReqKidneyshift.Enabled)
                this.FillDataToGridServiceReqKidneyShift();
        }

        public void PrintServiceReqKidneyShiftShortcut()
        {
            if (btnPrintForSearchServiceReqKidneyshift.Enabled)
                this.btnPrintForSearchServiceReqKidneyshift_Click(null, null);
        }

        public void SearchPatientInBedRoomShortcut()
        {
            if (btnSearchForPatientInBedroom.Enabled)
                this.btnSearchForPatientInBedroom_Click(null, null);
        }
        #endregion

        long? StartWeek(System.DateTime now)
        {
            long? result = null;
            try
            {
                int delta = DayOfWeek.Monday - now.DayOfWeek;
                System.DateTime monday = now.AddDays(delta);
                result = Int64.Parse(monday.ToString("yyyyMMdd") + START_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        System.DateTime? StartWeekSystemDateTime(System.DateTime now)
        {
            System.DateTime? result = null;
            try
            {
                int delta = DayOfWeek.Monday - now.DayOfWeek;
                System.DateTime monday = now.AddDays(delta);
                long time = 0;
                Int64.TryParse(monday.ToString("yyyyMMdd") + START_TIME, out time);
                result = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private void gridControlServiceReqKidneyshift_Click(object sender, EventArgs e)
        {

        }
    }
}
