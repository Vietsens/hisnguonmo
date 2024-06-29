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
using ACS.SDO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.ExamServiceAdd.ADO;
using HIS.UC.HisExamServiceAdd.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HIS.UC.HisExamServiceAdd.ADO;
using MOS.SDO;
using MOS.Filter;
using DevExpress.XtraEditors.Controls;
using System.Globalization;

namespace HIS.UC.ExamServiceAdd.Run
{
    public partial class UCExamServiceAdd : UserControl
    {
        public long positionHandle = -1;
        ExamServiceAddInitADO examServiceAddInitADO { get; set; }
        List<V_HIS_SERVICE_ROOM> serviceRooms { get; set; }
        HIS_SERE_SERV sereServ { get; set; }
        HIS_TREATMENT currentTreatment { get; set; }
        V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter { get; set; }
        List<HIS_PATIENT_TYPE> hisPatientType { get; set; }
        List<long> currentPatientTypeIds;
        List<V_HIS_SERVICE_PATY> currentServicePatys { get; set; }

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.UC.ExamServiceAdd";
        long? NUM_ORDER_BLOCK_ID = 0;
        ResultChooseNumOrderBlockADO resultChooseNumOrderBlockADO;
        bool IsAllowNotChooseService = false;
        public UCExamServiceAdd(ExamServiceAddInitADO _examServiceAddInitADO)
        {
            InitializeComponent();
            try
            {
                this.examServiceAddInitADO = _examServiceAddInitADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCExamServiceAdd_Load(object sender, EventArgs e)
        {
            try
            {
                hisPatientType = new List<HIS_PATIENT_TYPE>();

                HisConfig.LoadConfig();

                CommonParam param = new CommonParam();
                TimerSDO timeSync = new BackendAdapter(param).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, param);
                
                if (HisConfig.IsUsingServerTime)
                {
                    dtIntructionTime.Enabled = false;

                    //Lay gio server
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));
                    
                    ;
                    if (timeSync != null)
                    {
                        dtIntructionTime.DateTime = timeSync.DateNow;
                    }
                }
                long istime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.ShowServerTimeByDefault");
                if (istime == 1)
                {
                    try
                    {
                        dtIntructionTime.DateTime = timeSync.DateNow;
                        
                        }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                else
                {
                    dtIntructionTime.DateTime = DateTime.Now;
                }
                ValidateForm();
                LoadTreatment();
                InitCombo();
                //Kham chua xu ly vu phu thu
                //if (HisConfig.IsSetPrimaryPatientType)
                //{
                //    lciPrimaryPatientType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //}

                //chkAppointment.Enabled = false;
                chkPrintAppointment.Enabled = false;
                dtAppointment.Enabled = false;
                txtAdvise.Enabled = false;

                LoadCurrentPatientTypeAlter();
                InitControlState();
                LoadDataCboExecuteRoom();
                LoadSereServ();
                CheckKhamHienTai();
                CheckKhongHuongBHYT();
                InitControlEnabled();
                //if (!examServiceAddInitADO.IsMainExam)
                //    chkAppointment.Enabled = true;
                //else
                //    chkAppointment.Enabled = false;
                if (examServiceAddInitADO.AppointmentTime != null)
                {
                    //chkAppointment.Checked = true;
                    //chkAppointment.Enabled = true;
                    dtAppointment.Enabled = true;
                    txtAdvise.Enabled = true;
                    dtAppointment.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(examServiceAddInitADO.AppointmentTime ?? 0) ?? DateTime.Now;
                    txtAdvise.Text = examServiceAddInitADO.AppointmentDesc;
                    layoutControlItem15.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                EnableChkIsPrimary();
                EnableChkExamFinish();
                // auto tich chuyen khoa
                chkIsBranch.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableChkExamFinish()
        {
            try
            {
                if (HisConfig.IsFinishExamAdd == "1")
                {
                    chkKetThucKhamHienTai.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                InitComboRoomAppointment();
                //InitComboServiceAppointment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboServiceAppointment(long? roomId)
        {
            try
            {
                long branchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                if (examServiceAddInitADO == null && branchId <= 0)// || !examServiceAddInitADO.roomId.HasValue)
                    throw new Exception("branchId null");
                List<V_HIS_SERVICE_ROOM> serviceRooms = new List<V_HIS_SERVICE_ROOM>();
                if (roomId != null)
                {
                    serviceRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == roomId && o.IS_ACTIVE == 1 && o.SERVICE_TYPE_ID == 1).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboServiceAppointment, serviceRooms, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboRoomAppointment()
        {
            try
            {
                long branchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                if (branchId <= 0)// || !examServiceAddInitADO.roomId.HasValue)
                    throw new Exception("branchId null");
                List<V_HIS_ROOM> rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(
                    o => o.IS_EXAM == 1 && o.IS_ACTIVE == 1
                        && o.BRANCH_ID == branchId)
                        .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboRoomAppointment, rooms, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                if (examServiceAddInitADO.treatmentId.HasValue)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                    treatmentFilter.ID = examServiceAddInitADO.treatmentId.Value;
                    currentTreatment = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableChkIsPrimary()
        {
            try
            {
                if (HisConfig.IsAutoSetIsMainForAdditionExam)
                {
                    chkIsPrimary.Checked = true;
                    chkIsPrimary.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckKhongHuongBHYT()
        {
            try
            {
                if (currentTreatment != null)
                {
                    if (currentTreatment.TDL_PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                    {
                        chkKhongHuongBHYT.Enabled = true;
                    }
                    else chkKhongHuongBHYT.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckKhamHienTai()
        {
            if (chkKetThucKhamHienTai.CheckState == CheckState.Checked)
            {
                lciFinishTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                if (examServiceAddInitADO != null && examServiceAddInitADO.FinishTime.HasValue)
                    dtFinishTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(examServiceAddInitADO.FinishTime.Value) ?? DateTime.Now;
                else
                    dtFinishTime.DateTime = DateTime.Now;
            }
            else
            {
                lciFinishTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExecuteRoom.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            LoadDataCboExamService(data);
                            txtExecuteRoom.Text = data.EXECUTE_ROOM_CODE;
                            txtService.Focus();
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
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadExamRoom(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadService(strValue, false);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExamService.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM data = serviceRooms.FirstOrDefault(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExamService.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtService.Text = data.SERVICE_CODE;
                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_KeyUp(object sender, KeyEventArgs e)
        {
            if (cboExecuteRoom.EditValue != null)
            {
                MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString()));
                if (data != null)
                {
                    LoadDataCboExamService(data);
                    txtExecuteRoom.Text = data.EXECUTE_ROOM_CODE;
                    txtService.Focus();
                }
            }
        }

        private void dtIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExecuteRoom.Focus();
                    txtExecuteRoom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrimaryPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPrimaryPatientType.EditValue != null)
                {
                    cboPrimaryPatientType.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboPrimaryPatientType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPrimaryPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPrimaryPatientType.EditValue = null;
                    cboPrimaryPatientType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ValidComboPatienType(cboExamService.EditValue != null);
                if (cboExamService.EditValue != null)
                {
                    //Load CbopatientType
                    long serviceId = Inventec.Common.TypeConvert.Parse.ToInt64(cboExamService.EditValue.ToString());
                    LoadCboPatientType(serviceId);
                    LoadCboPatientTypePrimary(serviceId);
                }
                else
                {
                    cboPatientType.EditValue = null;
                    cboPatientType.Properties.DataSource = null;
                    cboPrimaryPatientType.EditValue = null;
                    cboPrimaryPatientType.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidComboPatienType(bool IsRequire)
        {
            try
            {
                if (IsAllowNotChooseService)
                {
                    if (IsRequire)
                    {
                        this.layoutControlItem9.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                        ValidateSingleControl(cboPatientType);
                    }
                    else
                    {
                        this.layoutControlItem9.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                        RemoveValidateControl(cboPatientType);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboPatientType.EditValue != null)
                {
                    //cboPrimaryPatientType.Focus();
                    //cboPrimaryPatientType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                long serviceId = Inventec.Common.TypeConvert.Parse.ToInt64(cboExamService.EditValue.ToString());
                LoadCboPatientTypePrimary(serviceId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkIsPrimary_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsPrimary.Checked)
                {
                    chkKetThucKhamHienTai.Checked = true;
                    //chkAppointment.Enabled = true;
                }
                else
                {
                    chkKetThucKhamHienTai.Checked = false;
                    //chkAppointment.Enabled = false;
                }
                InitControlEnabled();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKetThucKhamHienTai_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckKhamHienTai();
                if (chkKetThucKhamHienTai.Checked)
                {
                    chkAppointment.Enabled = true;
                }
                else
                {
                    chkAppointment.Checked = false;
                    chkAppointment.Enabled = false;
                    chkPrintAppointment.Enabled = false;
                    dtAppointment.Enabled = false;
                    txtAdvise.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkKhongHuongBHYT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                List<HIS_PATIENT_TYPE> PatientTypeNew = new List<HIS_PATIENT_TYPE>();
                HIS_PATIENT_TYPE delete = new HIS_PATIENT_TYPE();
                List<HIS_PATIENT_TYPE> check = new List<HIS_PATIENT_TYPE>();

                if (hisPatientType != null && hisPatientType.Count > 0)
                {
                    PatientTypeNew.AddRange(hisPatientType);

                    delete = hisPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfig.PatientTypeCode__BHYT);

                    check = PatientTypeNew.Where(o => o.ID == delete.ID).ToList();

                    if (chkKhongHuongBHYT.Checked)
                    {
                        var select = hisPatientType.FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfig.PatientTypeCode__VP);
                        if (select != null)
                        {
                            cboPatientType.EditValue = select.ID;
                        }
                        else
                        {
                            cboPatientType.EditValue = null;
                        }

                        if (check != null && check.Count > 0)
                        {
                            PatientTypeNew.Remove(delete);
                        }
                    }
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientType, PatientTypeNew, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintExamAdd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintExamAdd.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintExamAdd.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintExamAdd.Name;
                    csAddOrUpdate.VALUE = (chkPrintExamAdd.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintExamAdd.Name)
                        {
                            chkPrintExamAdd.Checked = item.VALUE == "1";
                        }

                        if (item.KEY == chkPrintAppointment.Name)
                        {
                            chkPrintAppointment.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chk_IsDepartment.Name)
                        {
                            chk_IsDepartment.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkSignExamAdd.Name)
                        {
                            chkSignExamAdd.Checked = item.VALUE == "1";
                        }
                    }
                }
                chkIsBranch.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkAppointment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAppointment.Checked && chkAppointment.Enabled)
                {
                    //if (this.sereServ != null && this.sereServ.TDL_IS_MAIN_EXAM == 1)
                    //{
                    //    chkIsPrimary.Checked = true;
                    //}
                    chkPrintAppointment.Enabled = true;
                    dtAppointment.Enabled = true;
                    txtAdvise.Enabled = true;

                    var appointmentTimeDefault = Inventec.Common.DateTime.Calculation.Add(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinishTime.DateTime) ?? 0, HisConfig.AppointmentTime__DEFAULT, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0;
                    dtAppointment.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTimeDefault);

                    layoutControlItem15.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;

                    //if (examServiceAddInitADO != null && examServiceAddInitADO.IsBlockNumOrder)
                    //{
                    //    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    //}
                    //else
                    //{
                    //    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //}

                    txtServiceAppointment.Enabled = txtRoomAppointment.Enabled = cboServiceAppointment.Enabled = cboRoomAppointment.Enabled = true; ;
                }
                else
                {
                    chkPrintAppointment.Enabled = false;
                    dtAppointment.Enabled = false;
                    txtAdvise.Enabled = false;
                    layoutControlItem15.AppearanceItemCaption.ForeColor = System.Drawing.Color.Empty;
                    txtServiceAppointment.Enabled = txtRoomAppointment.Enabled = cboServiceAppointment.Enabled = cboRoomAppointment.Enabled = false;
                    cboServiceAppointment.EditValue = cboRoomAppointment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintAppointment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintAppointment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintAppointment.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintAppointment.Name;
                    csAddOrUpdate.VALUE = (chkPrintAppointment.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ActChooseDataProcess(ResultChooseNumOrderBlockADO _resultChooseNumOrderBlockADO)
        {
            try
            {
                //TODO
                this.resultChooseNumOrderBlockADO = _resultChooseNumOrderBlockADO;
                if ((Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAppointment.DateTime) ?? 0) != this.resultChooseNumOrderBlockADO.Date)
                {
                    dtAppointment.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.resultChooseNumOrderBlockADO.Date);
                }
                ProcessLoadGetOccupiedStatus();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadGetOccupiedStatus()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.1");
                List<HisNumOrderBlockSDO> HisNumOrderBlockSDOs = new List<HisNumOrderBlockSDO>();
                NUM_ORDER_BLOCK_ID = null;
                HisNumOrderBlockOccupiedStatusFilter filter = new HisNumOrderBlockOccupiedStatusFilter();
                filter.ISSUE_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dtAppointment.DateTime.ToString("yyyyMMdd") + "000000");
                filter.ROOM_ID = examServiceAddInitADO.DefaultIdRoom ?? 0;
                var HisNumOrderBlockSDOTmps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HisNumOrderBlockSDO>>("api/HisNumOrderBlock/GetOccupiedStatus", ApiConsumers.MosConsumer, filter, null);
                if (HisNumOrderBlockSDOTmps != null && HisNumOrderBlockSDOTmps.Count > 0)
                {
                    HisNumOrderBlockSDOs = HisNumOrderBlockSDOTmps.Where(o => o.IS_ISSUED == null || o.IS_ISSUED != 1).OrderBy(o => o.FROM_TIME).ToList();
                    if (HisNumOrderBlockSDOs != null && HisNumOrderBlockSDOs.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.2");
                        if (resultChooseNumOrderBlockADO != null && resultChooseNumOrderBlockADO.NumOrderBlock != null && resultChooseNumOrderBlockADO.NumOrderBlock.NUM_ORDER_BLOCK_ID > 0)
                        {
                            NUM_ORDER_BLOCK_ID = resultChooseNumOrderBlockADO.NumOrderBlock.NUM_ORDER_BLOCK_ID;
                        }
                        else
                            NUM_ORDER_BLOCK_ID = HisNumOrderBlockSDOs.FirstOrDefault().NUM_ORDER_BLOCK_ID;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisNumOrderBlockSDOs.FirstOrDefault()", HisNumOrderBlockSDOs.FirstOrDefault())
                            + Inventec.Common.Logging.LogUtil.TraceData("resultChooseNumOrderBlockADO", resultChooseNumOrderBlockADO));
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.3");
                        List<HisAppointmentPeriodCountByDateSDO> ListTime = new List<HisAppointmentPeriodCountByDateSDO>();
                        NUM_ORDER_BLOCK_ID = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Đã cấp hết số khám vào ngày {0}", dtAppointment.DateTime.ToString("dd/MM")),
                             Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Đã cấp hết số khám vào ngày {0}", dtAppointment.DateTime.ToString("dd/MM")),
                             Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    NUM_ORDER_BLOCK_ID = null;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisNumOrderBlockSDOTmps), HisNumOrderBlockSDOTmps)
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisNumOrderBlockSDOs), HisNumOrderBlockSDOs));
                Inventec.Common.Logging.LogSystem.Debug("ProcessLoadGetOccupiedStatus.4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChonKhungGio1_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                NumOrderBlockChooserADO numOrderBlockChooserADO = new Desktop.ADO.NumOrderBlockChooserADO();


                numOrderBlockChooserADO.DefaultRoomId = examServiceAddInitADO.DefaultIdRoom;

                numOrderBlockChooserADO.DefaultDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAppointment.DateTime) ?? 0;
                //numOrderBlockChooserADO.DisableDate = false;
                //numOrderBlockChooserADO.DisableRoom = false;
                List<V_HIS_ROOM> lst = new List<V_HIS_ROOM>();
                lst.Add(BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == (examServiceAddInitADO.DefaultIdRoom ?? 0)).FirstOrDefault());
                numOrderBlockChooserADO.ListRoom = lst;
                numOrderBlockChooserADO.DelegateChooseData = ActChooseDataProcess;//TODO
                bool? isNeedTimeString = true;
                listArgs.Add(isNeedTimeString);

                Inventec.Common.Logging.LogSystem.Debug("Call module HIS.Desktop.Plugins.HisNumOrderBlockChooser: Input data: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numOrderBlockChooserADO.DefaultRoomId), numOrderBlockChooserADO.DefaultRoomId)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numOrderBlockChooserADO.DefaultDate), numOrderBlockChooserADO.DefaultDate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numOrderBlockChooserADO.ListRoom), numOrderBlockChooserADO.ListRoom));

                listArgs.Add(numOrderBlockChooserADO);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisNumOrderBlockChooser", 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chk_IsDepartment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkIsBranch.Enabled = !chk_IsDepartment.Checked;
                cboExecuteRoom.EditValue = null;
                txtExecuteRoom.Text = "";
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chk_IsDepartment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chk_IsDepartment.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chk_IsDepartment.Name;
                    csAddOrUpdate.VALUE = (chk_IsDepartment.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                LoadDataCboExecuteRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsBranch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cboExecuteRoom.EditValue = null;
                txtExecuteRoom.Text = "";
                LoadDataCboExecuteRoom();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (HisConfig.AutoCheckChangeDepartment == "1" && cboExecuteRoom.EditValue != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExecuteRoom.EditValue));

                    if (room != null && currentTreatment != null)
                    {
                        chkChangeDepartment.Checked = room.DEPARTMENT_ID != currentTreatment.LAST_DEPARTMENT_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignExamAdd.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignExamAdd.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignExamAdd.Name;
                    csAddOrUpdate.VALUE = (chkSignExamAdd.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoomAppointment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtRoomAppointment.Text = null;
                    cboRoomAppointment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceAppointment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtServiceAppointment.Text = null;
                    cboServiceAppointment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRoomAppointment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadRoomAppointment(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRoomAppointment(string searchCode, bool v)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboRoomAppointment.Focus();
                    cboRoomAppointment.ShowPopup();
                    gridView3.FocusedRowHandle = 0;
                }
                else
                {

                    long branchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                    if (branchId <= 0)// || !examServiceAddInitADO.roomId.HasValue)
                        throw new Exception("branchId null");
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(
                        o => o.IS_EXAM == 1 && o.IS_ACTIVE == 1
                            && o.BRANCH_ID == branchId && o.ROOM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboRoomAppointment.EditValue = data[0].ID;
                            cboServiceAppointment.Focus();
                            cboServiceAppointment.ShowPopup();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ROOM_CODE == searchCode);
                            if (search != null)
                            {
                                cboRoomAppointment.EditValue = search.ID;
                                cboServiceAppointment.Focus();
                                cboServiceAppointment.ShowPopup();
                            }
                            else
                            {
                                cboRoomAppointment.EditValue = null;
                                cboRoomAppointment.Focus();
                                cboRoomAppointment.ShowPopup();
                                gridView3.FocusedRowHandle = 0;
                            }
                        }
                    }
                    else
                    {
                        cboRoomAppointment.EditValue = null;
                        cboRoomAppointment.Focus();
                        cboRoomAppointment.ShowPopup();
                        gridView3.FocusedRowHandle = 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceAppointment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadServiceAppointment(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceAppointment(string searchCode, bool v)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboServiceAppointment.Focus();
                    cboServiceAppointment.ShowPopup();
                    gridView4.FocusedRowHandle = 0;
                }
                else
                {
                    long? roomId = cboRoomAppointment.EditValue != null ? (long?)Convert.ToInt64(cboRoomAppointment.EditValue.ToString()) : null;
                    if (roomId == null) return;
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == roomId && o.IS_ACTIVE == 1 && o.SERVICE_TYPE_ID == 1 && o.SERVICE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboServiceAppointment.EditValue = data[0].ID;
                            cboServiceAppointment.Focus();
                            cboServiceAppointment.ShowPopup();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ROOM_CODE == searchCode);
                            if (search != null)
                            {
                                cboServiceAppointment.EditValue = search.ID;
                                cboServiceAppointment.Focus();
                                cboServiceAppointment.ShowPopup();
                            }
                            else
                            {
                                cboServiceAppointment.EditValue = null;
                                cboServiceAppointment.Focus();
                                cboServiceAppointment.ShowPopup();
                                gridView4.FocusedRowHandle = 0;
                            }
                        }
                    }
                    else
                    {
                        cboServiceAppointment.EditValue = null;
                        cboServiceAppointment.Focus();
                        cboServiceAppointment.ShowPopup();
                        gridView4.FocusedRowHandle = 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoomAppointment_Closed(object sender, ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                InitComboServiceAppointment(null);
                txtRoomAppointment.Text = null;
                txtServiceAppointment.Text = null;
                cboServiceAppointment.EditValue = null;
                if (cboRoomAppointment.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ROOM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomAppointment.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        InitComboServiceAppointment(data.ID);
                        txtRoomAppointment.Text = data.ROOM_CODE;
                    }
                }
            }
        }

        private void cboServiceAppointment_Closed(object sender, ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                txtServiceAppointment.Text = "";
                if (cboServiceAppointment.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().FirstOrDefault(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceAppointment.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtServiceAppointment.Text = data.SERVICE_CODE;
                    }
                }
            }
        }
    }
}
