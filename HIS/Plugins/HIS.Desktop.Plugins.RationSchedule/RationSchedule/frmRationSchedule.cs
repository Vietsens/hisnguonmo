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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using AutoMapper;
using HIS.Desktop.Plugins.RationSchedule;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Threading;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.RationSchedule.Validtion;
using MOS.SDO;
using System.Globalization;

namespace HIS.Desktop.Plugins.RationSchedule.RationSchedule
{
    public partial class frmRationSchedule : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        List<MOS.EFMODEL.DataModels.V_HIS_RATION_SCHEDULE> lstData;
        MOS.EFMODEL.DataModels.V_HIS_RATION_SCHEDULE currentData;
        long departmentId = 0;
        bool IsActionTextEdit = false;
        V_HIS_SERVICE currentService { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_RATION_TIME> lstRationTime { get; set; }
        List<V_HIS_SERVICE> lstServiceAll { get; set; }
        List<V_HIS_SERVICE> lstServiceSA { get; set; }
        List<HIS_PATIENT_TYPE> lstPatientTypeAll { get; set; }
        List<HIS_REFECTORY> lstReceptionRoomAll { get; set; }
        List<HIS_REFECTORY> lstReceptionRoomSA { get; set; }
        List<HIS_SERVICE_RATI> lstServiceRatiAll { get; set; }
        List<HIS_SERVICE_ROOM> lstServiceRoomAll { get; set; }
        List<HIS_SERVICE_ROOM> lstServiceRoomSA { get; set; }
        private MOS.SDO.WorkPlaceSDO WorkPlaceSDO;
        private string loginName;
        private bool IsAdmin;
        private Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs { get; set; }
        L_HIS_TREATMENT_BED_ROOM treatmentBedRoom;
        long treatmentId;
        #endregion


        #region FormConstructor

        public frmRationSchedule(Inventec.Desktop.Common.Modules.Module moduleData, L_HIS_TREATMENT_BED_ROOM treatmentBedRoom)
        : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

                try
                {
                    this.treatmentBedRoom = treatmentBedRoom;
                    treatmentId = treatmentBedRoom.TREATMENT_ID;
                    loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    IsAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName);
                    WorkPlaceSDO = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == moduleData.RoomId);
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmRationSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                departmentId = WorkPlaceSDO.DepartmentId;
                SetCaptionByLanguageKey();
                LoadDataToGrid();
                Init();
                timer1.Start();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmRationSchedule
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RationSchedule.Resources.Lang", typeof(frmRationSchedule).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickSearch.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnQuickSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickAdd.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnQuickAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickEdit.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnQuickEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQuickReset.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnQuickReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnF2.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnF2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkHalfInFirstDay.Properties.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.chkHalfInFirstDay.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkForHome.Properties.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.chkForHome.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboReceptionRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRationSchedule.cboReceptionRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRationSchedule.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceName.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRationSchedule.cboServiceName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRationTime.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRationSchedule.cboRationTime.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem14.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmRationSchedule.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCode.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.gridColumnCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnName.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.gridColumnName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CREATE_TIME.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.CREATE_TIME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CREATOR.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.CREATOR.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.MODIFY_TIME.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.MODIFY_TIME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.gridColumnModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmRationSchedule.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmRationSchedule.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public void Init()
        {
            try
            {
                LoadDataPatientType();
                CreateThreadByServiceReq();
                FillCombo();
                SetDefaultValue();
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillCombo()
        {
            try
            {
                LoadDataToCbo(cboRationTime, lstRationTime, "RATION_TIME_NAME", "RATION_TIME_CODE", "ID");
                LoadDataToCbo(cboServiceName, lstServiceAll, "SERVICE_NAME", "SERVICE_CODE", "ID");
                LoadDataToCbo(cboPatientType, lstPatientTypeAll, "PATIENT_TYPE_NAME", "PATIENT_TYPE_CODE", "ID");
                LoadDataToCbo(cboReceptionRoom, lstReceptionRoomAll, "REFECTORY_NAME", "REFECTORY_CODE", "ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadByServiceReq()
        {
            Thread threadServicePaty = new Thread(new ThreadStart(LoadDataServicePaty));
            Thread threadRationTime = new Thread(new ThreadStart(LoadDataRationTime));
            Thread threadService = new Thread(new ThreadStart(LoadDataService));
            Thread threadReceptionRoom = new Thread(new ThreadStart(LoadDataReceptionRoom));
            Thread threadServiceRati = new Thread(new ThreadStart(LoadDataServiceRati));
            Thread threadServiceRoom = new Thread(new ThreadStart(LoadDataServiceRoom));
            try
            {
                threadServicePaty.Start();
                threadRationTime.Start();
                threadService.Start();
                threadReceptionRoom.Start();
                threadServiceRati.Start();
                threadServiceRoom.Start();
                threadServicePaty.Join();
                threadRationTime.Join();
                threadService.Join();
                threadReceptionRoom.Join();
                threadServiceRati.Join();
                threadServiceRoom.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadRationTime.Abort();
                threadService.Abort();
                threadReceptionRoom.Abort();
                threadServiceRati.Abort();
                threadServiceRoom.Abort();
                threadServicePaty.Abort();
            }
        }

        private void LoadDataServicePaty()
        {
            try
            {
                var patientTypeIdAls = lstPatientTypeAll.Select(o => o.ID);
                var servicePatyTemps = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
                    .ToList();
                this.servicePatyInBranchs = servicePatyTemps
                    .GroupBy(o => o.SERVICE_ID)
                    .ToDictionary(o => o.Key, o => o.ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadDataServiceRoom()
        {
            try
            {
                lstServiceRoomAll = BackendDataWorker.Get<HIS_SERVICE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceRati()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceRatiFilter ratiFilter = new HisServiceRatiFilter();
                ratiFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                lstServiceRatiAll = new BackendAdapter(param).Get<List<HIS_SERVICE_RATI>>("api/HisServiceRati/Get", ApiConsumer.ApiConsumers.MosConsumer, ratiFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataReceptionRoom()
        {
            try
            {
                lstReceptionRoomAll = BackendDataWorker.Get<HIS_REFECTORY>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatientType()
        {
            try
            {
                lstPatientTypeAll = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_RATION == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataService()
        {
            try
            {
                lstServiceAll = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataRationTime()
        {
            try
            {
                lstRationTime = BackendDataWorker.Get<HIS_RATION_TIME>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ValidateForm()
        {
            try
            {
                ValidGridlookUp(cboRationTime);
                ValidGridlookUpWithText(cboServiceName, txtServiceCode);
                ValidGridlookUp(cboPatientType);
                ValidGridlookUp(cboReceptionRoom);
                ValidTimeFrom(dteFrom);
                ValidTimeTo(dteFrom,dteTo);
                ValidMaxLength(memNote);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidMaxLength(MemoEdit mem)
        {
            try
            {
                ValidateMaxLength rule = new ValidateMaxLength();
                rule.textEdit = mem;
                rule.maxLength = 1000;
                dxValidationProviderInfo.SetValidationRule(mem, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidGridlookUp(GridLookUpEdit cbo)
        {
            try
            {
                GridLookupValidationRule rule = new GridLookupValidationRule();
                rule.gridLookUpEdit = cbo;
                dxValidationProviderInfo.SetValidationRule(cbo, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidGridlookUpWithText(GridLookUpEdit cbo, TextEdit txt)
        {
            try
            {
                GridLookupWithTextValidationRule rule = new GridLookupWithTextValidationRule();
                rule.gridLookUpEdit = cbo;
                rule.txt = txt;
                dxValidationProviderInfo.SetValidationRule(txt, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidTimeFrom(DateEdit dte)
        {
            try
            {
                TimeFromValidationRule rule = new TimeFromValidationRule();
                rule.dtFrom = dte;
                dxValidationProviderInfo.SetValidationRule(dte, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidTimeTo(DateEdit dteF, DateEdit dteT)
        {
            try
            {
                TimeToValidationRule rule = new TimeToValidationRule();
                rule.dtFrom = dteF;
                rule.dtTo = dteT;
                dxValidationProviderInfo.SetValidationRule(dteT, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                currentData = null;
                cboRationTime.EditValue = null;
                txtServiceCode.Text = null;
                cboServiceName.EditValue = null;
                cboPatientType.EditValue = null;
                cboReceptionRoom.EditValue = null;
                spnAmount.EditValue = 1;
                spnAmount.Enabled = false;
                if (lstData != null && lstData.Count > 0 && lstData.Max(o => o.TO_TIME ?? 0) != 0)
                {
                    dteFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(lstData.Max(o => o.TO_TIME ?? 0)) ?? DateTime.Now;
                }
                else
                    dteFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(treatmentBedRoom.ADD_TIME) ?? DateTime.Now;
                dteTo.EditValue = null;
                memNote.EditValue = null;
                chkForHome.Checked = false;
                chkHalfInFirstDay.Checked = false;
                dteFrom.Enabled = true;
                dteTo.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisRationScheduleFilter filter = new HisRationScheduleFilter();
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.TREATMENT_ID__EXACT = treatmentId;
                gridView1.BeginUpdate();
                lstData =  new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_RATION_SCHEDULE>>("api/HisRationSchedule/GetView", ApiConsumers.MosConsumer, filter, param);
                gridControlFormList.DataSource = lstData;
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_RATION_SCHEDULE pData = (MOS.EFMODEL.DataModels.V_HIS_RATION_SCHEDULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "FROM_TIME_str" && pData.FROM_TIME != null)
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.FROM_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "TO_TIME_str" && pData.TO_TIME != null)
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.TO_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_FOR_HOMIE_bool" && pData.IS_FOR_HOMIE != null)
                    {
                        try
                        {
                            e.Value = true;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_HALF_IN_FIRST_DAY_bool" && pData.IS_HALF_IN_FIRST_DAY != null)
                    {
                        try
                        {
                            e.Value = true;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeDataRow(V_HIS_RATION_SCHEDULE rowData)
        {
            try
            {
                if (rowData != null)
                {
                    FillDataToEditorControl(rowData);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(V_HIS_RATION_SCHEDULE rowData)
        {
            try
            {
                cboRationTime.EditValue = rowData.RATION_TIME_ID;
                txtServiceCode.Text = rowData.SERVICE_CODE;
                cboServiceName.EditValue = rowData.SERVICE_ID;
                cboPatientType.EditValue = rowData.PATIENT_TYPE_ID;
                cboReceptionRoom.EditValue = rowData.REFECTORY_ROOM_ID;
                spnAmount.EditValue = rowData.AMOUNT;
                dteFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rowData.FROM_TIME ?? 0) ?? DateTime.Now;
                if (rowData.TO_TIME.HasValue)
                    dteTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rowData.TO_TIME ?? 0) ?? DateTime.Now;
                memNote.Text = rowData.NOTE;
                chkForHome.Checked = rowData.IS_FOR_HOMIE == (short?)1;
                chkHalfInFirstDay.Checked = rowData.IS_HALF_IN_FIRST_DAY == (short?)1;
                if (rowData.LAST_ASSIGN_DATE != null)
                {
                    dteFrom.Enabled = false;
                    if (rowData.TO_TIME.HasValue && rowData.LAST_ASSIGN_DATE >= (rowData.TO_TIME ?? 0))
                        dteTo.Enabled = false;
                }
                if (currentService.IS_MULTI_REQUEST > 1)
                    spnAmount.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderInfo, dxErrorProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess(bool Print)
        {
            try
            {
                CommonParam param = new CommonParam();

                try
                {
                    bool success = false;

                    if (!btnSave.Enabled && !btnSaveAndPrint.Enabled)
                    {
                        return;
                    }

                    if (!dxValidationProviderInfo.Validate())
                    {
                        return;
                    }
                    RationScheduleSDO data = new RationScheduleSDO();
                    string api = "api/HisRationSchedule/Create";
                    if (currentData != null)
                    {
                        api = "api/HisRationSchedule/Update";
                        data.RationScheduleId = currentData.ID;
                    }
                    data.ReqRoomId = moduleData.RoomId;
                    data.TreatmentId = treatmentId;
                    data.RationTimeId = Int64.Parse(cboRationTime.EditValue.ToString());
                    data.PatientTypeId = Int64.Parse(cboPatientType.EditValue.ToString());
                    data.ServiceId = currentService.ID;
                    data.Amount = (long)spnAmount.Value;
                    data.RefectoryRoomId = Int64.Parse(cboReceptionRoom.EditValue.ToString());
                    data.FromTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteFrom.DateTime) ?? 0;
                    if(dteTo.EditValue != null && dteTo.DateTime != DateTime.MinValue)
                        data.ToTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteTo.DateTime) ?? 0;
                    data.IsForHomie = chkForHome.Checked;
                    data.HalfInFirstDay = chkHalfInFirstDay.Checked;
                    data.Note = memNote.Text.Trim();
                    
                    string dateInTime = treatmentBedRoom.CLINICAL_IN_TIME.ToString();
                    DateTime dateTime = DateTime.ParseExact(dateInTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    string formattedDateTime = dateTime.ToString("dd/MM/yyyy HH:mm:ss");
                    if (data.FromTime < treatmentBedRoom.CLINICAL_IN_TIME && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian từ nhỏ hơn thời gian vào điều trị {0}.", formattedDateTime), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK) == DialogResult.OK)
                            return;
                    if (data.RationScheduleId != null && currentData.LAST_ASSIGN_DATE != null && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Lịch báo ăn đã có chỉ định suất ăn tương ứng. Bạn có muốn thực hiện không?", treatmentBedRoom.TREATMENT_CODE), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    WaitingManager.Show();

                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_RATION_SCHEDULE>(api, ApiConsumers.MosConsumer, data, param);

                    if (resultData != null)
                    {
                        success = true;
                        LoadDataToGrid();
                        currentData = lstData.FirstOrDefault(o => o.ID == resultData.ID);
                        if (Print)
                            PrintMps492();
                    }

                    WaitingManager.Hide();
                    if (success)
                        SetDefaultValue();
                    MessageManager.Show(this, param, success);
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintMps492()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000492", DelegateRunPrinter);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000492":
                        Mps492(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps492(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentBedRoom.TREATMENT_CODE, printTypeCode, moduleData.RoomId);

                MPS.Processor.Mps000492.PDO.Mps000492PDO pdo = new MPS.Processor.Mps000492.PDO.Mps000492PDO(
                       currentData,
                       treatmentBedRoom
                       );
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToCbo(GridLookUpEdit cbo, object data, string DisplayName, string DisplayCode, string Value)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayName;
                cbo.Properties.ValueMember = Value;
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;

                cbo.Properties.View.Columns.Clear();
                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField(DisplayCode);
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 70;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField(DisplayName);
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 250;

                cbo.Properties.PopupFormSize = new Size(320, cbo.Properties.PopupFormSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                LoadDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    V_HIS_RATION_SCHEDULE HisRationScheduleFocus = (V_HIS_RATION_SCHEDULE)gridView1.GetFocusedRow();
                    if (HisRationScheduleFocus != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>
                            ("api/HisRationSchedule/Delete", ApiConsumers.MosConsumer, HisRationScheduleFocus.ID, param);
                        if (success)
                        {
                            SetDefaultValue();
                            LoadDataToGrid();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_RATION_SCHEDULE data = (V_HIS_RATION_SCHEDULE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.LAST_ASSIGN_DATE == null && data.DEPARTMENT_ID == departmentId && (data.CREATOR == loginName || IsAdmin)) ? gridButtonDelete : deleteD;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }


        private void gridView1_Click_1(object sender, EventArgs e)
        {
            try
            {

                var rowData = (V_HIS_RATION_SCHEDULE)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    SetDefaultValue();
                    currentData = rowData;
                    ChangeDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #region ShortCut
        private void btnQuickSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQuickAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndPrint.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQuickEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQuickReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset.PerformClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ValueChangedLoadCombo
        private void cboRationTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRationTime.EditValue != null)
                {
                    var ListServiceRati = lstServiceRatiAll.Where(o => o.RATION_TIME_ID == Int64.Parse(cboRationTime.EditValue.ToString())).ToList();
                    if (ListServiceRati != null && ListServiceRati.Count > 0)
                    {
                        lstServiceSA = lstServiceAll.Where(o => ListServiceRati.Exists(p => p.SERVICE_ID == o.ID)).ToList();
                    }
                    else
                    {
                        lstServiceSA = new List<V_HIS_SERVICE>();
                    }
                    LoadDataToCbo(cboServiceName, lstServiceSA, "SERVICE_NAME", "SERVICE_CODE", "ID");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceName.EditValue != null)
                {
                    LoadService(Int64.Parse(cboServiceName.EditValue.ToString()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadService(object key)
        {
            try
            {
                if (!IsActionTextEdit)
                    currentService = lstServiceSA.FirstOrDefault(o => o.ID == (long)key);
                else
                    currentService = lstServiceSA.FirstOrDefault(o => o.SERVICE_CODE == key.ToString());
                if (currentService != null)
                {
                    if (currentService.IS_MULTI_REQUEST == 1)
                        spnAmount.Enabled = true;
                    else
                        spnAmount.Enabled = false;
                    txtServiceCode.Text = currentService.SERVICE_CODE;
                    lstServiceRoomSA = lstServiceRoomAll.Where(o => o.SERVICE_ID == currentService.ID).ToList();
                    if (lstServiceRoomSA != null && lstServiceRoomSA.Count > 0)
                    {
                        lstReceptionRoomSA = lstReceptionRoomAll.Where(o => lstServiceRoomSA.Exists(p => p.ROOM_ID == o.ROOM_ID)).ToList();
                    }
                    else
                    {
                        lstReceptionRoomSA = new List<HIS_REFECTORY>();
                    }
                    LoadDataToCbo(cboReceptionRoom, lstReceptionRoomSA, "REFECTORY_NAME", "REFECTORY_CODE", "ID");
                    var ListServicePaty = servicePatyInBranchs.ContainsKey(currentService.ID) ? servicePatyInBranchs[currentService.ID] : null;
                    List<HIS_PATIENT_TYPE> lstPatientType = new List<HIS_PATIENT_TYPE>();
                    if (ListServicePaty != null)
                    {
                        lstPatientType = lstPatientTypeAll.Where(o => ListServicePaty.Exists(p => p.PATIENT_TYPE_ID == o.ID)).ToList();
                    }
                    LoadDataToCbo(cboPatientType, lstPatientTypeAll, "PATIENT_TYPE_NAME", "PATIENT_TYPE_CODE", "ID");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtServiceCode.Text.Trim()))
                    {
                        LoadService(txtServiceCode.Text.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Init();
                timer1.Stop();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var currentDataTmp = currentData;
                currentData = (V_HIS_RATION_SCHEDULE)gridView1.GetFocusedRow();
                PrintMps492();
                currentData = currentDataTmp;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
