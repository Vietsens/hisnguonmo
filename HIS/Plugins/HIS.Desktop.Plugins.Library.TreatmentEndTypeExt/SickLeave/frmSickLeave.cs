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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using His.Bhyt.InsuranceExpertise.LDO;
using His.Bhyt.InsuranceExpertise;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave
{
    public partial class frmSickLeave : FormBase
    {
        long treatmentId;
        HIS_TREATMENT treatment;
        List<HIS_TREATMENT> lstTreatmentByPatient = null;
        long CheDoHienThiNoiLamViecManHinhDangKyTiepDon;
        DelegateSelectData ReloadDataTreatmentEndTypeExt;
        TreatmentEndTypeExtData TreatmentEndTypeExtData;
        FormEnum.TYPE type;
        int positionHandleControlLeft = -1;
        HisTreatmentFinishSDO treatmentFinishSDO;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public frmSickLeave(Inventec.Desktop.Common.Modules.Module module, long _treatmentId, FormEnum.TYPE _type, TreatmentEndTypeExtData _sickLeaveData, DelegateSelectData _reloadDataTreatmentEndTypeExt, HisTreatmentFinishSDO _treatmentFinishSDO)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.moduleData = module;
                this.treatmentId = _treatmentId;
                this.ReloadDataTreatmentEndTypeExt = _reloadDataTreatmentEndTypeExt;
                this.TreatmentEndTypeExtData = _sickLeaveData;
                this.type = _type;
                this.treatmentFinishSDO = _treatmentFinishSDO;
                SetIcon();
                if (this.moduleData == null)
                {
                    this.moduleData = new Inventec.Desktop.Common.Modules.Module();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmSickLeave_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboUser();
                ValidateControl();
                GetConfigSda();
                LoadTreatment();
                ValidMaxLengthRequired(false, memPregnancyTerminationReason, 1000, null);
                ValidationControlAge();
                InitComboRelaytionType();
                InitComboWorkPlace();
                InitComboDocumentBook();
                InitUIByFormType();
                SetDataDefault();
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

        private void spinSickLeaveDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtSickLeaveFromTime.Focus();
                    dtSickLeaveFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSickLeaveToTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientMotherName.Focus();
                    txtPatientMotherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtPatientMotherName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientFatherName.Focus();
                    txtPatientFatherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientFatherName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRelativeName.Focus();
                    txtRelativeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboWorkPlace.Enabled)
                    {
                        cboWorkPlace.Focus();
                        cboWorkPlace.SelectAll();
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRelativeType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboRelativeType.EditValue != null)
                {
                    cboWorkPlace.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ValidationControlAge();
                if (treatment != null)
                {
                    if (treatment.TDL_PATIENT_DOB != null)
                    {

                        if (Inventec.Common.DateTime.Calculation.Age(treatment.TDL_PATIENT_DOB) < 7)
                        {
                            if (cboRelativeType.EditValue == null || string.IsNullOrEmpty(cboRelativeType.Text) || string.IsNullOrEmpty(txtRelativeName.Text))
                            {
                                MessageBox.Show(" Bệnh nhân là trẻ em, bắt buộc nhập thông tin người thân và quan hệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                this.positionHandleControlLeft = -1;
                if (!dxValidationProvider1.Validate()) return;

                if (!Check()) return;


                if (this.treatment != null)
                {
                    if (this.treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && type == FormEnum.TYPE.NGHI_OM)
                    {
                        if (spinSickLeaveDay.EditValue != null && spinSickLeaveDay.Value > 30)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Số ngày nghỉ không được vượt quá 30 ngày", "Thông báo");
                            return;
                        }

                        CommonParam param = new CommonParam();
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.PATIENT_ID = treatment.PATIENT_ID;
                        filter.TREATMENT_END_TYPE_EXT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM;
                        filter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                        var dataCheck = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                        if (dataCheck != null && dataCheck.Count > 0)
                        {
                            var dt = dataCheck.Where(o => o.ID != treatment.ID
                                                && o.SICK_LEAVE_FROM != null && Int64.Parse(dtSickLeaveFromTime.DateTime.ToString("yyyyMMdd") + "000000") >= o.SICK_LEAVE_FROM
                                                && o.SICK_LEAVE_TO != null && Int64.Parse(dtSickLeaveFromTime.DateTime.ToString("yyyyMMdd") + "000000") <= o.SICK_LEAVE_TO).ToList();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dt), dt));
                            if (dt != null && dt.Count > 0)
                            {
                                var treatmentCheck = dt.OrderByDescending(o => o.OUT_TIME).ToList()[0];
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Ngày nghỉ ốm giao với ngày nghỉ ốm được cấp của đợt khám trước đó: {0} (nghỉ từ {1} - {2})", treatmentCheck.TREATMENT_CODE,
                                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentCheck.SICK_LEAVE_FROM ?? 0),
                                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentCheck.SICK_LEAVE_TO ?? 0)), "Thông báo");
                                return;
                            }
                        }
                    }

                }

                TreatmentEndTypeExtData sickLeaveOut = new TreatmentEndTypeExtData();
                if (spinSickLeaveDay.EditValue != null)
                    sickLeaveOut.SickLeaveDay = spinSickLeaveDay.Value;
                sickLeaveOut.MotherName = txtPatientMotherName.Text.Trim();
                sickLeaveOut.FatherName = txtPatientFatherName.Text.Trim();
                sickLeaveOut.PatientRelativeName = txtRelativeName.Text.Trim();
                sickLeaveOut.PatientRelativeType = cboRelativeType.Text;
                string sothe = "";
                if (!string.IsNullOrEmpty(txtSoThe.Text))
                {
                    sothe = txtSoThe.Text.Replace("-", "");
                }
                if (!string.IsNullOrEmpty(sothe))
                {
                    sickLeaveOut.SickHeinCardNumber = sothe.Trim().ToUpper();//txtSoThe.Text;
                }

                if (!string.IsNullOrEmpty(txtBhxhCode.Text.Trim()))
                {
                    sickLeaveOut.SocialInsuranceNumber = txtBhxhCode.Text.Trim();
                }

                if (dtSickLeaveFromTime.EditValue != null)
                    sickLeaveOut.SickLeaveFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSickLeaveFromTime.DateTime);
                if (dtSickLeaveToTime.EditValue != null)
                    sickLeaveOut.SickLeaveTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSickLeaveToTime.DateTime);
                if (cboDocumentBook.EditValue != null)
                    sickLeaveOut.DocumentBookId = (long)cboDocumentBook.EditValue;
                sickLeaveOut.PatientWorkPlace = txtPatientWorkPlace.Text;
                if (!String.IsNullOrEmpty((cboUser.EditValue ?? "").ToString()))
                {
                    sickLeaveOut.Loginname = (string)cboUser.EditValue;
                    sickLeaveOut.Username = cboUser.Text;
                }
                if (cboWorkPlace.EditValue != null)
                {
                    sickLeaveOut.WorkPlaceId = (long)cboWorkPlace.EditValue;
                }

                switch (type)
                {
                    case FormEnum.TYPE.NGHI_OM:
                        sickLeaveOut.TreatmentEndTypeExtId = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM;
                        break;
                    case FormEnum.TYPE.NGHI_DUONG_THAI:
                        sickLeaveOut.TreatmentEndTypeExtId = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI;
                        break;
                    //case FormEnum.TYPE.NGHI_VIEC_HUONG_BHXH:
                    //    sickLeaveOut.TreatmentEndTypeExtId = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_VIEC_HUONG_BHXH;
                    //    break;
                    default:
                        break;
                }
                sickLeaveOut.EndTypeExtNote = txtEndTypeExtNote.Text.Trim();
                //sickLeaveOut.Babes = GetBabys();

                sickLeaveOut.TreatmentMethod = memTreatmentMethod.Text.Trim();
                sickLeaveOut.IsPregnancyTermination = chkIsPregnancyTermination.Checked;
                sickLeaveOut.PregnancyTerminationReason = memPregnancyTerminationReason.Text.Trim();
                if (!string.IsNullOrEmpty(txtGestationAge.Text.Trim()))
                    sickLeaveOut.GestationalAge = Int64.Parse(txtGestationAge.Text.Trim());
                else
                    sickLeaveOut.GestationalAge = null;
                if (dtPregnancyTerminationTime.EditValue != null)
                    sickLeaveOut.PregnancyTerminationTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPregnancyTerminationTime.DateTime);
                else
                    sickLeaveOut.PregnancyTerminationTime = null;

                Inventec.Common.Logging.LogSystem.Debug("TreatmentEndTypeExt.frmSickLeave.btnSave_Click____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sickLeaveOut), sickLeaveOut));

                if (this.ReloadDataTreatmentEndTypeExt != null)
                    this.ReloadDataTreatmentEndTypeExt(sickLeaveOut);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditActionAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var babyADOs = gridControlMaternityLeave.DataSource as List<BabyADO>;
                BabyADO babyADO = new BabyADO();
                babyADOs.Add(babyADO);
                gridControlMaternityLeave.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditActionDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var babyADOs = gridControlMaternityLeave.DataSource as List<BabyADO>;
                var babyADO = gridViewMaternityLeave.GetFocusedRow() as BabyADO;
                if (babyADO != null)
                {
                    babyADOs.Remove(babyADO);
                    gridControlMaternityLeave.RefreshDataSource();
                    gridViewMaternityLeave.FocusedColumn = gridViewMaternityLeave.Columns[1];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSickLeaveFromTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtSickLeaveToTime.EditValue == null || dtSickLeaveToTime.DateTime == DateTime.MinValue)
                {
                    this.CalculateDateTo();
                }

                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSickLeaveToTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtSickLeaveFromTime.EditValue == null || dtSickLeaveFromTime.DateTime == DateTime.MinValue)
                {
                    this.CalculateDateFrom();
                }

                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSickLeaveDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SpinEdit editor = sender as SpinEdit;
                //if (editor != null && editor.EditorContainsFocus)
                //{
                if (dtSickLeaveFromTime.EditValue != null)
                {
                    this.CalculateDateTo();
                }
                else
                {
                    this.CalculateDateFrom();
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void CalculateDateFrom()
        {
            try
            {
                if (dtSickLeaveToTime.EditValue != null && dtSickLeaveToTime.DateTime != DateTime.MinValue && spinSickLeaveDay.EditValue != null)
                {
                    dtSickLeaveFromTime.DateTime = dtSickLeaveToTime.DateTime.AddDays((double)(-spinSickLeaveDay.Value + 1));
                }
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

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
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

        private void txtPatientWorkPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if(txtSoThe.Enabled)
                    {
                        txtSoThe.Focus();
                        txtSoThe.SelectAll();
                    }
                    else
                    {
                        txtBhxhCode.Focus();
                        txtBhxhCode.SelectAll();
                    }    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRelativeType.Focus();
                    cboRelativeType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoThe_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string heinCardNumber = this.txtSoThe.Text;
                    heinCardNumber = HeinCardHelper.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());

                    bool valid = true;
                    valid = valid && new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heinCardNumber);
                    if (valid && !String.IsNullOrEmpty(heinCardNumber))
                    {
                        this.dxErrorProvider1.ClearErrors();
                        btnSave.Focus();
                    }
                    else
                    {
                        this.dxErrorProvider1.SetError(this.txtSoThe, "Người dùng nhập số thẻ BHYT không hợp lệ");
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoThe_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Người dùng nhập số thẻ BHYT không hợp lệ";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisWorkPlace").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisWorkPlace'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisWorkPlace' is not plugins");

                    List<object> listArgs = new List<object>();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, (this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0), (this.currentModuleBase != null ? this.currentModuleBase.RoomTypeId : 0)), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                    InitComboWorkPlace();

                    //this.workPlaceProcessor.Reload(WorkPlaceProcessor.Template.Combo1, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().Where(p => p.IS_ACTIVE == 1).ToList());
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

        private void FocusWhileSelectedUser()
        {
            try
            {
                spinSickLeaveDay.Focus();
                spinSickLeaveDay.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBaby_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfantInformation").FirstOrDefault();
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(treatment.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                    LoadBabyInfor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBabyInfor()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBabyViewFilter filter = new HisBabyViewFilter();
                filter.TREATMENT_ID = this.treatmentId;
                List<V_HIS_BABY> babes = new BackendAdapter(param).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, filter, param);
                if (babes != null && babes.Count > 0)
                {
                    gridControlMaternityLeave.DataSource = babes;
                    gridControlMaternityLeave.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaternityLeave_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_BABY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "BORN_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.BORN_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSoThe_Leave(object sender, EventArgs e)
        {
            try
            {
                if (lciBhxhCode.Visible && !string.IsNullOrEmpty(txtSoThe.Text))
                {
                    var soThe = txtSoThe.Text.Replace("-", "");
                    if (soThe.Length > 10) txtBhxhCode.Text = soThe.Substring(soThe.Length - 10, 10);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGestationAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsPregnancyTermination_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsPregnancyTermination.Checked)
                {
                    lciGestationalAge.AppearanceItemCaption.ForeColor = Color.Maroon;
                    memPregnancyTerminationReason.ReadOnly = false;
                    dtPregnancyTerminationTime.ReadOnly = false;
                    lciPregnancyTerminationReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciPregnancyTerminationTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidMaxLengthRequired(true, memPregnancyTerminationReason, 1000, "Bắt buộc nhập thông tin lý do đình chỉ trong trường hợp đình chỉ thai nghén");
                    ValidMaxLengthRequired(true, txtGestationAge, 0, "Bắt buộc nhập thông tin tuổi thai trong trường hợp đình chỉ thai nghén");
                    ValidMaxLengthRequired(true, dtPregnancyTerminationTime, 0, "Bắt buộc nhập thông tin thời gian đình chỉ trong trường hợp đình chỉ thai nghén");
                }
                else
                {
                    lciGestationalAge.OptionsToolTip.ToolTip = null;
                    lciGestationalAge.AppearanceItemCaption.ForeColor = Color.Black;
                    memPregnancyTerminationReason.Text = null;
                    memPregnancyTerminationReason.ReadOnly = true;
                    lciPregnancyTerminationReason.AppearanceItemCaption.ForeColor = Color.Black;
                    dtPregnancyTerminationTime.EditValue = null;
                    dtPregnancyTerminationTime.ReadOnly = true;
                    lciPregnancyTerminationTime.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider1.SetValidationRule(txtGestationAge, null);
                    dxValidationProvider1.SetValidationRule(memPregnancyTerminationReason, null);
                    dxValidationProvider1.SetValidationRule(dtPregnancyTerminationTime, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidMaxLengthRequired(bool IsRequied, BaseControl control, int maxlength, string tooltip)
        {
            try
            {
                ValidateMaxLength valid = new ValidateMaxLength();
                valid.tooltip = tooltip;
                valid.IsRequired = IsRequied;
                valid.txt = control;
                valid.maxLength = maxlength;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGestationAge_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!memPregnancyTerminationReason.ReadOnly)
                    {
                        memPregnancyTerminationReason.Focus();
                        memPregnancyTerminationReason.SelectAll();
                    }
                    else
                    {
                        memTreatmentMethod.Focus();
                        memTreatmentMethod.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void memTreatmentMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEndTypeExtNote.Focus();
                    txtEndTypeExtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void memPregnancyTerminationReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    memTreatmentMethod.Focus();
                    memTreatmentMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocumentBook_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDocumentBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        HIS_PATIENT Patient = null;
        private void btnLaySoTheBHYT_Click(object sender, EventArgs e)
        {
            MOS.Filter.HisPatientFilter patientFilter = new MOS.Filter.HisPatientFilter();
            patientFilter.ID = this.treatment.PATIENT_ID;
            Patient = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null).FirstOrDefault();
            if (!dxValidationProvider1.Validate(txtBhxhCode) || Patient == null) return;
            LogSystem.Debug("Bat dau check cong BHXH ");
            CheckBhxh();
        }
        private HIS_EMPLOYEE GetEmployee(string username)
        {
            HIS_EMPLOYEE result = new HIS_EMPLOYEE();
            try
            {
                var rs = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(s => s.LOGINNAME == username).FirstOrDefault();
                if (rs != null)
                {
                    result = rs;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }
        List<string> connectInfors = new List<string>();
        string api = "";
        string nameCb = "";
        string cccdCb = "";
        private async Task CheckBhxh()
        {
            ResultHistoryLDO rsData = null;
            try
            {
                BHXHLoginCFG.LoadConfig();
                CommonParam param = new CommonParam();
                ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                
                CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                checkHistoryLDO.maThe = txtBhxhCode.Text.Trim();
                checkHistoryLDO.ngaySinh = Patient.IS_HAS_NOT_DAY_DOB == 1 ? Patient.DOB.ToString().Substring(0, 4) : ((Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Patient.DOB) ?? DateTime.MinValue).ToString("dd/MM/yyyy"));
                checkHistoryLDO.hoTen = Inventec.Common.String.Convert.HexToUTF8Fix(Patient.VIR_PATIENT_NAME.ToLower());
                checkHistoryLDO.hoTen = (String.IsNullOrEmpty(checkHistoryLDO.hoTen) ? Patient.VIR_PATIENT_NAME.ToLower() : checkHistoryLDO.hoTen);
                string username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employee = GetEmployee(username);
                try
                {
                    LogSystem.Debug("Get value config check cong BHXH: ");
                    string connect_infor = HisConfigs.Get<string>("HIS.CHECK_HEIN_CARD.BHXH__API");
                    if (!string.IsNullOrEmpty(connect_infor))
                    {
                        connectInfors = connect_infor.Split('|').ToList();
                        api = connectInfors.Count > 0 ? connectInfors[0] : string.Empty;

                        nameCb = connectInfors.Count > 1 && !string.IsNullOrEmpty(connectInfors[1]) ? connectInfors[1] : employee.TDL_USERNAME;
                        cccdCb = connectInfors.Count > 2 && !string.IsNullOrEmpty(connectInfors[2]) ? connectInfors[2] : employee.IDENTIFICATION_NUMBER;

                        if(connectInfors.Count > 1)LogSystem.Debug("BHXHLoginCFG.OFFICERNAME: " + connectInfors[1]);
                        if(connectInfors.Count > 2)LogSystem.Debug("BHXHLoginCFG.CCCDOFFICER: " + connectInfors[2]);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                
                LogSystem.Debug("employee: " + LogUtil.TraceData("employee", employee));
                
                apiInsuranceExpertise.ApiEgw = api;
                checkHistoryLDO.hoTenCb = nameCb;
                checkHistoryLDO.cccdCb = cccdCb;

                Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 1");
                if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkHistoryLDO), checkHistoryLDO));
                    rsData = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsData), rsData));
                    if (rsData != null)
                    {
                        if (rsData.maKetQua == "000" || rsData.maKetQua == "001" || rsData.maKetQua == "002" || rsData.maKetQua == "004")
                        {
                            txtSoThe.Text = rsData.maThe;
                            if (!string.IsNullOrEmpty(rsData.ngaySinh) && checkHistoryLDO.ngaySinh != rsData.ngaySinh && Patient != null && DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn cập nhật lại ngày sinh của bệnh nhân không?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                WaitingManager.Show();
                                var splDob = rsData.ngaySinh.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (splDob.Count > 2)
                                {
                                    Patient.DOB = Int64.Parse(splDob[2] + splDob[1] + splDob[0] + "000000");
                                    Patient.IS_HAS_NOT_DAY_DOB = null;
                                }
                                else
                                {
                                    Patient.DOB = Int64.Parse(splDob[0] + "0101000000");
                                    Patient.IS_HAS_NOT_DAY_DOB = 1;

                                }
                                HisPatientUpdateSDO sdo = new HisPatientUpdateSDO();
                                sdo.HisPatient = Patient;
                                sdo.TreatmentId = this.treatment.ID;
                                var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, sdo, param);
                                WaitingManager.Hide();
                                MessageManager.Show(this.ParentForm, param, resultData != null);
                            }
                        }
                        else if (!string.IsNullOrEmpty(rsData.ghiChu))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(rsData.ghiChu, "Thông báo");
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
