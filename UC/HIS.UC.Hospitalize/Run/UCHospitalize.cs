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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using MOS.EFMODEL.DataModels;
using HIS.UC.Hospitalize.ADO;
using HIS.UC.Hospitalize.Config;
using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using HIS.UC.SecondaryIcd;
using HIS.UC.Hospitalize.ValidateRule;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.UC.Hospitalize.Run
{
    public partial class UCHospitalize : UserControl
    {
        int positionHandleControl = -1;
        List<V_HIS_DEPARTMENT_1> listDepartment { get; set; }
        List<HIS_TREATMENT_TYPE> listTreatmentType { get; set; }
        HospitalizeInitADO hospitalizeInitADO { get; set; }
        List<V_HIS_BED_ROOM> listBedRoom { get; set; }
        CheckEdit_CheckChange CheckSign_CheckChange = null;
        CheckEdit_CheckChange CheckPrintDocumentSign_CheckChange = null;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "";
        bool isNotLoadWhileChangeControlStateInFirst;
        long RoomId = 0;
        bool IsFirstLoad = false;
        private Desktop.Plugins.Library.CheckIcd.CheckIcdManager checkIcdManager { get; set; }
        private Desktop.Plugins.Library.CheckIcd.DelegateRefeshIcd dlgRefeshIcd { get; set; }
        HIS_TREATMENT treatment { get; set; }
        string icdSubCodeScreeen { get; set; }
        public UCHospitalize()
        {
            InitializeComponent();
        }
        public UCHospitalize(HospitalizeInitADO _hospitalizeInitADO)
        {
            InitializeComponent();
            try
            {
                this.hospitalizeInitADO = _hospitalizeInitADO;
                this.CheckSign_CheckChange = _hospitalizeInitADO.CheckEditSign_CheckChange;
                this.CheckPrintDocumentSign_CheckChange = _hospitalizeInitADO.CheckEditPrintDocumentSign_CheckChange;
                this.moduleLink = _hospitalizeInitADO.ModuleLink;
                this.treatment = _hospitalizeInitADO.Treatment;
                this.dlgRefeshIcd = _hospitalizeInitADO.dlgRefeshIcd;
                this.icdSubCodeScreeen = _hospitalizeInitADO.dlgSendIcd();
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

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void UCHospitalize_Load(object sender, EventArgs e)
        {
            try
            {
                IsFirstLoad = true;
                HisConfig.LoadConfig();
                this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                if (this.RoomId > 0)
                {
                    this.isAllowNoIcd = (BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.RoomId).IS_ALLOW_NO_ICD == 1);
                }

                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                this.currentTraditionalIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && p.IS_TRADITIONAL == 1).OrderBy(o => o.ICD_CODE).ToList();
                this.isAutoCheckIcd = (this.autoCheckIcd == 1);
                UCIcdInit();
                ValidateForm();
                LoadDataToComboCareer();
                LoadDataToDepartmentComboExecute();
                LoadDataToComboTreatmentType();
                TimerSDO timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                if (HisConfig.IsUsingServerTime)
                {
                    dtLogTime.Enabled = false;
                    //Lay gio server
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));
                    ;
                    if (timeSync != null)
                    {
                        dtLogTime.DateTime = timeSync.DateNow;
                    }
                }
                long istime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.ShowServerTimeByDefault");
                if (istime == 1)
                {
                    try
                    {
                        dtLogTime.DateTime = timeSync.DateNow;

                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                else
                {
                    dtLogTime.DateTime = DateTime.Now;
                }
                if (HisConfig.RelativesInforOption == "1")
                {
                    layoutControlItem21.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem22.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem23.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateRelative(txtRELATIVE_NAME);
                    ValidateRelative(txtRELATIVE_PHONE);
                    ValidateRelative(txtRELATIVE_ADDRESS);
                }
                else
                {
                    layoutControlItem21.AppearanceItemCaption.ForeColor = Color.Black;
                    layoutControlItem22.AppearanceItemCaption.ForeColor = Color.Black;
                    layoutControlItem23.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider1.SetValidationRule(txtRELATIVE_NAME, null);
                    dxValidationProvider1.SetValidationRule(txtRELATIVE_PHONE, null);
                    dxValidationProvider1.SetValidationRule(txtRELATIVE_ADDRESS, null);
                }
                txtRELATIVE_PHONE.Text = this.hospitalizeInitADO.RelativePhone;
                txtRELATIVE_ADDRESS.Text = this.hospitalizeInitADO.RelativeAddress;
                txtRELATIVE_NAME.Text = this.hospitalizeInitADO.RelativeName;
                cboCareer.EditValue = this.hospitalizeInitADO.CareerId;
                if (this.hospitalizeInitADO.isAutoCheckChkHospitalizeExam)
                {
                    chkPrintHospitalizeExam.CheckState = CheckState.Checked;
                }
                else
                {
                    chkPrintHospitalizeExam.CheckState = CheckState.Unchecked;
                }
                if (this.hospitalizeInitADO != null && this.hospitalizeInitADO.FinishTime.HasValue)
                {
                    dtFinishTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.hospitalizeInitADO.FinishTime.Value) ?? DateTime.Now;
                }
                else
                {
                    dtFinishTime.DateTime = DateTime.Now;
                }

                if (this.hospitalizeInitADO.isEmergency)
                {
                    chkEmergency.CheckState = CheckState.Checked;
                }
                else
                {
                    chkEmergency.CheckState = CheckState.Unchecked;
                }

                LoadIcdToControl(this.hospitalizeInitADO.IcdCode, this.hospitalizeInitADO.IcdName);
                LoadTraditionalIcdToControl(this.hospitalizeInitADO.TraditionalIcdCode, this.hospitalizeInitADO.TraditionalIcdName);
                LoadTraditionalSubIcdToControl(this.hospitalizeInitADO.TraditionalIcdSubCode, this.hospitalizeInitADO.TraditionalIcdText);
                txtNote.Text = this.hospitalizeInitADO.Note;
                LoadDataToIcdSub(this.hospitalizeInitADO.IcdSubCode, this.hospitalizeInitADO.IcdText);
                LoadHospitalReason();
                ValidateForm();
                UpdateCheckPrintAndSign();
                checkIcdManager = new Desktop.Plugins.Library.CheckIcd.CheckIcdManager(dlgRefeshIcd, treatment);
                IsFirstLoad = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHospitalReason()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_HOSPITALIZE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                txtHospitalReasonCode.Text = this.hospitalizeInitADO.InHospitalizationReasonCode;
                GridLookupReasonEditValidationRule validCode = new GridLookupReasonEditValidationRule();
                validCode.maxLength = 10;
                validCode.ListObject = data;
                validCode.textEdit = txtHospitalReasonCode;
                if(HisConfig.IsInHospitalizationReasonRequired)
                {
                    lciHospitalReasonCode.AppearanceItemCaption.ForeColor = Color.Maroon;
                    validCode.IsRequired = true;
                }
                dxValidationProvider1.SetValidationRule(txtHospitalReasonCode, validCode);


                btnHospitalReasonName.Text = this.hospitalizeInitADO.InHospitalizationReasonName;
                ValidateMaxLength validName = new ValidateMaxLength();
                validName.maxLength = 1000;
                validName.textEdit = btnHospitalReasonName;
                if (HisConfig.IsInHospitalizationReasonRequired)
                {
                    validName.IsRequired = true;
                }
                dxValidationProvider1.SetValidationRule(btnHospitalReasonName, validName);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HOSPITALIZE_REASON_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("HOSPITALIZE_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HOSPITALIZE_REASON_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboHospitalReasonName, data, controlEditorADO);
                if(data != null)
                gridControl1.DataSource = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.chkSign)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkPrintDocumentSigned)
                        {
                            chkPrintDocumentSigned.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkCCS)
                        {
                            chkCCS.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPrintMps178.Name)
                        {
                            chkPrintMps178.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtTreatmentTypeCode.Text))
                    {
                        cboTreatmentType.EditValue = null;
                        cboTreatmentType.Focus();
                        cboTreatmentType.ShowPopup();
                    }
                    else
                    {
                        List<HIS_TREATMENT_TYPE> searchs = null;
                        var listData = this.listTreatmentType.Where(o => o.TREATMENT_TYPE_CODE.ToUpper().Contains(txtTreatmentTypeCode.Text.ToUpper())).ToList();
                        if (listData != null && listData.Count > 0)
                        {
                            searchs = (listData.Count == 1) ? listData : (listData.Where(o => o.TREATMENT_TYPE_CODE.ToUpper() == txtTreatmentTypeCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtTreatmentTypeCode.Text = searchs[0].TREATMENT_TYPE_CODE;
                            cboTreatmentType.EditValue = searchs[0].ID;
                            txtDepartmentCode.Focus();
                            txtDepartmentCode.SelectAll();
                        }
                        else
                        {
                            cboTreatmentType.Focus();
                            cboTreatmentType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        var data = this.listTreatmentType.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtTreatmentTypeCode.Text = data.TREATMENT_TYPE_CODE;
                        }
                        txtDepartmentCode.Focus();
                        txtDepartmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void cboTreatmentType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        var data = this.listTreatmentType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            txtTreatmentTypeCode.Text = data.TREATMENT_TYPE_CODE;
                        }
                        txtDepartmentCode.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    var data = this.listDepartment.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        if (!this.hospitalizeInitADO.DepartmentId.HasValue)
                            throw new Exception("DepartmentId is null");
                        if (Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue).ToString()) == this.hospitalizeInitADO.DepartmentId)
                        {

                            SetShowControl(true);
                            LoadDataToBedRoomCombo(cboBedRoom);
                        }
                        else
                        {
                            this.listBedRoom = null;
                            SetShowControl(false);
                        }
                        txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                        txtHospitalReasonCode.Focus();
                        txtHospitalReasonCode.SelectAll();
                        LoadBedCount(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var data = this.listDepartment.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                            LoadBedCount(data);
                        }
                    }
                    else
                    {
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBedRoomCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtBedRoomCode.Text))
                    {
                        cboBedRoom.EditValue = null;
                        cboBedRoom.Focus();
                        cboBedRoom.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_BED_ROOM> searchs = null;
                        var listData1 = this.listBedRoom.Where(o => o.BED_ROOM_CODE.ToUpper().Contains(txtBedRoomCode.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.BED_ROOM_CODE.ToUpper() == txtBedRoomCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtBedRoomCode.Text = searchs[0].BED_ROOM_CODE;
                            cboBedRoom.EditValue = searchs[0].ID;
                            chkPrintHospitalizeExam.Focus();
                        }
                        else
                        {
                            cboBedRoom.Focus();
                            cboBedRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    var data = this.listBedRoom.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                    if (data != null)
                    {
                        txtBedRoomCode.Text = data.BED_ROOM_CODE;
                    }
                    chkPrintHospitalizeExam.Focus();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBedRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBedRoom.EditValue != null)
                    {
                        var data = this.listBedRoom.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboBedRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtBedRoomCode.Text = data.BED_ROOM_CODE;
                        }
                        chkPrintHospitalizeExam.Focus();
                    }
                    else
                    {
                        cboBedRoom.Focus();
                        cboBedRoom.ShowPopup();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtLogTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTreatmentTypeCode.Focus();
                    txtTreatmentTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtDepartmentCode.Text))
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_DEPARTMENT_1> searchs = null;
                        var listData = this.listDepartment.Where(o => o.DEPARTMENT_CODE.ToUpper().Contains(txtDepartmentCode.Text.ToUpper())).ToList();
                        if (listData != null && listData.Count > 0)
                        {
                            searchs = (listData.Count == 1) ? listData : (listData.Where(o => o.DEPARTMENT_CODE.ToUpper() == txtDepartmentCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            if (searchs[0].ID == this.hospitalizeInitADO.DepartmentId)
                            {

                                SetShowControl(true);
                                LoadDataToBedRoomCombo(cboBedRoom);
                            }
                            else
                            {
                                this.listBedRoom = null;
                                SetShowControl(false);
                            }

                            txtDepartmentCode.Text = searchs[0].DEPARTMENT_CODE;
                            cboDepartment.EditValue = searchs[0].ID;
                            txtBedRoomCode.Focus();
                            txtBedRoomCode.SelectAll();
                            LoadBedCount(searchs[0]);
                        }
                        else
                        {
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboTreatmentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTreatmentType.EditValue != null)
                {
                    SetDataComboDepartment(Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()));
                }
                else
                {
                    cboDepartment.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintHospitalizeExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateCheckPrintAndSign();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateCheckPrintAndSign()
        {
            try
            {
                if (chkPrintHospitalizeExam.CheckState == CheckState.Checked)
                {
                    lciSignAndPrint.Enabled = true;
                    lciSign.Enabled = true;
                    isNotLoadWhileChangeControlStateInFirst = true;
                    InitControlState();
                }
                else
                {
                    chkPrintDocumentSigned.CheckState = CheckState.Unchecked;
                    chkSign.CheckState = CheckState.Unchecked;
                    lciSignAndPrint.Enabled = false;
                    lciSign.Enabled = false;
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
                chkPrintDocumentSigned.Enabled = chkSign.Checked;
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                if (chkSign.Checked == false)
                {
                    chkPrintDocumentSigned.Checked = false;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkSign && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkSign;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintDocumentSigned_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrintDocumentSigned && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkPrintDocumentSigned;
                    csAddOrUpdate.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcds.Text))
                {
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = "";
                    chkEditIcd.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    UcIcdNextForcusOut();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UcIcdNextForcusOut()
        {
            try
            {
                chkPrintHospitalizeExam.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode);
                        ValidationICD(10, 500, !this.isAllowNoIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "ICD không đúng";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTraditionIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!ProccessorByTraditionalIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                    {
                        e.Handled = true;
                        return;
                    }
                    txtTraditionIcdText.Focus();
                    txtTraditionIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTraditionIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    HIS.UC.ExamTreatmentFinish.frmSecondaryIcd FormSecondaryIcd = new HIS.UC.ExamTreatmentFinish.frmSecondaryIcd(stringIcds, this.txtTraditionIcdSubCode.Text, this.txtTraditionIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTraditionIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    HIS.UC.ExamTreatmentFinish.frmSecondaryIcd FormSecondaryIcd = new HIS.UC.ExamTreatmentFinish.frmSecondaryIcd(stringTraditionalIcds, this.txtTraditionIcdSubCode.Text, this.txtTraditionIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringTraditionalIcds(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtTraditionIcdSubCode.Text = icdCode;
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtTraditionIcdText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCCS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkCCS && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkCCS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkCCS;
                    csAddOrUpdate.VALUE = (chkCCS.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();

                try
                {
                    if (cboTreatmentType.EditValue != null)
                    {
                        SetDataComboDepartment(Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()));
                    }
                    else
                    {
                        cboDepartment.Properties.DataSource = null;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHospitalReasonCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (popupControlContainer1.Visible)
                    {
                        var dt = this.gridView3.GetFocusedRow() as HIS_HOSPITALIZE_REASON;
                        txtHospitalReasonCode.Text = dt.HOSPITALIZE_REASON_CODE;
                        btnHospitalReasonName.Text = dt.HOSPITALIZE_REASON_NAME;
                    }
                    popupControlContainer1.HidePopup();
                    btnHospitalReasonName.Focus();
                    btnHospitalReasonName.SelectAll();
                }else if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    gridView3.Focus();
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnHospitalReasonName_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
                {
                    cboHospitalReasonName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHospitalReasonName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_HOSPITALIZE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (data != null)
                {
                    var obj = data.FirstOrDefault(o=>o.ID == Int64.Parse(cboHospitalReasonName.EditValue.ToString()));
                    if (obj != null)
                    {
                        IsFirstLoad = true;
                        txtHospitalReasonCode.Text = obj.HOSPITALIZE_REASON_CODE;
                        btnHospitalReasonName.Text = obj.HOSPITALIZE_REASON_NAME;
                        txtBedRoomCode.Focus();
                        txtBedRoomCode.SelectAll();
                        IsFirstLoad = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHospitalReasonCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsFirstLoad)
                    return;
                txtHospitalReasonCode.Refresh();
                string keyWord = txtHospitalReasonCode.Text.Trim();
                
                gridView3.ActiveFilterString = String.Format("[HOSPITALIZE_REASON_CODE] Like '%{0}%'", keyWord);
                gridView3.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                gridView3.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                gridView3.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                gridView3.FocusedRowHandle = 0;
                gridView3.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                gridView3.OptionsFind.HighlightFindResults = true;
                popupControlContainer1.Size = new Size(txtHospitalReasonCode.Size.Width , 150);
                Rectangle buttonBounds = new Rectangle(lciHospitalReasonCode.Bounds.X, lciHospitalReasonCode.Bounds.Y, lciHospitalReasonCode.Bounds.Width, lciHospitalReasonCode.Bounds.Height);
                popupControlContainer1.ShowPopup(new Point(buttonBounds.X + lciHospitalReasonCode.Bounds.X + 1380, buttonBounds.Bottom + 247));
                txtHospitalReasonCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var dt = this.gridView3.GetFocusedRow() as HIS_HOSPITALIZE_REASON;
                txtHospitalReasonCode.Text = dt.HOSPITALIZE_REASON_CODE;
                btnHospitalReasonName.Text = dt.HOSPITALIZE_REASON_NAME;
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    var dt = this.gridView3.GetFocusedRow() as HIS_HOSPITALIZE_REASON;
                    txtHospitalReasonCode.Text = dt.HOSPITALIZE_REASON_CODE;
                    btnHospitalReasonName.Text = dt.HOSPITALIZE_REASON_NAME;
                    popupControlContainer1.HidePopup();
                    btnHospitalReasonName.Focus();
                    btnHospitalReasonName.SelectAll();
                }   
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnHospitalReasonName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    txtBedRoomCode.Focus();
                    txtBedRoomCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintMps178_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintMps178.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintMps178.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintMps178.Name;
                    csAddOrUpdate.VALUE = (chkPrintMps178.Checked ? "1" : "");
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
    }
}
